using AntConfigurationGrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SmallEarthTech.AntRadioInterface;

namespace AntGrpcService.Services
{
    public class AntConfigurationService : gRPCAntConfiguration.gRPCAntConfigurationBase
    {
        private readonly IAntConfiguration _config;

        public AntConfigurationService(IAntRadio antRadio)
        {
            _config = (IAntConfiguration)antRadio;
        }

        // implement gRCPAntConfiguration.gRPCAntConfigurationBase methods
        override public Task<BoolValue> ConfigureAdvancedBursting(ConfigureAdvancedBurstingRequest request, ServerCallContext context)
        {
            bool result = _config.ConfigureAdvancedBursting(request.Enable, (byte)request.MaxPacketLength, (SmallEarthTech.AntRadioInterface.AdvancedBurstConfigFlags)request.RequiredFields, (SmallEarthTech.AntRadioInterface.AdvancedBurstConfigFlags)request.OptionalFields, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> ConfigureAdvancedBurstingExt(ConfigureAdvancedBurstingExtRequest request, ServerCallContext context)
        {
            bool result = _config.ConfigureAdvancedBursting_ext(request.Enable, (byte)request.MaxPacketLength, (SmallEarthTech.AntRadioInterface.AdvancedBurstConfigFlags)request.RequiredFields, (SmallEarthTech.AntRadioInterface.AdvancedBurstConfigFlags)request.OptionalFields, (ushort)request.StallCount, (byte)request.RetryCount, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> ConfigureAdvancedBurstSplitting(BoolValue request, ServerCallContext context)
        {
            bool result = _config.ConfigureAdvancedBurstSplitting(request.Value);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> ConfigureEventBuffer(ConfigureEventBufferRequest request, ServerCallContext context)
        {
            bool result = _config.ConfigureEventBuffer((SmallEarthTech.AntRadioInterface.EventBufferConfig)request.Config, (ushort)request.Size, (ushort)request.Time, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> ConfigureEventFilter(ConfigureEventFilterRequest request, ServerCallContext context)
        {
            bool result = _config.ConfigureEventFilter((ushort)request.EventFilter, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> ConfigureHighDutySearch(ConfigureHighDutySearchRequest request, ServerCallContext context)
        {
            bool result = _config.ConfigureHighDutySearch(request.Enable, (byte)request.SuppressionCycles, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> ConfigureUserNvm(ConfigureUserNvmRequest request, ServerCallContext context)
        {
            bool result = _config.ConfigureUserNvm((ushort)request.Address, request.Data.ToByteArray(), (byte)request.Size, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> CrystalEnable(CrystalEnableRequest request, ServerCallContext context)
        {
            bool result = _config.CrystalEnable(request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> EnableLED(EnableLEDRequest request, ServerCallContext context)
        {
            bool result = _config.EnableLED(request.IsEnabled, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> EnableRxExtendedMessages(EnableRxExtendedMessagesRequest request, ServerCallContext context)
        {
            bool result = _config.EnableRxExtendedMessages(request.IsEnabled, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> SetLibConfig(SetLibConfigRequest request, ServerCallContext context)
        {
            bool result = _config.SetLibConfig((LibConfigFlags)request.LibConfigFlags, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> SetNetworkKey(SetNetworkKeyRequest request, ServerCallContext context)
        {
            bool result = _config.SetNetworkKey((byte)request.NetNumber, request.NetworkKey.ToByteArray(), request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }

        override public Task<BoolValue> SetTransmitPowerForAllChannels(SetTransmitPowerForAllChannelsRequest request, ServerCallContext context)
        {
            bool result = _config.SetTransmitPowerForAllChannels((SmallEarthTech.AntRadioInterface.TransmitPower)request.TransmitPower, request.ResponseWaitTime);
            return Task.FromResult(new BoolValue { Value = result });
        }
    }
}
