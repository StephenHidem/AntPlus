﻿// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;
using SmallEarthTech.AntUsbStick;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

// opening credits
Console.WriteLine(string.Format("ANT Multicast Server - Version {0}.", Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)));
Console.WriteLine("Copyright 2023 Stephen Hidem.");

// dependency services
IHost host = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs()).
    ConfigureServices(s =>
    {
        s.AddSingleton<IAntRadio, AntRadio>();
    }).
    Build();

// create UDP client
using UdpClient udpServer = new(2000, AddressFamily.InterNetworkV6);

// create multicast endpoint to send ANT data to
IPEndPoint endPoint = new(IPAddress.Parse("FF02::1"), 55436);
Console.WriteLine(string.Format("Establishing IPv6 endpoint - {0}.", endPoint.ToString()));

// create and configure ANT radio
Console.WriteLine("Configuring ANT radio (uses the first USB stick found).");
AntRadio antRadio = (AntRadio)host.Services.GetRequiredService<IAntRadio>();
IAntChannel channel = antRadio.InitializeContinuousScanMode();
channel.ChannelResponse += Channel_ChannelResponse;

// create background task to receive UDP directed to this server from any clients
_ = Task.Run(async () =>
{
    using IAntChannel txChannel = antRadio.GetChannel(1);
    while (true)
    {
        UdpReceiveResult result = await udpServer.ReceiveAsync();
        Debug.WriteLine(BitConverter.ToString(result.Buffer));
        ChannelId channelId = new(BitConverter.ToUInt32(result.Buffer, 0));
        byte[] msg = result.Buffer.Skip(4).Take(8).ToArray();
        uint ackWaitTime = BitConverter.ToUInt32(result.Buffer, 12);
        _ = await txChannel.SendExtAcknowledgedData(channelId, msg, ackWaitTime);
    }
});

Console.WriteLine("Up and running!");

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
