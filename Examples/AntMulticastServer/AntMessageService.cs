using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace AntMulticastServer
{
    internal class AntMessageService : BackgroundService
    {
        private readonly ILogger<AntMessageService> _logger;
        private readonly IAntRadio _antRadio;
        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _udpEndPoint;

        public AntMessageService(ILogger<AntMessageService> logger, IAntRadio antRadio, UdpClient udpClient)
        {
            _logger = logger;
            _antRadio = antRadio;
            _udpClient = udpClient;

            // create multicast endpoint to send ANT broadcast data to
            _udpEndPoint = new(IPAddress.Parse("FF02::1"), 55436);
            Console.WriteLine(string.Format("Establishing IPv6 endpoint - {0}.", _udpEndPoint.ToString()));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // configure ANT radio
            Console.WriteLine("Configuring ANT radio (uses the first USB stick found).");
            IAntChannel[] channel = await _antRadio.InitializeContinuousScanMode();
            channel[0].ChannelResponse += Channel_ChannelResponse;
            Console.WriteLine("Up and running!");

            // relay received messages to ANT radio to send to an ANT device
            while (!stoppingToken.IsCancellationRequested)
            {
                UdpReceiveResult result = await _udpClient.ReceiveAsync(stoppingToken);
                _logger.LogDebug("Multicast reply: {Reply}", BitConverter.ToString(result.Buffer));
                ChannelId channelId = new(BitConverter.ToUInt32(result.Buffer, 0));
                byte[] msg = result.Buffer.Skip(4).Take(8).ToArray();
                uint ackWaitTime = BitConverter.ToUInt32(result.Buffer, 12);
                _ = await channel[1].SendExtAcknowledgedData(channelId, msg, ackWaitTime);
            }

            // remove handler
            channel[0].ChannelResponse -= Channel_ChannelResponse;
        }

        /// <summary>
        /// Messages received from the ANT radio are serialized and forwarded to the multicast endpoint.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The ANT response.</param>
        private void Channel_ChannelResponse(object? sender, AntResponse e)
        {
            MemoryStream ms = new();
            DataContractSerializer dcs = new(typeof(AntResponse));
            dcs.WriteObject(ms, e);

            ms.Position = 0;
            byte[] txt = Encoding.Default.GetBytes(new StreamReader(ms).ReadToEnd());

            _ = _udpClient.Send(txt, txt.Length, _udpEndPoint);
        }
    }
}
