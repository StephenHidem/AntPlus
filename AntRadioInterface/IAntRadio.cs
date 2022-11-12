using System;

namespace AntRadioInterface
{
    /// <summary>
    /// Flags for configuring advanced bursting features.
    /// </summary>
    [Flags]
    public enum AdvancedBurstConfigFlags : uint
    {
        FrequencyHopEnable = 0x00000001,
    };

    /// <summary>
    /// Event groups for configuring Event Buffering
    /// </summary>
    [Flags]
    public enum EventBufferConfig : byte
    {
        BufferLowPriorityEvents = 0x00,
        BufferAllEvents = 0x01
    };

    public enum EncryptionNVMOp : byte
    {
        LoadKeyFromNvm = 0x00,
        StoreKeyToNvm = 0x01,
    }

    /// <summary>
    /// Possible port connection types.
    /// </summary>
    public enum PortType : byte
    {
        USB = 0x00,
        COM = 0x01
    };

    /// <summary>
    /// Possible framing modes.
    /// Use BasicANT unless you know you need to use another.
    /// </summary>
    public enum FramerType : byte
    {
        BasicANT = 0x00,
    };

    /// <summary>
    /// Message ID to request message.
    /// Note: Where desired, raw byte values can be cast to the enum type. IE: <c>(RequestMessageID)0x4E</c> will compile.
    /// </summary>
    public enum RequestMessageID : byte
    {
        Version = 0x3E,
        ChannelId = 0x51,
        ChannelStatus = 0x52,
        Capabilities = 0x54,
        SerialNumber = 0x61,
        UserNvm = 0x7C,
    };

    public enum EncryptionInfo : byte
    {
        EncryptionId = 0x00,
        UserInfoString = 0x01,
        RandomNumberSeed = 0x02,
    };

    /// <summary>
    /// Flags for configuring device ANT library
    /// </summary>
    [Flags]
    public enum LibConfigFlags
    {
        RadioConfigAlways = 0x01,
        MesgOutIncTimeStamp = 0x20,
        MesgOutIncRssi = 0x40,
        MesgOutIncDeviceId = 0x80,
    }

    /// <summary>
    /// Transmit Power offSets
    /// </summary>
    public enum TransmitPower : byte
    {
        TxMinus20DB = 0x00,
        TxMinus10DB = 0x01,
        TxMinus5DB = 0x02,
        Tx0DB = 0x03
    };

    [Flags]
    public enum FlagByte
    {
        None = 0,
        RxTimestamp = 0x20,
        Rssi = 0x40,
        ChannelId = 0x80,
    }

    public enum MessageId

    {
        BroadcastData = 0x4E,
        AcknowledgedData = 0x4F,
        BurstData = 0x50,
        ExtBroadcastData = 0x5D,
        ExtAcknowledgedData = 0x5E,
        ExtBurstData = 0x5F
    }

    public enum EventMsgId : byte
    {
        RxSearchTimeout = 0x01,
        RxFailed = 0x02,
        TxSuccess = 0x03,
        TransferRxFailed = 0x04,
        TransferTxCompleted = 0x05,
        TransferTxFailed = 0x06,
        ChannelClosed = 0x07,
        RxFailGoToSearch = 0x08,
        ChannelCollision = 0x09,
        TransferTxStart = 0x0A,
        TransferNextDataBlock = 0x11,
        SerialQueueOverflow = 0x34,
        QueueOverflow = 0x35
    }

    public enum ResponseMsgId : byte
    {
        NoError = 0x00,
        ChannelInWrongState = 0x15,
        ChannelNotOpened = 0x16,
        ChannelIdNotSet = 0x18,
        CloseAllChannels = 0x19,
        TransferInProgress = 0x1F,
        TransferSeqNumberError = 0x20,
        TransferInError = 0x21,
        MsgSizeExceedsLimit = 0x27,
        InvalidMsg = 0x28,
        InvalidNetworkNumber = 0x29,
        InvalidListId = 0x30,
        InvalidScanTxChannels = 0x31,
        InvalidParameter = 0x33,
        EncryptNegotiationSucces = 0x38,
        EncryptNegotiationFail = 0x39,
        NvmFullError = 0x40,
        NvmWriteError = 0x41,
        UsbStringWriteFail = 0x70,
        MsgSerialErrorId = 0xAE
    }

