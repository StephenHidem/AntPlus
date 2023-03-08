// See https://aka.ms/new-console-template for more information
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

// create UDP client
using UdpClient udpServer = new(2000, AddressFamily.InterNetworkV6);

// create multicast endpoint to send ANT data to
IPEndPoint endPoint = new(IPAddress.Parse("FF02::1"), 55436);

// create and configure ANT radio
using AntRadio antRadio = new();
antRadio.SetNetworkKey(0, new byte[] { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 });
antRadio.EnableRxExtendedMessages(true);
antRadio.GetChannel(0).AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
antRadio.GetChannel(0).SetChannelID(new ChannelId(0), 500);
antRadio.GetChannel(0).SetChannelFreq(57, 500);
antRadio.OpenRxScanMode();
IAntChannel channel = antRadio.GetChannel(0);
channel.ChannelResponse += Channel_ChannelResponse;

// create background task to receive UDP directed to this server
Task.Run(async () =>
{
    while (true)
    {
        UdpReceiveResult result = await udpServer.ReceiveAsync();
        Debug.WriteLine(result.Buffer.ToString());
        ChannelId channelId = new(BitConverter.ToUInt32(result.Buffer, 0));
        byte[] msg = result.Buffer.Skip(4).Take(8).ToArray();
        uint ackWaitTime = BitConverter.ToUInt32(result.Buffer, 12);
        antRadio.GetChannel(1).SendExtAcknowledgedData(channelId, msg, ackWaitTime);
    }
});

// loop until terminated
Console.WriteLine("Press Enter to terminate.");
Console.ReadLine();

// clean up
channel.ChannelResponse -= Channel_ChannelResponse;

void Channel_ChannelResponse(object? sender, AntResponse e)
{
    MemoryStream ms = new();
    DataContractSerializer dcs = new(typeof(AntResponse));
    dcs.WriteObject(ms, e);

    ms.Position = 0;
    byte[] txt = Encoding.Default.GetBytes(new StreamReader(ms).ReadToEnd());

    _ = udpServer.Send(txt, txt.Length, endPoint);
}
