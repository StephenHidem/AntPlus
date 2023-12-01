using AntRadioGrpcService;
using CommunityToolkit.Mvvm.ComponentModel;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MauiAntClientApp.Services
{
    public partial class AntRadioService(ILogger<AntRadioService> logger) : ObservableObject, IAntRadio
    {
        private readonly IPAddress grpAddress = IPAddress.Parse("239.55.43.6");
        private const int multicastPort = 55437;        // multicast port
        private const int gRPCPort = 7072;              // gRPC port

        private AntRadio.AntRadioClient? client;
        private readonly ILogger<AntRadioService> _logger = logger;

        private IAntChannel[]? radioChannels;
        [ObservableProperty]
        private IPAddress? serverIPAddress;
        [ObservableProperty]
        private string? productDescription;
        [ObservableProperty]
        private string? serialString;
        [ObservableProperty]
        private string? hostVersion;

        public int NumChannels => throw new NotImplementedException();

        public FramerType OpenedFrameType => throw new NotImplementedException();

        public PortType OpenedPortType => throw new NotImplementedException();

        public uint SerialNumber => throw new NotImplementedException();

        public event EventHandler<AntResponse>? RadioResponse;

        public async Task FindAntRadioServerAsync()
        {
            IPEndPoint multicastEndPoint = new(grpAddress, multicastPort);
            byte[] req = Encoding.ASCII.GetBytes("AntRadioServer");
            UdpReceiveResult result;
            using UdpClient udpClient = new(AddressFamily.InterNetwork);

            while (true)
            {
                using CancellationTokenSource cts = new(2000);

                // send request for ANT radio server
                _ = udpClient.Send(req, req.Length, multicastEndPoint);
                try
                {
                    result = await udpClient.ReceiveAsync(cts.Token);
                    ServerIPAddress = result.RemoteEndPoint.Address;
                    string msg = Encoding.ASCII.GetString(result.Buffer);
                    _logger.LogInformation("ANT radio endpoint {ServerAddress}, message {Msg}", ServerIPAddress, msg);
                    break;
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("SearchAsync: OperationCanceledException. Retry.");
                }
            }

            UriBuilder uriBuilder = new("https", ServerIPAddress.ToString(), gRPCPort);
            GrpcChannel channel = GrpcChannel.ForAddress(uriBuilder.Uri, new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
            });
            client = new AntRadio.AntRadioClient(channel);
            PropertiesReply reply = await client.GetPropertiesAsync(new PropertiesRequest());
            ProductDescription = reply.ProductDescription;
            SerialString = reply.SerialString;
            HostVersion = reply.HostVersion;
        }

        public IAntChannel[] InitializeContinuousScanMode()
        {
            InitScanModeReply reply = client!.InitializeContinuousScanMode(new InitScanModeRequest());
            for (int i = 0; i < reply.NumChannels; i++)
            {

            }
            return radioChannels;
        }

        public void CancelTransfers(int cancelWaitTime)
        {
            throw new NotImplementedException();
        }

        public IAntChannel GetChannel(int num)
        {
            throw new NotImplementedException();
        }

        public DeviceCapabilities GetDeviceCapabilities()
        {
            throw new NotImplementedException();
        }

        public DeviceCapabilities GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public DeviceCapabilities GetDeviceCapabilities(uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public AntResponse ReadUserNvm(ushort address, byte size)
        {
            throw new NotImplementedException();
        }

        public AntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public AntResponse RequestMessageAndResponse(byte channelNum, RequestMessageID messageID, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public AntResponse RequestMessageAndResponse(RequestMessageID messageID, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool WriteRawMessageToDevice(byte msgID, byte[] msgData)
        {
            throw new NotImplementedException();
        }
    }
}
