using AntRadioGrpcService;
using CommunityToolkit.Mvvm.ComponentModel;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmallEarthTech.AntRadioInterface;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MauiAntGrpcClient.Services
{
    public partial class AntRadioService(ILogger<AntRadioService> logger, ILoggerFactory loggerFactory) : ObservableObject, IAntRadio
    {
        private readonly IPAddress grpAddress = IPAddress.Parse("239.55.43.6");
        private const int multicastPort = 55437;        // multicast port
        private const int gRPCPort = 5073;              // gRPC port

        private gRPCAntRadio.gRPCAntRadioClient? _client;
        private readonly ILoggerFactory _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        private readonly ILogger<AntRadioService> _logger = logger ?? NullLogger<AntRadioService>.Instance;
        private GrpcChannel? _channel;

        [ObservableProperty]
        private IPAddress? serverIPAddress;
        [ObservableProperty]
        private string? productDescription;
        [ObservableProperty]
        private string? serialString;
        [ObservableProperty]
        private string? hostVersion;

        public int NumChannels => throw new NotImplementedException();
        public uint SerialNumber => throw new NotImplementedException();

        [Obsolete("This property is only used by the underlying native DLL. It will be removed in the next release.")]
        public FramerType OpenedFrameType => throw new NotImplementedException();

        [Obsolete("This property is only used by the underlying native DLL. It will be removed in the next release.")]
        public PortType OpenedPortType => throw new NotImplementedException();

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
                    _logger.LogInformation("FindAntRadioServerAsync: OperationCanceledException. Retry.");
                }
            }

            UriBuilder uriBuilder = new("http", ServerIPAddress.ToString(), gRPCPort);
            _channel = GrpcChannel.ForAddress(uriBuilder.Uri);
            _client = new gRPCAntRadio.gRPCAntRadioClient(_channel);
            PropertiesReply reply = await _client.GetPropertiesAsync(new Empty());
            ProductDescription = reply.ProductDescription;
            SerialString = reply.SerialString;
            HostVersion = reply.HostVersion;
        }

        public async Task<IAntChannel[]> InitializeContinuousScanMode()
        {
            if (_channel == null)
            {
                _logger.LogError("_channel is null!");
                return [];
            }
            InitScanModeReply reply = await _client!.InitializeContinuousScanModeAsync(new Empty());
            AntChannelService[] channels = new AntChannelService[reply.NumChannels];
            ILogger<AntChannelService> logger = _loggerFactory.CreateLogger<AntChannelService>();
            for (byte i = 0; i < reply.NumChannels; i++)
            {
                channels[i] = new AntChannelService(logger, i, _channel);
            }
            return channels;
        }

        public void CancelTransfers(int cancelWaitTime)
        {
            throw new NotImplementedException();
        }

        public IAntChannel GetChannel(int num)
        {
            throw new NotImplementedException();
        }

        public async Task<DeviceCapabilities> GetDeviceCapabilities()
        {
            GetDeviceCapabilitiesReply caps = await _client!.GetDeviceCapabilitiesAsync(new GetDeviceCapabilitiesRequest());
            return new GrpcDeviceCapabilities(caps);
        }

        public async Task<DeviceCapabilities> GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime)
        {
            GetDeviceCapabilitiesReply caps = await _client!.GetDeviceCapabilitiesAsync(new GetDeviceCapabilitiesRequest { ForceCopy = forceNewCopy, WaitResponseTime = responseWaitTime });
            return new GrpcDeviceCapabilities(caps);
        }

        public async Task<DeviceCapabilities> GetDeviceCapabilities(uint responseWaitTime)
        {
            GetDeviceCapabilitiesReply caps = await _client!.GetDeviceCapabilitiesAsync(new GetDeviceCapabilitiesRequest { WaitResponseTime = responseWaitTime });
            return new GrpcDeviceCapabilities(caps);
        }

        public AntResponse ReadUserNvm(ushort address, byte size)
        {
            throw new NotImplementedException();
        }

        public AntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public AntResponse RequestMessageAndResponse(byte channelNum, SmallEarthTech.AntRadioInterface.RequestMessageID messageID, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public AntResponse RequestMessageAndResponse(SmallEarthTech.AntRadioInterface.RequestMessageID messageID, uint responseWaitTime)
        {
            throw new NotImplementedException();
        }

        public bool WriteRawMessageToDevice(byte msgID, byte[] msgData)
        {
            throw new NotImplementedException();
        }
    }
}
