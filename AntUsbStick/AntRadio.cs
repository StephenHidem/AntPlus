using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntUsbStick
{
    /// <summary>This class implements the IAntRadio interface.</summary>
    public class AntRadio : IAntRadio
    {
        private readonly ANT_Device antDevice;

        /// <inheritdoc/>
        public event EventHandler<IAntResponse> RadioResponse;

        /// <inheritdoc/>
        public ushort DeviceUSBPID => antDevice.getDeviceUSBPID();

        /// <inheritdoc/>
        public ushort DeviceUSBVID => antDevice.getDeviceUSBVID();

        /// <inheritdoc/>
        public int NumChannels => antDevice.getNumChannels();

        /// <inheritdoc/>
        public FramerType OpenedFrameType => (FramerType)antDevice.getOpenedFrameType();

        /// <inheritdoc/>
        public PortType OpenedPortType => (PortType)antDevice.getOpenedPortType();

        /// <inheritdoc/>
        public uint OpenedUSBBaudRate => antDevice.getOpenedUSBBaudRate();

        /// <inheritdoc/>
        public int OpenedUSBDeviceNum => antDevice.getOpenedUSBDeviceNum();

        /// <inheritdoc/>
        public uint SerialNumber => antDevice.getSerialNumber();

        /// <inheritdoc/>
        public void CancelTransfers(int cancelWaitTime)
        {
            antDevice.cancelTransfers(cancelWaitTime);
        }

        /// <summary>Initializes a new instance of the <see cref="AntRadio" /> class.</summary>
        public AntRadio()
        {
            antDevice = new ANT_Device();
            antDevice.deviceResponse += AntDevice_deviceResponse;
        }

        private void AntDevice_deviceResponse(ANT_Response response)
        {
            RadioResponse?.Invoke(this, new AntResponse(response));
        }

        /// <inheritdoc/>
        public void ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields)
        {
            antDevice.configureAdvancedBursting(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields);
        }

        /// <inheritdoc/>
        public bool ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, uint responseWaitTime)
        {
            return antDevice.configureAdvancedBursting(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, responseWaitTime);
        }

        /// <inheritdoc/>
        public void ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount)
        {
            antDevice.configureAdvancedBursting_ext(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, stallCount, retryCount);
        }

        /// <inheritdoc/>
        public bool ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount, uint responseWaitTime)
        {
            return antDevice.configureAdvancedBursting_ext(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, stallCount, retryCount, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool ConfigureAdvancedBurstSplitting(bool splitBursts)
        {
            return antDevice.configureAdvancedBurstSplitting(splitBursts);
        }

        /// <inheritdoc/>
        public void ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time)
        {
            antDevice.configureEventBuffer((ANT_ReferenceLibrary.EventBufferConfig)config, size, time);
        }

        /// <inheritdoc/>
        public bool ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time, uint responseWaitTime)
        {
            return antDevice.configureEventBuffer((ANT_ReferenceLibrary.EventBufferConfig)config, size, time, responseWaitTime);
        }

        /// <inheritdoc/>
        public void ConfigureEventFilter(ushort eventFilter)
        {
            antDevice.configureEventFilter(eventFilter);
        }

        /// <inheritdoc/>
        public bool ConfigureEventFilter(ushort eventFilter, uint responseWaitTime)
        {
            return antDevice.configureEventFilter(eventFilter, responseWaitTime);
        }

        /// <inheritdoc/>
        public void ConfigureHighDutySearch(bool enable, byte suppressionCycles)
        {
            antDevice.configureHighDutySearch(enable, suppressionCycles);
        }

        /// <inheritdoc/>
        public bool ConfigureHighDutySearch(bool enable, byte suppressionCycles, uint responseWaitTime)
        {
            return antDevice.configureHighDutySearch(enable, suppressionCycles, responseWaitTime);
        }

        /// <inheritdoc/>
        public void ConfigureUserNvm(ushort address, byte[] data, byte size)
        {
            antDevice.configureUserNvm(address, data, size);
        }

        /// <inheritdoc/>
        public bool ConfigureUserNvm(ushort address, byte[] data, byte size, uint responseWaitTime)
        {
            return antDevice.configureUserNvm(address, data, size, responseWaitTime);
        }

        /// <inheritdoc/>
        public void CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData)
        {
            antDevice.cryptoKeyNVMOp((ANT_ReferenceLibrary.EncryptionNVMOp)memoryOperation, nonVolatileKeyIndex, operationData);
        }

        /// <inheritdoc/>
        public bool CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData, uint responseWaitTime)
        {
            return antDevice.cryptoKeyNVMOp((ANT_ReferenceLibrary.EncryptionNVMOp)memoryOperation, nonVolatileKeyIndex, operationData, responseWaitTime);
        }

        /// <inheritdoc/>
        public void CrystalEnable()
        {
            antDevice.crystalEnable();
        }

        /// <inheritdoc/>
        public bool CrystalEnable(uint responseWaitTime)
        {
            return antDevice.crystalEnable(responseWaitTime);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            antDevice.Dispose();
        }

        /// <inheritdoc/>
        public void EnableLED(bool isEnabled)
        {
            antDevice.EnableLED(isEnabled);
        }

        /// <inheritdoc/>
        public bool EnableLED(bool isEnabled, uint responseWaitTime)
        {
            return antDevice.EnableLED(isEnabled, responseWaitTime);
        }

        /// <inheritdoc/>
        public void EnableRxExtendedMessages(bool isEnabled)
        {
            antDevice.enableRxExtendedMessages(isEnabled);
        }

        /// <inheritdoc/>
        public bool EnableRxExtendedMessages(bool isEnabled, uint responseWaitTime)
        {
            return antDevice.enableRxExtendedMessages(isEnabled, responseWaitTime);
        }

        /// <inheritdoc/>
        public void FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv)
        {
            antDevice.fitAdjustPairingSettings(searchLv, pairLv, trackLv);
        }

        /// <inheritdoc/>
        public bool FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv, uint responseWaitTime)
        {
            return antDevice.fitAdjustPairingSettings(searchLv, pairLv, trackLv, responseWaitTime);
        }

        /// <inheritdoc/>
        public void FitSetFEState(byte feState)
        {
            antDevice.fitSetFEState(feState);
        }

        /// <inheritdoc/>
        public bool FitSetFEState(byte feState, uint responseWaitTime)
        {
            return antDevice.fitSetFEState(feState, responseWaitTime);
        }

        /// <inheritdoc/>
        public IAntChannel GetChannel(int num)
        {
            return new AntChannel(antDevice.getChannel(num));
        }

        /// <inheritdoc/>
        public IDeviceCapabilities GetDeviceCapabilities()
        {
            return (IDeviceCapabilities)antDevice.getDeviceCapabilities();
        }

        /// <inheritdoc/>
        public IDeviceCapabilities GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime)
        {
            return (IDeviceCapabilities)antDevice.getDeviceCapabilities(forceNewCopy, responseWaitTime);
        }

        /// <inheritdoc/>
        public IDeviceCapabilities GetDeviceCapabilities(uint responseWaitTime)
        {
            return (IDeviceCapabilities)antDevice.getDeviceCapabilities(responseWaitTime);
        }

        /// <inheritdoc/>
        public DeviceInfo GetDeviceUSBInfo()
        {
            ANT_DeviceInfo di = antDevice.getDeviceUSBInfo();
            return new DeviceInfo(di.productDescription, di.serialString);
        }

        /// <inheritdoc/>
        public DeviceInfo GetDeviceUSBInfo(byte deviceNum)
        {
            ANT_DeviceInfo di = antDevice.getDeviceUSBInfo(deviceNum);
            return new DeviceInfo(di.productDescription, di.serialString);
        }

        /// <inheritdoc/>
        public void LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex)
        {
            antDevice.loadCryptoKeyFromNVM(nonVolatileKeyIndex, volatileKeyIndex);
        }

        /// <inheritdoc/>
        public bool LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex, uint responseWaitTime)
        {
            return antDevice.loadCryptoKeyFromNVM(nonVolatileKeyIndex, volatileKeyIndex, responseWaitTime);
        }

        /// <inheritdoc/>
        public void OpenRxScanMode()
        {
            antDevice.openRxScanMode();
        }

        /// <inheritdoc/>
        public bool OpenRxScanMode(uint responseWaitTime)
        {
            return antDevice.openRxScanMode(responseWaitTime);
        }

        /// <inheritdoc/>
        public IAntResponse ReadUserNvm(ushort address, byte size)
        {
            return new AntResponse(antDevice.readUserNvm(address, size));
        }

        /// <inheritdoc/>
        public IAntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime)
        {
            return new AntResponse(antDevice.readUserNvm(address, size, responseWaitTime));
        }

        /// <inheritdoc/>
        public void RequestMessage(byte channelNum, RequestMessageID messageID)
        {
            antDevice.requestMessage(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID);
        }

        /// <inheritdoc/>
        public void RequestMessage(RequestMessageID messageID)
        {
            antDevice.requestMessage((ANT_ReferenceLibrary.RequestMessageID)messageID);
        }

        /// <inheritdoc/>
        public IAntResponse RequestMessageAndResponse(byte channelNum, RequestMessageID messageID, uint responseWaitTime)
        {
            return new AntResponse(antDevice.requestMessageAndResponse(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID, responseWaitTime));
        }

        /// <inheritdoc/>
        public IAntResponse RequestMessageAndResponse(RequestMessageID messageID, uint responseWaitTime)
        {
            return new AntResponse(antDevice.requestMessageAndResponse((ANT_ReferenceLibrary.RequestMessageID)messageID, responseWaitTime));
        }

        /// <inheritdoc/>
        public void ResetSystem()
        {
            antDevice.ResetSystem();
        }

        /// <inheritdoc/>
        public bool ResetSystem(uint responseWaitTime)
        {
            return antDevice.ResetSystem(responseWaitTime);
        }

        /// <inheritdoc/>
        public void ResetUSB()
        {
            antDevice.ResetUSB();
        }

        /// <inheritdoc/>
        public void SetCryptoID(byte[] encryptionID)
        {
            antDevice.setCryptoID(encryptionID);
        }

        /// <inheritdoc/>
        public bool SetCryptoID(byte[] encryptionID, uint responseWaitTime)
        {
            return antDevice.setCryptoID(encryptionID, responseWaitTime);
        }

        /// <inheritdoc/>
        public void SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData)
        {
            antDevice.setCryptoInfo((ANT_ReferenceLibrary.EncryptionInfo)encryptionParameter, parameterData);
        }

        /// <inheritdoc/>
        public bool SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData, uint responseWaitTime)
        {
            return antDevice.setCryptoInfo((ANT_ReferenceLibrary.EncryptionInfo)encryptionParameter, parameterData, responseWaitTime);
        }

        /// <inheritdoc/>
        public void SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey)
        {
            antDevice.setCryptoKey(volatileKeyIndex, encryptionKey);
        }

        /// <inheritdoc/>
        public bool SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey, uint responseWaitTime)
        {
            return antDevice.setCryptoKey(volatileKeyIndex, encryptionKey, responseWaitTime);
        }

        /// <inheritdoc/>
        public void SetCryptoRNGSeed(byte[] cryptoRNGSeed)
        {
            antDevice.setCryptoRNGSeed(cryptoRNGSeed);
        }

        /// <inheritdoc/>
        public bool SetCryptoRNGSeed(byte[] cryptoRNGSeed, uint responseWaitTime)
        {
            return antDevice.setCryptoRNGSeed(cryptoRNGSeed, responseWaitTime);
        }

        /// <inheritdoc/>
        public void SetCryptoUserInfo(byte[] userInfoString)
        {
            antDevice.setCryptoUserInfo(userInfoString);
        }

        /// <inheritdoc/>
        public bool SetCryptoUserInfo(byte[] userInfoString, uint responseWaitTime)
        {
            return antDevice.setCryptoUserInfo(userInfoString, responseWaitTime);
        }

        /// <inheritdoc/>
        public void SetLibConfig(LibConfigFlags libConfigFlags)
        {
            antDevice.setLibConfig((ANT_ReferenceLibrary.LibConfigFlags)libConfigFlags);
        }

        /// <inheritdoc/>
        public bool SetLibConfig(LibConfigFlags libConfigFlags, uint responseWaitTime)
        {
            return antDevice.setLibConfig((ANT_ReferenceLibrary.LibConfigFlags)libConfigFlags, responseWaitTime);
        }

        /// <inheritdoc/>
        public void SetNetworkKey(byte netNumber, byte[] networkKey)
        {
            antDevice.setNetworkKey(netNumber, networkKey);
        }

        /// <inheritdoc/>
        public bool SetNetworkKey(byte netNumber, byte[] networkKey, uint responseWaitTime)
        {
            return antDevice.setNetworkKey(netNumber, networkKey, responseWaitTime);
        }

        /// <inheritdoc/>
        public void SetTransmitPowerForAllChannels(TransmitPower transmitPower)
        {
            antDevice.setTransmitPowerForAllChannels((ANT_ReferenceLibrary.TransmitPower)transmitPower);
        }

        /// <inheritdoc/>
        public bool SetTransmitPowerForAllChannels(TransmitPower transmitPower, uint responseWaitTime)
        {
            return antDevice.setTransmitPowerForAllChannels((ANT_ReferenceLibrary.TransmitPower)transmitPower, responseWaitTime);
        }

        /// <inheritdoc/>
        public void StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey)
        {
            antDevice.storeCryptoKeyToNVM(nonVolatileKeyIndex, encryptionKey);
        }

        /// <inheritdoc/>
        public bool StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey, uint responseWaitTime)
        {
            return antDevice.storeCryptoKeyToNVM(nonVolatileKeyIndex, encryptionKey, responseWaitTime);
        }

        /// <inheritdoc/>
        public bool WriteRawMessageToDevice(byte msgID, byte[] msgData)
        {
            return antDevice.writeRawMessageToDevice(msgID, msgData);
        }
    }
}
