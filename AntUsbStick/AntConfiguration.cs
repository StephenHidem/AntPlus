using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IAntConfiguration
    {
        /// <inheritdoc/>
        public void ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields) => antDevice.configureAdvancedBursting(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields);

        /// <inheritdoc/>
        public bool ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, uint responseWaitTime) => antDevice.configureAdvancedBursting(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, responseWaitTime);

        /// <inheritdoc/>
        public void ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount) => antDevice.configureAdvancedBursting_ext(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, stallCount, retryCount);

        /// <inheritdoc/>
        public bool ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount, uint responseWaitTime) => antDevice.configureAdvancedBursting_ext(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, stallCount, retryCount, responseWaitTime);

        /// <inheritdoc/>
        public bool ConfigureAdvancedBurstSplitting(bool splitBursts) => antDevice.configureAdvancedBurstSplitting(splitBursts);

        /// <inheritdoc/>
        public void ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time) => antDevice.configureEventBuffer((ANT_ReferenceLibrary.EventBufferConfig)config, size, time);

        /// <inheritdoc/>
        public bool ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time, uint responseWaitTime) => antDevice.configureEventBuffer((ANT_ReferenceLibrary.EventBufferConfig)config, size, time, responseWaitTime);

        /// <inheritdoc/>
        public void ConfigureEventFilter(ushort eventFilter) => antDevice.configureEventFilter(eventFilter);

        /// <inheritdoc/>
        public bool ConfigureEventFilter(ushort eventFilter, uint responseWaitTime) => antDevice.configureEventFilter(eventFilter, responseWaitTime);

        /// <inheritdoc/>
        public void ConfigureHighDutySearch(bool enable, byte suppressionCycles) => antDevice.configureHighDutySearch(enable, suppressionCycles);

        /// <inheritdoc/>
        public bool ConfigureHighDutySearch(bool enable, byte suppressionCycles, uint responseWaitTime) => antDevice.configureHighDutySearch(enable, suppressionCycles, responseWaitTime);

        /// <inheritdoc/>
        public void ConfigureUserNvm(ushort address, byte[] data, byte size) => antDevice.configureUserNvm(address, data, size);

        /// <inheritdoc/>
        public bool ConfigureUserNvm(ushort address, byte[] data, byte size, uint responseWaitTime) => antDevice.configureUserNvm(address, data, size, responseWaitTime);

        /// <inheritdoc/>
        public void CrystalEnable() => antDevice.crystalEnable();

        /// <inheritdoc/>
        public bool CrystalEnable(uint responseWaitTime) => antDevice.crystalEnable(responseWaitTime);

        /// <inheritdoc/>
        public void EnableLED(bool isEnabled) => antDevice.EnableLED(isEnabled);

        /// <inheritdoc/>
        public bool EnableLED(bool isEnabled, uint responseWaitTime) => antDevice.EnableLED(isEnabled, responseWaitTime);

        /// <inheritdoc/>
        public void EnableRxExtendedMessages(bool isEnabled) => antDevice.enableRxExtendedMessages(isEnabled);

        /// <inheritdoc/>
        public bool EnableRxExtendedMessages(bool isEnabled, uint responseWaitTime) => antDevice.enableRxExtendedMessages(isEnabled, responseWaitTime);

        /// <inheritdoc/>
        public void SetLibConfig(LibConfigFlags libConfigFlags) => antDevice.setLibConfig((ANT_ReferenceLibrary.LibConfigFlags)libConfigFlags);

        /// <inheritdoc/>
        public bool SetLibConfig(LibConfigFlags libConfigFlags, uint responseWaitTime) => antDevice.setLibConfig((ANT_ReferenceLibrary.LibConfigFlags)libConfigFlags, responseWaitTime);

        /// <inheritdoc/>
        public void SetNetworkKey(byte netNumber, byte[] networkKey) => antDevice.setNetworkKey(netNumber, networkKey);

        /// <inheritdoc/>
        public bool SetNetworkKey(byte netNumber, byte[] networkKey, uint responseWaitTime) => antDevice.setNetworkKey(netNumber, networkKey, responseWaitTime);

        /// <inheritdoc/>
        public void SetTransmitPowerForAllChannels(TransmitPower transmitPower) => antDevice.setTransmitPowerForAllChannels((ANT_ReferenceLibrary.TransmitPower)transmitPower);

        /// <inheritdoc/>
        public bool SetTransmitPowerForAllChannels(TransmitPower transmitPower, uint responseWaitTime) => antDevice.setTransmitPowerForAllChannels((ANT_ReferenceLibrary.TransmitPower)transmitPower, responseWaitTime);

    }
}
