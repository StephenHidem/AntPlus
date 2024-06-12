using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IAntConfiguration
    {

        /// <inheritdoc/>
        public bool ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, uint responseWaitTime = 0) => _antDevice.configureAdvancedBursting(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, responseWaitTime);

        /// <inheritdoc/>
        public bool ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount, uint responseWaitTime = 0) => _antDevice.configureAdvancedBursting_ext(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, stallCount, retryCount, responseWaitTime);

        /// <inheritdoc/>
        public bool ConfigureAdvancedBurstSplitting(bool splitBursts) => _antDevice.configureAdvancedBurstSplitting(splitBursts);

        /// <inheritdoc/>
        public bool ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time, uint responseWaitTime = 0) => _antDevice.configureEventBuffer((ANT_ReferenceLibrary.EventBufferConfig)config, size, time, responseWaitTime);

        /// <inheritdoc/>
        public bool ConfigureEventFilter(ushort eventFilter, uint responseWaitTime = 0) => _antDevice.configureEventFilter(eventFilter, responseWaitTime);

        /// <inheritdoc/>
        public bool ConfigureHighDutySearch(bool enable, byte suppressionCycles, uint responseWaitTime = 0) => _antDevice.configureHighDutySearch(enable, suppressionCycles, responseWaitTime);

        /// <inheritdoc/>
        public bool ConfigureUserNvm(ushort address, byte[] data, byte size, uint responseWaitTime = 0) => _antDevice.configureUserNvm(address, data, size, responseWaitTime);

        /// <inheritdoc/>
        public bool CrystalEnable(uint responseWaitTime = 0) => _antDevice.crystalEnable(responseWaitTime);

        /// <inheritdoc/>
        public bool EnableLED(bool isEnabled, uint responseWaitTime = 0) => _antDevice.EnableLED(isEnabled, responseWaitTime);

        /// <inheritdoc/>
        public bool EnableRxExtendedMessages(bool isEnabled, uint responseWaitTime = 0) => _antDevice.enableRxExtendedMessages(isEnabled, responseWaitTime);

        /// <inheritdoc/>
        public bool SetLibConfig(LibConfigFlags libConfigFlags, uint responseWaitTime = 0) => _antDevice.setLibConfig((ANT_ReferenceLibrary.LibConfigFlags)libConfigFlags, responseWaitTime);

        /// <inheritdoc/>
        public bool SetNetworkKey(byte netNumber, byte[] networkKey, uint responseWaitTime = 0) => _antDevice.setNetworkKey(netNumber, networkKey, responseWaitTime);

        /// <inheritdoc/>
        public bool SetTransmitPowerForAllChannels(TransmitPower transmitPower, uint responseWaitTime = 0) => _antDevice.setTransmitPowerForAllChannels((ANT_ReferenceLibrary.TransmitPower)transmitPower, responseWaitTime);

    }
}
