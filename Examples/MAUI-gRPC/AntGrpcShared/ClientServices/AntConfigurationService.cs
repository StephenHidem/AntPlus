using AntConfigurationGrpcService;
using Google.Protobuf.WellKnownTypes;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcShared.ClientServices
{
    /// <summary>
    /// Service for interacting with ANT radio configuration using gRPC.
    /// </summary>
    public partial class AntRadioService : IAntConfiguration
    {
        private gRPCAntConfiguration.gRPCAntConfigurationClient? _config;

        /// <inheritdoc/>
        public bool ConfigureAdvancedBursting(bool enable, byte maxPacketLength, SmallEarthTech.AntRadioInterface.AdvancedBurstConfigFlags requiredFields, SmallEarthTech.AntRadioInterface.AdvancedBurstConfigFlags optionalFields, uint responseWaitTime = 0)
        {
            return _config!.ConfigureAdvancedBursting(new ConfigureAdvancedBurstingRequest
            {
                Enable = enable,
                MaxPacketLength = maxPacketLength,
                RequiredFields = (AntConfigurationGrpcService.AdvancedBurstConfigFlags)requiredFields,
                OptionalFields = (AntConfigurationGrpcService.AdvancedBurstConfigFlags)optionalFields,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, SmallEarthTech.AntRadioInterface.AdvancedBurstConfigFlags requiredFields, SmallEarthTech.AntRadioInterface.AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount, uint responseWaitTime = 0)
        {
            return _config!.ConfigureAdvancedBurstingExt(new ConfigureAdvancedBurstingExtRequest
            {
                Enable = enable,
                MaxPacketLength = maxPacketLength,
                RequiredFields = (AntConfigurationGrpcService.AdvancedBurstConfigFlags)requiredFields,
                OptionalFields = (AntConfigurationGrpcService.AdvancedBurstConfigFlags)optionalFields,
                StallCount = stallCount,
                RetryCount = retryCount,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool ConfigureAdvancedBurstSplitting(bool splitBursts)
        {
            return _config!.ConfigureAdvancedBurstSplitting(new BoolValue
            {
                Value = splitBursts
            }).Value;
        }

        /// <inheritdoc/>
        public bool ConfigureEventBuffer(SmallEarthTech.AntRadioInterface.EventBufferConfig config, ushort size, ushort time, uint responseWaitTime = 0)
        {
            return _config!.ConfigureEventBuffer(new ConfigureEventBufferRequest
            {
                Config = (AntConfigurationGrpcService.EventBufferConfig)config,
                Size = size,
                Time = time,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool ConfigureEventFilter(ushort eventFilter, uint responseWaitTime = 0)
        {
            return _config!.ConfigureEventFilter(new ConfigureEventFilterRequest
            {
                EventFilter = eventFilter,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool ConfigureHighDutySearch(bool enable, byte suppressionCycles, uint responseWaitTime = 0)
        {
            return _config!.ConfigureHighDutySearch(new ConfigureHighDutySearchRequest
            {
                Enable = enable,
                SuppressionCycles = suppressionCycles,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool ConfigureUserNvm(ushort address, byte[] data, byte size, uint responseWaitTime = 0)
        {
            return _config!.ConfigureUserNvm(new ConfigureUserNvmRequest
            {
                Address = address,
                Data = Google.Protobuf.ByteString.CopyFrom(data),
                Size = size,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool CrystalEnable(uint responseWaitTime = 0)
        {
            return _config!.CrystalEnable(new CrystalEnableRequest
            {
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool EnableLED(bool isEnabled, uint responseWaitTime = 0)
        {
            return _config!.EnableLED(new EnableLEDRequest
            {
                IsEnabled = isEnabled,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool EnableRxExtendedMessages(bool isEnabled, uint responseWaitTime = 0)
        {
            return _config!.EnableRxExtendedMessages(new EnableRxExtendedMessagesRequest
            {
                IsEnabled = isEnabled,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetLibConfig(LibConfigFlags libConfigFlags, uint responseWaitTime = 0)
        {
            return _config!.SetLibConfig(new SetLibConfigRequest
            {
                LibConfigFlags = (int)libConfigFlags,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetNetworkKey(byte netNumber, byte[] networkKey, uint responseWaitTime = 0)
        {
            return _config!.SetNetworkKey(new SetNetworkKeyRequest
            {
                NetNumber = netNumber,
                NetworkKey = Google.Protobuf.ByteString.CopyFrom(networkKey),
                ResponseWaitTime = responseWaitTime
            }).Value;
        }

        /// <inheritdoc/>
        public bool SetTransmitPowerForAllChannels(SmallEarthTech.AntRadioInterface.TransmitPower transmitPower, uint responseWaitTime = 0)
        {
            return _config!.SetTransmitPowerForAllChannels(new SetTransmitPowerForAllChannelsRequest
            {
                TransmitPower = (AntConfigurationGrpcService.TransmitPower)transmitPower,
                ResponseWaitTime = responseWaitTime
            }).Value;
        }
    }
}
