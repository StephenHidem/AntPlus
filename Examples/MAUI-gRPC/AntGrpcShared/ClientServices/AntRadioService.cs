﻿using AntConfigurationGrpcService;
using AntControlGrpcService;
using AntCryptoGrpcService;
using AntRadioGrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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
    public partial class AntRadioService : IAntRadio
    {
        private readonly IPAddress grpAddress = IPAddress.Parse("239.55.43.6");
        private const int multicastPort = 55437;        // multicast port
        private const int gRPCPort = 5073;              // gRPC port

        private readonly ILoggerFactory _loggerFactory;
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
        /// Event triggered when an RPC exception is received.
        /// </summary>
        public event EventHandler<RpcException>? RpcExceptionReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="AntRadioService"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        /// <param name="grpcChannelOptions">Optional gRPC channel configuration options.</param>
        public AntRadioService(
            ILoggerFactory loggerFactory, CancellationTokenSource cancellationTokenSource,
            GrpcChannelOptions? grpcChannelOptions = default)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<AntRadioService>();
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
            IPEndPoint multicastEndPoint = new(grpAddress, multicastPort);
            byte[] req = Encoding.ASCII.GetBytes("MauiAntGrpcClient discovery request");

            // initiate receive
            using UdpClient udpClient = new(0);
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

                    // create a gRPC channel to the server
                    UriBuilder uriBuilder = new("http", ServerIPAddress.ToString(), gRPCPort);
                    _grpcChannel = GrpcChannel.ForAddress(uriBuilder.Uri, _grpcChannelOptions);
                    _client = new gRPCAntRadio.gRPCAntRadioClient(_grpcChannel);
                    _control = new gRPCAntControl.gRPCAntControlClient(_grpcChannel);
                    _config = new gRPCAntConfiguration.gRPCAntConfigurationClient(_grpcChannel);
                    _crypto = new gRPCAntCrypto.gRPCAntCryptoClient(_grpcChannel);

                    // subscribe to radio response updates
                    HandleRadioResponseUpdates(_cancellationTokenSource.Token);

                    // get properties from server
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

        /// <summary>
        /// Handles radio response updates.
        /// </summary>
        /// <param name="cancellationToken">Cancels subscription to ChannelResponseUpdate.</param>
        private async void HandleRadioResponseUpdates(CancellationToken cancellationToken)
        {
            using var response = _client!.Subscribe(new Empty(), cancellationToken: cancellationToken);
            try
            {
                await foreach (AntResponseReply? update in response.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    _logger.LogDebug("OnDeviceResponse: {Channel}, {ResponseId}, {Data}", update.ChannelNumber, (MessageId)update.ResponseId, BitConverter.ToString(update.Payload.ToByteArray()));
                    RadioResponse?.Invoke(this, new GrpcAntResponse(update));
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                _logger.LogInformation("RpcException: unavailable");
                RpcExceptionReceived?.Invoke(this, ex);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogInformation("RpcException: operation cancelled");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("OperationCanceledException");
            }
        }

        /// <inheritdoc/>
        public void CancelTransfers(int cancelWaitTime)
        {
            _client!.CancelTransfers(new CancelTransfersRequest { WaitTime = cancelWaitTime });
        }

        /// <inheritdoc/>
        public IAntChannel GetChannel(int num)
        {
            _ = _client!.GetChannel(new GetChannelRequest { ChannelNumber = (byte)num });
            return new AntChannelService(_loggerFactory.CreateLogger<AntChannelService>(), (byte)num, _grpcChannel!);
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
                channels[i] = new AntChannelService(_loggerFactory.CreateLogger<AntChannelService>(), i, _grpcChannel);
            }
            channels[0].HandleChannelResponseUpdates(_cancellationTokenSource.Token);
            return channels;
        }

        /// <inheritdoc/>
        public AntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime = 500)
        {
            AntResponseReply response = _client!.ReadUserNvm(new ReadUserNvmRequest { Address = address, Size = size, WaitResponseTime = responseWaitTime });
            return new GrpcAntResponse(response);
        }

        /// <inheritdoc/>
        public AntResponse RequestMessageAndResponse(SmallEarthTech.AntRadioInterface.RequestMessageID messageID, uint responseWaitTime, byte channelNum = 0)
        {
            AntResponseReply response = _client!.RequestMessageAndResponse(new RequestMessageAndResponseRequest { MsgId = (AntRadioGrpcService.RequestMessageID)messageID, WaitResponseTime = responseWaitTime, ChannelNumber = channelNum });
            return new GrpcAntResponse(response);
        }

        /// <inheritdoc/>
        public bool WriteRawMessageToDevice(byte msgID, byte[] msgData)
        {
            BoolValue reply = _client!.WriteRawMessageToDevice(new WriteRawMessageToDeviceRequest { MsgId = msgID, MsgData = Google.Protobuf.ByteString.CopyFrom(msgData) });
            return reply.Value;
        }
    }
}
