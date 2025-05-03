using AntRadioGrpcService;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;
using System.Text;

namespace AntGrpcService.Services
{
    public class AntRadioService(ILogger<AntRadioService> logger, IAntRadio antRadio, IAntRadioSubscriberFactory subscriberFactory) : gRPCAntRadio.gRPCAntRadioBase
    {
        public static IAntChannel[] AntChannels { get; private set; } = [];

        public override async Task Subscribe(Empty request, IServerStreamWriter<AntResponseReply> responseStream, ServerCallContext context)
        {
            logger.LogInformation("Radio subscriber entered. Peer = {Peer}", context.Peer);
            using IAntRadioSubscriber subscriber = subscriberFactory.CreateAntRadioSubscriber(antRadio);

            // create a response handler delegate and add it to subscriber
            async void handler(object? sender, AntResponse args) => await WriteUpdateAsync(responseStream, args);
            subscriber.OnAntRadioResponse += handler;

            await AwaitCancellation(context.CancellationToken);

            // remove our response handler from the subscriber
            subscriber.OnAntRadioResponse -= handler;

            logger.LogInformation("Radio subscriber exited. Peer = {Peer}", context.Peer);
        }

        /// <summary>
        /// Writes the ANT response update to the stream.
        /// </summary>
        /// <param name="responseStream">Subscribed stream.</param>
        /// <param name="radioResponse">ANT radio response received.</param>
        /// <returns>Task to await</returns>
        private async Task WriteUpdateAsync(IServerStreamWriter<AntResponseReply> responseStream, AntResponse radioResponse)
        {
            try
            {
                await responseStream.WriteAsync(FromAntResponse(radioResponse));
            }
            catch (Exception e)
            {
                // Handle any errors caused by broken connection, etc.
                logger.LogError(e, "Failed to write radio response.");
            }
        }

        /// <summary>
        /// This task completes when the connection is closed by the client.
        /// </summary>
        /// <param name="token">Client cancellation token</param>
        /// <returns>Task that completes when canceled.</returns>
        private static Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource();
            token.Register(() => completion.SetResult());
            return completion.Task;
        }

        public override Task<PropertiesReply> GetProperties(Empty request, ServerCallContext context)
        {
            logger.LogDebug(nameof(GetProperties));
            SmallEarthTech.AntUsbStick.AntRadio usbAntRadio = (SmallEarthTech.AntUsbStick.AntRadio)antRadio;
            AntResponse rsp = usbAntRadio.RequestMessageAndResponse(SmallEarthTech.AntRadioInterface.RequestMessageID.Version, 500);
            return Task.FromResult(new PropertiesReply
            {
                SerialNumber = usbAntRadio.SerialNumber,
                Version = Encoding.Default.GetString(rsp.Payload!).TrimEnd('\0'),
                ProductDescription = usbAntRadio.ProductDescription
            });
        }

        public override async Task<InitScanModeReply> InitializeContinuousScanMode(Empty request, ServerCallContext context)
        {
            logger.LogDebug(nameof(InitializeContinuousScanMode));
            AntChannels = await antRadio.InitializeContinuousScanMode();

            return new InitScanModeReply
            {
                NumChannels = AntChannels.Length
            };
        }

        public override Task<Empty> CancelTransfers(CancelTransfersRequest request, ServerCallContext context)
        {
            antRadio.CancelTransfers(request.WaitTime);
            return Task.FromResult(new Empty());
        }

        public override Task<GetChannelReply> GetChannel(GetChannelRequest request, ServerCallContext context)
        {
            logger.LogDebug("GetChannel: Channel = {ChannelNumber}", request.ChannelNumber);
            _ = antRadio.GetChannel(request.ChannelNumber);

            return Task.FromResult(new GetChannelReply());
        }

        public override async Task<GetDeviceCapabilitiesReply> GetDeviceCapabilities(GetDeviceCapabilitiesRequest request, ServerCallContext context)
        {
            DeviceCapabilities capabilities = await antRadio.GetDeviceCapabilities(request.ForceCopy, request.WaitResponseTime);
            return new GetDeviceCapabilitiesReply
            {
                MaxAntChannels = capabilities.MaxANTChannels,
                MaxNetworks = capabilities.MaxNetworks,
                NoReceiveChannels = capabilities.NoReceiveChannels,
                NoTransmitChannels = capabilities.NoTransmitChannels,
                NoReceiveMessages = capabilities.NoReceiveMessages,
                NoTransmitMessages = capabilities.NoTransmitMessages,
                NoAckMessages = capabilities.NoAckMessages,
                NoBurstMessages = capabilities.NoBurstMessages,
                PrivateNetworks = capabilities.PrivateNetworks,
                SerialNumber = capabilities.SerialNumber,
                PerChannelTransmitPower = capabilities.PerChannelTransmitPower,
                LowPrioritySearch = capabilities.LowPrioritySearch,
                ScriptSupport = capabilities.ScriptSupport,
                SearchList = capabilities.SearchList,
                OnboardLed = capabilities.OnboardLED,
                ExtendedMessaging = capabilities.ExtendedMessaging,
                ScanModeSupport = capabilities.ScanModeSupport,
                ExtendedChannelAssignment = capabilities.ExtendedChannelAssignment,
                ProximitySearch = capabilities.ProximitySearch,
                AntfsSupport = capabilities.ANTFS_Support,
                FitSupport = capabilities.FITSupport,
                AdvancedBurst = capabilities.AdvancedBurst,
                EventBuffering = capabilities.EventBuffering,
                EventFiltering = capabilities.EventFiltering,
                HighDutySearch = capabilities.HighDutySearch,
                SearchSharing = capabilities.SearchSharing,
                SelectiveDataUpdate = capabilities.SelectiveDataUpdate,
                SingleChannelEncryption = capabilities.SingleChannelEncryption,
                MaxDataChannels = capabilities.MaxDataChannels
            };
        }

        public override Task<AntResponseReply> ReadUserNvm(ReadUserNvmRequest request, ServerCallContext context)
        {
            AntResponse result = antRadio.ReadUserNvm((ushort)request.Address, (byte)request.Size, request.WaitResponseTime);
            return Task.FromResult(FromAntResponse(result));
        }

        public override Task<AntResponseReply> RequestMessageAndResponse(RequestMessageAndResponseRequest request, ServerCallContext context)
        {
            AntResponse result = antRadio.RequestMessageAndResponse((SmallEarthTech.AntRadioInterface.RequestMessageID)request.MsgId, request.WaitResponseTime, (byte)request.ChannelNumber);
            return Task.FromResult(FromAntResponse(result));
        }

        public override Task<BoolValue> WriteRawMessageToDevice(WriteRawMessageToDeviceRequest request, ServerCallContext context)
        {
            bool result = antRadio.WriteRawMessageToDevice((byte)request.MsgId, [.. request.MsgData]);
            return Task.FromResult(new BoolValue { Value = result });
        }

        private static AntResponseReply FromAntResponse(AntResponse antResponse)
        {
            return new AntResponseReply
            {
                ChannelNumber = antResponse.ChannelNumber,
                ResponseId = (uint)antResponse.ResponseId,
                Payload = ByteString.CopyFrom(antResponse.Payload),
                Rssi = antResponse.Rssi,
                ThresholdConfigurationValue = antResponse.ThresholdConfigurationValue,
                Timestamp = antResponse.Timestamp
            };
        }
    }
}
