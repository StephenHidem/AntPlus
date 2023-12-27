using AntRadioGrpcService;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;
using System.Text;

namespace AntGrpcServer.Services
{
    public class AntRadioService(ILogger<AntRadioService> logger, IAntRadio antRadio) : gRPCAntRadio.gRPCAntRadioBase
    {
        private readonly ILogger<AntRadioService> _logger = logger;
        private readonly IAntRadio _antRadio = antRadio;

        public static IAntChannel[] AntChannels { get; private set; } = [];

        public override Task<PropertiesReply> GetProperties(PropertiesRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{nameof(GetProperties)}");
            SmallEarthTech.AntUsbStick.AntRadio usbAntRadio = (SmallEarthTech.AntUsbStick.AntRadio)_antRadio;
            AntResponse rsp = usbAntRadio.RequestMessageAndResponse(RequestMessageID.Version, 500);
            return Task.FromResult(new PropertiesReply
            {
                SerialString = usbAntRadio.GetSerialString(),
                HostVersion = Encoding.Default.GetString(rsp.Payload).TrimEnd('\0'),
                ProductDescription = usbAntRadio.GetProductDescription()
            });
        }

        public override async Task<InitScanModeReply> InitializeContinuousScanMode(InitScanModeRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{nameof(InitializeContinuousScanMode)}");
            AntChannels = await _antRadio.InitializeContinuousScanMode();

            return new InitScanModeReply
            {
                NumChannels = AntChannels.Length
            };
        }

        public override Task<GetChannelReply> GetChannel(GetChannelRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"{nameof(GetChannel)}");
            _ = _antRadio.GetChannel(request.ChannelNumber);

            return Task.FromResult(new GetChannelReply());
        }

        public override Task<GetDeviceCapabilitiesReply> GetDeviceCapabilities(GetDeviceCapabilitiesRequest request, ServerCallContext context)
        {
            DeviceCapabilities capabilities;
            if (request.HasForceCopy && request.HasWaitResponseTime)
            {
                capabilities = _antRadio.GetDeviceCapabilities(request.ForceCopy, request.WaitResponseTime);
            }
            else if (request.HasWaitResponseTime)
            {
                capabilities = _antRadio.GetDeviceCapabilities(request.WaitResponseTime);
            }
            else
            {
                capabilities = _antRadio.GetDeviceCapabilities();
            }

            return Task.FromResult(new GetDeviceCapabilitiesReply
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
            });
        }
    }
}
