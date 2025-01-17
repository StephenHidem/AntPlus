using AntRadioGrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntGrpcShared.ClientServices
{
    /// <summary>
    /// Service for interacting with ANT radio using gRPC.
    /// </summary>
    public class AntRadioService : IAntRadio
    {
        private readonly IPAddress grpAddress = IPAddress.Parse("239.55.43.6");
        private const int multicastPort = 55437;        // multicast port
        private const int gRPCPort = 5073;              // gRPC port

        private readonly ILogger<AntRadioService> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly GrpcChannelOptions _grpcChannelOptions;
        private gRPCAntRadio.gRPCAntRadioClient? _client;
        private GrpcChannel? _grpcChannel;

        /// <summary>
        /// Gets the IP address of the server.
        /// </summary>
        public IPAddress ServerIPAddress { get; private set; } = IPAddress.None;

        /// <inheritdoc/>
        public int NumChannels => throw new NotImplementedException();

        /// <inheritdoc/>
        public string ProductDescription { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public uint SerialNumber { get; private set; }

        /// <inheritdoc/>
        public string Version { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public event EventHandler<AntResponse>? RadioResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="AntRadioService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        /// <param name="grpcChannelOptions">Optional gRPC channel configuration options.</param>
        public AntRadioService(
            ILogger<AntRadioService> logger, CancellationTokenSource cancellationTokenSource,
            GrpcChannelOptions? grpcChannelOptions = null)
        {
            _logger = logger;
            _cancellationTokenSource = cancellationTokenSource;
            _grpcChannelOptions = grpcChannelOptions ?? new GrpcChannelOptions();
        }

        /// <summary>
        /// Searches for an ANT radio server. A receive task creates a UdpClient and then sends a
        /// message to the multicast endpoint every 2 seconds until a server responds. The server
        /// IP address in the response is then used to connect to the server via gRPC.
        /// 
        /// Note: this method is cancellable and can throw an OperationCanceledException.
        /// </summary>
        /// <returns>A void Task.</returns>
        /// <exception cref="OperationCanceledException">Thrown when the CancellationTokenSource is canceled.</exception>
        public async Task FindAntRadioServerAsync()
        {
            IPEndPoint multicastEndPoint = new IPEndPoint(grpAddress, multicastPort);
            byte[] req = Encoding.ASCII.GetBytes("MauiAntGrpcClient discovery request");

            // initiate receive
            using UdpClient udpClient = new UdpClient(0);
            Task<UdpReceiveResult> receiveTask = udpClient.ReceiveAsync();

            // loop every 2 seconds sending a message to the any listening servers
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                // send request for ANT radio server
                _ = udpClient.Send(req, req.Length, multicastEndPoint);

                // wait for response from server, timeout, or cancellation
                if (receiveTask.Wait(2000, _cancellationTokenSource.Token))
                {
                    UdpReceiveResult result = receiveTask.Result;
                    ServerIPAddress = result.RemoteEndPoint.Address;
                    string msg = Encoding.ASCII.GetString(result.Buffer);
                    _logger.LogInformation("ANT radio endpoint {ServerAddress}, message {Msg}", ServerIPAddress, msg);

                    UriBuilder uriBuilder = new UriBuilder("http", ServerIPAddress.ToString(), gRPCPort);
                    _grpcChannel = GrpcChannel.ForAddress(uriBuilder.Uri, _grpcChannelOptions);
                    _client = new gRPCAntRadio.gRPCAntRadioClient(_grpcChannel);
                    PropertiesReply reply = await _client.GetPropertiesAsync(new Empty());
                    ProductDescription = reply.ProductDescription;
                    SerialNumber = reply.SerialNumber;
                    Version = reply.Version;
                    break;
                }
                else
                {
                    _logger.LogInformation("FindAntRadioServerAsync: Timeout. Retry.");
                }
            }
        }

        /// <inheritdoc/>
        public void CancelTransfers(int cancelWaitTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IAntChannel GetChannel(int num)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<DeviceCapabilities> GetDeviceCapabilities(bool forceNewCopy = false, uint responseWaitTime = 1500)
        {
            GetDeviceCapabilitiesReply caps = await _client!.GetDeviceCapabilitiesAsync(new GetDeviceCapabilitiesRequest { ForceCopy = forceNewCopy, WaitResponseTime = responseWaitTime });
            return new GrpcDeviceCapabilities(caps);
        }

        /// <inheritdoc/>
        public async Task<IAntChannel[]> InitializeContinuousScanMode()
        {
            if (_grpcChannel == null)
            {
                throw new InvalidOperationException("gRPC channel is not initialized. Invoke FindAntRadioServerAsync method prior to invoking this method.");
            }

            InitScanModeReply reply = await _client!.InitializeContinuousScanModeAsync(new Empty());
            AntChannelService[] channels = new AntChannelService[reply.NumChannels];
            for (byte i = 0; i < reply.NumChannels; i++)
            {
                channels[i] = new AntChannelService(_logger, i, _grpcChannel);
            }
            channels[0].HandleChannelResponseEvents(_cancellationTokenSource.Token);
            return channels;
        }

        /// <inheritdoc/>
        public AntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime = 500)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public AntResponse RequestMessageAndResponse(SmallEarthTech.AntRadioInterface.RequestMessageID messageID, uint responseWaitTime, byte channelNum = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool WriteRawMessageToDevice(byte msgID, byte[] msgData)
        {
            throw new NotImplementedException();
        }
    }
}
