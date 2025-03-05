using AntRadioGrpcService;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;
using System.Text;

namespace AntGrpcService.Services
{
    public class AntRadioService(ILogger<AntRadioService> logger, IAntRadio antRadio) : gRPCAntRadio.gRPCAntRadioBase
    {
        private readonly ILogger<AntRadioService> _logger = logger;
        private readonly IAntRadio _antRadio = antRadio;
        private TaskCompletionSource<AntResponse>? _response;

        public static IAntChannel[] AntChannels { get; private set; } = [];

        public override Task<PropertiesReply> GetProperties(Empty request, ServerCallContext context)
        {
            _logger.LogDebug(nameof(GetProperties));
            SmallEarthTech.AntUsbStick.AntRadio usbAntRadio = (SmallEarthTech.AntUsbStick.AntRadio)_antRadio;
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
            _logger.LogDebug(nameof(InitializeContinuousScanMode));
            AntChannels = await _antRadio.InitializeContinuousScanMode();

            return new InitScanModeReply
            {
                NumChannels = AntChannels.Length
            };
        }

        public override Task<Empty> CancelTransfers(CancelTransfersRequest request, ServerCallContext context)
        {
            _antRadio.CancelTransfers(request.WaitTime);
            return Task.FromResult(new Empty());
        }

        public override Task<GetChannelReply> GetChannel(GetChannelRequest request, ServerCallContext context)
        {
            _logger.LogDebug("GetChannel: Channel = {ChannelNumber}", request.ChannelNumber);
            _ = _antRadio.GetChannel(request.ChannelNumber);

            return Task.FromResult(new GetChannelReply());
        }

        public override async Task<GetDeviceCapabilitiesReply> GetDeviceCapabilities(GetDeviceCapabilitiesRequest request, ServerCallContext context)
        {
            DeviceCapabilities capabilities = await _antRadio.GetDeviceCapabilities(request.ForceCopy, request.WaitResponseTime);
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
            AntResponse result = _antRadio.ReadUserNvm((ushort)request.Address, (byte)request.Size, request.WaitResponseTime);
            return Task.FromResult(FromAntResponse(result));
        }

        public override Task<AntResponseReply> RequestMessageAndResponse(RequestMessageAndResponseRequest request, ServerCallContext context)
        {
            AntResponse result = _antRadio.RequestMessageAndResponse((SmallEarthTech.AntRadioInterface.RequestMessageID)request.MsgId, request.WaitResponseTime, (byte)request.ChannelNumber);
            return Task.FromResult(FromAntResponse(result));
        }

        public override Task<BoolValue> WriteRawMessageToDevice(WriteRawMessageToDeviceRequest request, ServerCallContext context)
        {
            bool result = _antRadio.WriteRawMessageToDevice((byte)request.MsgId, [.. request.MsgData]);
            return Task.FromResult(new BoolValue { Value = result });
        }

        public override async Task Subscribe(Empty request, IServerStreamWriter<AntResponseReply> responseStream, ServerCallContext context)
        {
            _logger.LogDebug("Subscribe called.");
            _antRadio.RadioResponse += AntRadio_RadioResponse;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                _response = new TaskCompletionSource<AntResponse>();
                AntResponse radioResponse = await _response.Task;
                try
                {
                    await responseStream.WriteAsync(FromAntResponse(radioResponse));
                }
                catch (InvalidOperationException e)
                {
                    _logger.LogWarning("Subscription closed. {Msg}", e.Message);
                    break;
                }
            }
            _antRadio.RadioResponse -= AntRadio_RadioResponse;
        }

        private void AntRadio_RadioResponse(object? sender, AntResponse e)
        {
            if (sender != null && _response != null && !_response.TrySetResult(e))
            {
                ((IAntRadio)sender).RadioResponse -= AntRadio_RadioResponse;
            }
        }

        private static AntResponseReply FromAntResponse(AntResponse antResponse)
        {
            return new AntResponseReply
            {
                ChannelNumber = antResponse.ChannelNumber,
                ResponseId = (uint)antResponse.ResponseId,
                ChannelId = antResponse.ChannelId!.Id,
                Payload = ByteString.CopyFrom(antResponse.Payload),
                Rssi = antResponse.Rssi,
                ThresholdConfigurationValue = antResponse.ThresholdConfigurationValue,
                Timestamp = antResponse.Timestamp
            };
        }
    }
}
