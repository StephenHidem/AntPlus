using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntUsbStick
{
    public class AntRadio : IAntRadio
    {
        private readonly ANT_Device antDevice;

        public event EventHandler<IAntResponse> RadioResponse;

        public ushort DeviceUSBPID => antDevice.getDeviceUSBPID();

        public ushort DeviceUSBVID => antDevice.getDeviceUSBVID();

        public int NumChannels => antDevice.getNumChannels();

        public FramerType OpenedFrameType => (FramerType)antDevice.getOpenedFrameType();

        public PortType OpenedPortType => (PortType)antDevice.getOpenedPortType();

        public uint OpenedUSBBaudRate => antDevice.getOpenedUSBBaudRate();

        public int OpenedUSBDeviceNum => antDevice.getOpenedUSBDeviceNum();

        public uint SerialNumber => antDevice.getSerialNumber();

        public void CancelTransfers(int cancelWaitTime)
        {
            antDevice.cancelTransfers(cancelWaitTime);
        }

        public AntRadio()
        {
            antDevice = new ANT_Device();
            antDevice.deviceResponse += AntDevice_deviceResponse;
        }

        private void AntDevice_deviceResponse(ANT_Response response)
        {
            RadioResponse?.Invoke(this, new AntResponse(response));
        }

        public void ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields)
        {
            antDevice.configureAdvancedBursting(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields);
        }

        public bool ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, uint responseWaitTime)
        {
            return antDevice.configureAdvancedBursting(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, responseWaitTime);
        }

        public void ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount)
        {
            antDevice.configureAdvancedBursting_ext(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, stallCount, retryCount);
        }

        public bool ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount, uint responseWaitTime)
        {
            return antDevice.configureAdvancedBursting_ext(enable, maxPacketLength, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)requiredFields, (ANT_ReferenceLibrary.AdvancedBurstConfigFlags)optionalFields, stallCount, retryCount, responseWaitTime);
        }

        public bool ConfigureAdvancedBurstSplitting(bool splitBursts)
        {
            return antDevice.configureAdvancedBurstSplitting(splitBursts);
        }

        public void ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time)
        {
            antDevice.configureEventBuffer((ANT_ReferenceLibrary.EventBufferConfig)config, size, time);
        }

        public bool ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time, uint responseWaitTime)
        {
            return antDevice.configureEventBuffer((ANT_ReferenceLibrary.EventBufferConfig)config, size, time, responseWaitTime);
        }

        public void ConfigureEventFilter(ushort eventFilter)
        {
            antDevice.configureEventFilter(eventFilter);
        }

        public bool ConfigureEventFilter(ushort eventFilter, uint responseWaitTime)
        {
            return antDevice.configureEventFilter(eventFilter, responseWaitTime);
        }

        public void ConfigureHighDutySearch(bool enable, byte suppressionCycles)
        {
            antDevice.configureHighDutySearch(enable, suppressionCycles);
        }

        public bool ConfigureHighDutySearch(bool enable, byte suppressionCycles, uint responseWaitTime)
        {
            return antDevice.configureHighDutySearch(enable, suppressionCycles, responseWaitTime);
        }

        public void ConfigureUserNvm(ushort address, byte[] data, byte size)
        {
            antDevice.configureUserNvm(address, data, size);
        }

        public bool ConfigureUserNvm(ushort address, byte[] data, byte size, uint responseWaitTime)
        {
            return antDevice.configureUserNvm(address, data, size, responseWaitTime);
        }

        public void CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData)
        {
            antDevice.cryptoKeyNVMOp((ANT_ReferenceLibrary.EncryptionNVMOp)memoryOperation, nonVolatileKeyIndex, operationData);
        }

        public bool CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData, uint responseWaitTime)
        {
            return antDevice.cryptoKeyNVMOp((ANT_ReferenceLibrary.EncryptionNVMOp)memoryOperation, nonVolatileKeyIndex, operationData, responseWaitTime);
        }

        public void CrystalEnable()
        {
            antDevice.crystalEnable();
        }

        public bool CrystalEnable(uint responseWaitTime)
        {
            return antDevice.crystalEnable(responseWaitTime);
        }

        public void Dispose()
        {
            antDevice.Dispose();
        }

        public void EnableLED(bool isEnabled)
        {
            antDevice.EnableLED(isEnabled);
        }

        public bool EnableLED(bool isEnabled, uint responseWaitTime)
        {
            return antDevice.EnableLED(isEnabled, responseWaitTime);
        }

        public void EnableRxExtendedMessages(bool isEnabled)
        {
            antDevice.enableRxExtendedMessages(isEnabled);
        }

        public bool EnableRxExtendedMessages(bool isEnabled, uint responseWaitTime)
        {
            return antDevice.enableRxExtendedMessages(isEnabled, responseWaitTime);
        }

        public void FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv)
        {
            antDevice.fitAdjustPairingSettings(searchLv, pairLv, trackLv);
        }

        public bool FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv, uint responseWaitTime)
        {
            return antDevice.fitAdjustPairingSettings(searchLv, pairLv, trackLv, responseWaitTime);
        }

        public void FitSetFEState(byte feState)
        {
            antDevice.fitSetFEState(feState);
        }

        public bool FitSetFEState(byte feState, uint responseWaitTime)
        {
            return antDevice.fitSetFEState(feState, responseWaitTime);
        }

        public IAntChannel GetChannel(int num)
        {
            return new AntChannel(antDevice.getChannel(num));
        }

        public IDeviceCapabilities GetDeviceCapabilities()
        {
            return (IDeviceCapabilities)antDevice.getDeviceCapabilities();
        }

        public IDeviceCapabilities GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime)
        {
            return (IDeviceCapabilities)antDevice.getDeviceCapabilities(forceNewCopy, responseWaitTime);
        }

        public IDeviceCapabilities GetDeviceCapabilities(uint responseWaitTime)
        {
            return (IDeviceCapabilities)antDevice.getDeviceCapabilities(responseWaitTime);
        }

        public DeviceInfo GetDeviceUSBInfo()
        {
            ANT_DeviceInfo di = antDevice.getDeviceUSBInfo();
            return new DeviceInfo(di.productDescription, di.serialString);
        }

        public DeviceInfo GetDeviceUSBInfo(byte deviceNum)
        {
            ANT_DeviceInfo di = antDevice.getDeviceUSBInfo(deviceNum);
            return new DeviceInfo(di.productDescription, di.serialString);
        }

        public void LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex)
        {
            antDevice.loadCryptoKeyFromNVM(nonVolatileKeyIndex, volatileKeyIndex);
        }

        public bool LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex, uint responseWaitTime)
        {
            return antDevice.loadCryptoKeyFromNVM(nonVolatileKeyIndex, volatileKeyIndex, responseWaitTime);
        }

        public void OpenRxScanMode()
        {
            antDevice.openRxScanMode();
        }

        public bool OpenRxScanMode(uint responseWaitTime)
        {
            return antDevice.openRxScanMode(responseWaitTime);
        }

        public IAntResponse ReadUserNvm(ushort address, byte size)
        {
            return new AntResponse(antDevice.readUserNvm(address, size));
        }

        public IAntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime)
        {
            return new AntResponse(antDevice.readUserNvm(address, size, responseWaitTime));
        }

        public void RequestMessage(byte channelNum, RequestMessageID messageID)
        {
            antDevice.requestMessage(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID);
        }

        public void RequestMessage(RequestMessageID messageID)
        {
            antDevice.requestMessage((ANT_ReferenceLibrary.RequestMessageID)messageID);
        }

        public IAntResponse RequestMessageAndResponse(byte channelNum, RequestMessageID messageID, uint responseWaitTime)
        {
            return new AntResponse(antDevice.requestMessageAndResponse(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID, responseWaitTime));
        }

        public IAntResponse RequestMessageAndResponse(RequestMessageID messageID, uint responseWaitTime)
        {
            return new AntResponse(antDevice.requestMessageAndResponse((ANT_ReferenceLibrary.RequestMessageID)messageID, responseWaitTime));
        }

        public void ResetSystem()
        {
            antDevice.ResetSystem();
        }

        public bool ResetSystem(uint responseWaitTime)
        {
            return antDevice.ResetSystem(responseWaitTime);
        }

        public void ResetUSB()
        {
            antDevice.ResetUSB();
        }

        public void SetCryptoID(byte[] encryptionID)
        {
            antDevice.setCryptoID(encryptionID);
        }

        public bool SetCryptoID(byte[] encryptionID, uint responseWaitTime)
        {
            return antDevice.setCryptoID(encryptionID, responseWaitTime);
        }

        public void SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData)
        {
            antDevice.setCryptoInfo((ANT_ReferenceLibrary.EncryptionInfo)encryptionParameter, parameterData);
        }

        public bool SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData, uint responseWaitTime)
        {
            return antDevice.setCryptoInfo((ANT_ReferenceLibrary.EncryptionInfo)encryptionParameter, parameterData, responseWaitTime);
        }

        public void SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey)
        {
            antDevice.setCryptoKey(volatileKeyIndex, encryptionKey);
        }

        public bool SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey, uint responseWaitTime)
        {
            return antDevice.setCryptoKey(volatileKeyIndex, encryptionKey, responseWaitTime);
        }

        public void SetCryptoRNGSeed(byte[] cryptoRNGSeed)
        {
            antDevice.setCryptoRNGSeed(cryptoRNGSeed);
        }

        public bool SetCryptoRNGSeed(byte[] cryptoRNGSeed, uint responseWaitTime)
        {
            return antDevice.setCryptoRNGSeed(cryptoRNGSeed, responseWaitTime);
        }

        public void SetCryptoUserInfo(byte[] userInfoString)
        {
            antDevice.setCryptoUserInfo(userInfoString);
        }

        public bool SetCryptoUserInfo(byte[] userInfoString, uint responseWaitTime)
        {
            return antDevice.setCryptoUserInfo(userInfoString, responseWaitTime);
        }

        public void SetLibConfig(LibConfigFlags libConfigFlags)
        {
            antDevice.setLibConfig((ANT_ReferenceLibrary.LibConfigFlags)libConfigFlags);
        }

        public bool SetLibConfig(LibConfigFlags libConfigFlags, uint responseWaitTime)
        {
            return antDevice.setLibConfig((ANT_ReferenceLibrary.LibConfigFlags)libConfigFlags, responseWaitTime);
        }

        public void SetNetworkKey(byte netNumber, byte[] networkKey)
        {
            antDevice.setNetworkKey(netNumber, networkKey);
        }

        public bool SetNetworkKey(byte netNumber, byte[] networkKey, uint responseWaitTime)
        {
            return antDevice.setNetworkKey(netNumber, networkKey, responseWaitTime);
        }

        public void SetTransmitPowerForAllChannels(TransmitPower transmitPower)
        {
            antDevice.setTransmitPowerForAllChannels((ANT_ReferenceLibrary.TransmitPower)transmitPower);
        }

        public bool SetTransmitPowerForAllChannels(TransmitPower transmitPower, uint responseWaitTime)
        {
            return antDevice.setTransmitPowerForAllChannels((ANT_ReferenceLibrary.TransmitPower)transmitPower, responseWaitTime);
        }

        public void StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey)
        {
            antDevice.storeCryptoKeyToNVM(nonVolatileKeyIndex, encryptionKey);
        }

        public bool StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey, uint responseWaitTime)
        {
            return antDevice.storeCryptoKeyToNVM(nonVolatileKeyIndex, encryptionKey, responseWaitTime);
        }

        public bool WriteRawMessageToDevice(byte msgID, byte[] msgData)
        {
            return antDevice.writeRawMessageToDevice(msgID, msgData);
        }
    }
}