    public interface IAntRadio
    {
        event EventHandler<IAntResponse> RadioResponse;
        void CancelTransfers(int cancelWaitTime);
        void ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields);
        bool ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, uint responseWaitTime);
        void ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount);
        bool ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount, uint responseWaitTime);
        bool ConfigureAdvancedBurstSplitting(bool splitBursts);
        void ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time);
        bool ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time, uint responseWaitTime);
        void ConfigureEventFilter(ushort eventFilter);
        bool ConfigureEventFilter(ushort eventFilter, uint responseWaitTime);
        void ConfigureHighDutySearch(bool enable, byte suppressionCycles);
        bool ConfigureHighDutySearch(bool enable, byte suppressionCycles, uint responseWaitTime);
        void ConfigureUserNvm(ushort address, byte[] data, byte size);
        bool ConfigureUserNvm(ushort address, byte[] data, byte size, uint responseWaitTime);
        void CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData);
        bool CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData, uint responseWaitTime);
        void CrystalEnable();
        bool CrystalEnable(uint responseWaitTime);
        void Dispose();
        void EnableLED(bool isEnabled);
        bool EnableLED(bool isEnabled, uint responseWaitTime);
        void EnableRxExtendedMessages(bool isEnabled);
        bool EnableRxExtendedMessages(bool isEnabled, uint responseWaitTime);
        void FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv);
        bool FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv, uint responseWaitTime);
        void FitSetFEState(byte feState);
        bool FitSetFEState(byte feState, uint responseWaitTime);
        IAntChannel GetChannel(int num);
        IDeviceCapabilities GetDeviceCapabilities();
        IDeviceCapabilities GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime);
        IDeviceCapabilities GetDeviceCapabilities(uint responseWaitTime);
        IDeviceInfo GetDeviceUSBInfo();
        IDeviceInfo GetDeviceUSBInfo(byte deviceNum);
        ushort DeviceUSBPID { get; }

        ushort DeviceUSBVID { get; }

        int NumChannels { get; }
        FramerType OpenedFrameType { get; }
        PortType OpenedPortType { get; }

        uint OpenedUSBBaudRate { get; }

        int OpenedUSBDeviceNum { get; }

        uint SerialNumber { get; }

        void LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex);
        bool LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex, uint responseWaitTime);
        void OpenRxScanMode();
        bool OpenRxScanMode(uint responseWaitTime);
        IAntResponse ReadUserNvm(ushort address, byte size);
        IAntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime);
        void RequestMessage(byte channelNum, RequestMessageID messageID);
        void RequestMessage(RequestMessageID messageID);
        IAntResponse RequestMessageAndResponse(byte channelNum, RequestMessageID messageID, uint responseWaitTime);
        IAntResponse RequestMessageAndResponse(RequestMessageID messageID, uint responseWaitTime);
        void ResetSystem();
        bool ResetSystem(uint responseWaitTime);
        void ResetUSB();
        void SetCryptoID(byte[] encryptionID);
        bool SetCryptoID(byte[] encryptionID, uint responseWaitTime);
        void SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData);
        bool SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData, uint responseWaitTime);
        void SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey);
        bool SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey, uint responseWaitTime);
        void SetCryptoRNGSeed(byte[] cryptoRNGSeed);
        bool SetCryptoRNGSeed(byte[] cryptoRNGSeed, uint responseWaitTime);
        void SetCryptoUserInfo(byte[] userInfoString);
        bool SetCryptoUserInfo(byte[] userInfoString, uint responseWaitTime);
        void SetLibConfig(LibConfigFlags libConfigFlags);
        bool SetLibConfig(LibConfigFlags libConfigFlags, uint responseWaitTime);
        void SetNetworkKey(byte netNumber, byte[] networkKey);
        bool SetNetworkKey(byte netNumber, byte[] networkKey, uint responseWaitTime);
        void SetTransmitPowerForAllChannels(TransmitPower transmitPower);
        bool SetTransmitPowerForAllChannels(TransmitPower transmitPower, uint responseWaitTime);
        void StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey);
        bool StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey, uint responseWaitTime);
        string ToString();
        bool WriteRawMessageToDevice(byte msgID, byte[] msgData);
    }
}
