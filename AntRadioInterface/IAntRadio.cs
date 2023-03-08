using System;

namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>
    /// Flags for configuring advanced bursting features.
    /// </summary>
    [Flags]
    public enum AdvancedBurstConfigFlags : uint
    {
        /// <summary>The frequency hop enable</summary>
        FrequencyHopEnable = 0x00000001,
    };

    /// <summary>
    /// Event groups for configuring Event Buffering
    /// </summary>
    [Flags]
    public enum EventBufferConfig : byte
    {
        /// <summary>The buffer low priority events</summary>
        BufferLowPriorityEvents = 0x00,
        /// <summary>The buffer all events</summary>
        BufferAllEvents = 0x01
    };

    /// <summary>Encryption non-volatile memory operations.</summary>
    public enum EncryptionNVMOp : byte
    {
        /// <summary>The load key from NVM</summary>
        LoadKeyFromNvm = 0x00,
        /// <summary>The store key to NVM</summary>
        StoreKeyToNvm = 0x01,
    }

    /// <summary>
    /// Possible port connection types.
    /// </summary>
    public enum PortType : byte
    {
        /// <summary>The USB port</summary>
        USB = 0x00,
        /// <summary>The CON port</summary>
        COM = 0x01
    };

    /// <summary>
    /// Possible framing modes. Use BasicANT unless you know you need to use another.
    /// </summary>
    public enum FramerType : byte
    {
        /// <summary>The basic ANT</summary>
        BasicANT = 0x00,
    };

    /// <summary>
    /// Message ID to request message.
    /// Note: Where desired, raw byte values can be cast to the enum type. IE: <c>(RequestMessageID)0x4E</c> will compile.
    /// </summary>
    public enum RequestMessageID : byte
    {
        /// <summary>The version</summary>
        Version = 0x3E,
        /// <summary>The channel identifier</summary>
        ChannelId = 0x51,
        /// <summary>The channel status</summary>
        ChannelStatus = 0x52,
        /// <summary>The capabilities</summary>
        Capabilities = 0x54,
        /// <summary>The serial number</summary>
        SerialNumber = 0x61,
        /// <summary>The user NVM</summary>
        UserNvm = 0x7C,
    };

    /// <summary>Encryption information.</summary>
    public enum EncryptionInfo : byte
    {
        /// <summary>The encryption identifier</summary>
        EncryptionId = 0x00,
        /// <summary>The user information string</summary>
        UserInfoString = 0x01,
        /// <summary>The random number seed</summary>
        RandomNumberSeed = 0x02,
    };

    /// <summary>
    /// Flags for configuring device ANT library
    /// </summary>
    [Flags]
    public enum LibConfigFlags
    {
        /// <summary>The radio configuration always</summary>
        RadioConfigAlways = 0x01,
        /// <summary>The message out inc time stamp</summary>
        MesgOutIncTimeStamp = 0x20,
        /// <summary>The message out inc rssi</summary>
        MesgOutIncRssi = 0x40,
        /// <summary>The message out inc device identifier</summary>
        MesgOutIncDeviceId = 0x80,
    }

    /// <summary>
    /// Transmit Power offSets
    /// </summary>
    public enum TransmitPower : byte
    {
        /// <summary>The minus 20 dB</summary>
        Minus20DB = 0x00,
        /// <summary>The minus 10 dB</summary>
        Minus10DB = 0x01,
        /// <summary>The minus 5 dB</summary>
        Minus5DB = 0x02,
        /// <summary>0 dB</summary>
        Tx0DB = 0x03
    };

    /// <summary>Flag byte enumeration.</summary>
    [Flags]
    public enum FlagByte
    {
        /// <summary>The none</summary>
        None = 0,
        /// <summary>The Rx timestamp</summary>
        RxTimestamp = 0x20,
        /// <summary>The RSSI</summary>
        Rssi = 0x40,
        /// <summary>The channel identifier</summary>
        ChannelId = 0x80,
    }

    /// <summary>Message identifier of the received data.</summary>
    public enum MessageId

    {
        /// <summary>The broadcast data</summary>
        BroadcastData = 0x4E,
        /// <summary>The acknowledged data</summary>
        AcknowledgedData = 0x4F,
        /// <summary>The burst data</summary>
        BurstData = 0x50,
        /// <summary>The extended broadcast data</summary>
        ExtBroadcastData = 0x5D,
        /// <summary>The extended acknowledged data</summary>
        ExtAcknowledgedData = 0x5E,
        /// <summary>The extended burst data</summary>
        ExtBurstData = 0x5F
    }

    /// <summary>Channel event message identifier.</summary>
    public enum EventMsgId : byte
    {
        /// <summary>The Rx search timeout</summary>
        RxSearchTimeout = 0x01,
        /// <summary>The Rx failed</summary>
        RxFailed = 0x02,
        /// <summary>The Tx success</summary>
        TxSuccess = 0x03,
        /// <summary>The transfer Rx failed</summary>
        TransferRxFailed = 0x04,
        /// <summary>The transfer Tx completed</summary>
        TransferTxCompleted = 0x05,
        /// <summary>The transfer Tx failed</summary>
        TransferTxFailed = 0x06,
        /// <summary>The channel closed</summary>
        ChannelClosed = 0x07,
        /// <summary>The Rx failed to go to search</summary>
        RxFailGoToSearch = 0x08,
        /// <summary>The channel collision</summary>
        ChannelCollision = 0x09,
        /// <summary>The transfer Tx start</summary>
        TransferTxStart = 0x0A,
        /// <summary>The transfer next data block</summary>
        TransferNextDataBlock = 0x11,
        /// <summary>The serial queue overflow</summary>
        SerialQueueOverflow = 0x34,
        /// <summary>The queue overflow</summary>
        QueueOverflow = 0x35
    }

    /// <summary>Channel response message identifier.</summary>
    public enum ResponseMsgId : byte
    {
        /// <summary>The no error</summary>
        NoError = 0x00,
        /// <summary>The channel in wrong state</summary>
        ChannelInWrongState = 0x15,
        /// <summary>The channel not opened</summary>
        ChannelNotOpened = 0x16,
        /// <summary>The channel identifier not set</summary>
        ChannelIdNotSet = 0x18,
        /// <summary>The close all channels</summary>
        CloseAllChannels = 0x19,
        /// <summary>The transfer in progress</summary>
        TransferInProgress = 0x1F,
        /// <summary>The transfer sequence number error</summary>
        TransferSeqNumberError = 0x20,
        /// <summary>The transfer in error</summary>
        TransferInError = 0x21,
        /// <summary>The message size exceeds limit</summary>
        MsgSizeExceedsLimit = 0x27,
        /// <summary>The invalid message</summary>
        InvalidMsg = 0x28,
        /// <summary>The invalid network number</summary>
        InvalidNetworkNumber = 0x29,
        /// <summary>The invalid list identifier</summary>
        InvalidListId = 0x30,
        /// <summary>The invalid scan Tx channels</summary>
        InvalidScanTxChannels = 0x31,
        /// <summary>The invalid parameter</summary>
        InvalidParameter = 0x33,
        /// <summary>The encrypt negotiation succes</summary>
        EncryptNegotiationSucces = 0x38,
        /// <summary>The encrypt negotiation failed</summary>
        EncryptNegotiationFail = 0x39,
        /// <summary>The NVM full error</summary>
        NvmFullError = 0x40,
        /// <summary>The NVM write error</summary>
        NvmWriteError = 0x41,
        /// <summary>The USB string write fail</summary>
        UsbStringWriteFail = 0x70,
        /// <summary>The message serial error identifier</summary>
        MsgSerialErrorId = 0xAE
    }

    /// <summary>ANT radio interface.</summary>
    public interface IAntRadio
    {
        /// <summary>Occurs when radio response has been received.</summary>
        event EventHandler<AntResponse> RadioResponse;
        /// <summary>Cancels the transfers.</summary>
        /// <param name="cancelWaitTime">The cancel wait time.</param>
        void CancelTransfers(int cancelWaitTime);
        /// <summary>Configures the advanced bursting.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="maxPacketLength">Maximum length of the packet.</param>
        /// <param name="requiredFields">The required fields.</param>
        /// <param name="optionalFields">The optional fields.</param>
        void ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields);
        /// <summary>Configures the advanced bursting.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="maxPacketLength">Maximum length of the packet.</param>
        /// <param name="requiredFields">The required fields.</param>
        /// <param name="optionalFields">The optional fields.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureAdvancedBursting(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, uint responseWaitTime);
        /// <summary>Configures the extended advanced bursting.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="maxPacketLength">Maximum length of the packet.</param>
        /// <param name="requiredFields">The required fields.</param>
        /// <param name="optionalFields">The optional fields.</param>
        /// <param name="stallCount">The stall count.</param>
        /// <param name="retryCount">The retry count.</param>
        void ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount);
        /// <summary>Configures the extended advanced bursting.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="maxPacketLength">Maximum length of the packet.</param>
        /// <param name="requiredFields">The required fields.</param>
        /// <param name="optionalFields">The optional fields.</param>
        /// <param name="stallCount">The stall count.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureAdvancedBursting_ext(bool enable, byte maxPacketLength, AdvancedBurstConfigFlags requiredFields, AdvancedBurstConfigFlags optionalFields, ushort stallCount, byte retryCount, uint responseWaitTime);
        /// <summary>Configures the advanced burst splitting.</summary>
        /// <param name="splitBursts">if set to <c>true</c> [split bursts].</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureAdvancedBurstSplitting(bool splitBursts);
        /// <summary>Configures the event buffer.</summary>
        /// <param name="config">The configuration.</param>
        /// <param name="size">The size.</param>
        /// <param name="time">The time.</param>
        void ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time);
        /// <summary>Configures the event buffer.</summary>
        /// <param name="config">The configuration.</param>
        /// <param name="size">The size.</param>
        /// <param name="time">The time.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureEventBuffer(EventBufferConfig config, ushort size, ushort time, uint responseWaitTime);
        /// <summary>Configures the event filter.</summary>
        /// <param name="eventFilter">The event filter.</param>
        void ConfigureEventFilter(ushort eventFilter);
        /// <summary>Configures the event filter.</summary>
        /// <param name="eventFilter">The event filter.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureEventFilter(ushort eventFilter, uint responseWaitTime);
        /// <summary>Configures the high duty search.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="suppressionCycles">The suppression cycles.</param>
        void ConfigureHighDutySearch(bool enable, byte suppressionCycles);
        /// <summary>Configures the high duty search.</summary>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <param name="suppressionCycles">The suppression cycles.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureHighDutySearch(bool enable, byte suppressionCycles, uint responseWaitTime);
        /// <summary>Configures the user NVM.</summary>
        /// <param name="address">The address.</param>
        /// <param name="data">The data.</param>
        /// <param name="size">The size.</param>
        void ConfigureUserNvm(ushort address, byte[] data, byte size);
        /// <summary>Configures the user NVM.</summary>
        /// <param name="address">The address.</param>
        /// <param name="data">The data.</param>
        /// <param name="size">The size.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ConfigureUserNvm(ushort address, byte[] data, byte size, uint responseWaitTime);
        /// <summary>Crypto key NVM operation.</summary>
        /// <param name="memoryOperation">The memory operation.</param>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="operationData">The operation data.</param>
        void CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData);
        /// <summary>Crypto key NVM operation.</summary>
        /// <param name="memoryOperation">The memory operation.</param>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="operationData">The operation data.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool CryptoKeyNVMOp(EncryptionNVMOp memoryOperation, byte nonVolatileKeyIndex, byte[] operationData, uint responseWaitTime);
        /// <summary>Crystal enable.</summary>
        void CrystalEnable();
        /// <summary>Crystal enable.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool CrystalEnable(uint responseWaitTime);
        /// <summary>Enables the led.</summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        void EnableLED(bool isEnabled);
        /// <summary>Enables the led.</summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool EnableLED(bool isEnabled, uint responseWaitTime);
        /// <summary>Enables Rx extended messages.</summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        void EnableRxExtendedMessages(bool isEnabled);
        /// <summary>Enables Rx extended messages.</summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool EnableRxExtendedMessages(bool isEnabled, uint responseWaitTime);
        /// <summary>FIT adjust pairing settings.</summary>
        /// <param name="searchLv">The search lv.</param>
        /// <param name="pairLv">The pair lv.</param>
        /// <param name="trackLv">The track lv.</param>
        void FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv);
        /// <summary>FIT adjust pairing settings.</summary>
        /// <param name="searchLv">The search lv.</param>
        /// <param name="pairLv">The pair lv.</param>
        /// <param name="trackLv">The track lv.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool FitAdjustPairingSettings(byte searchLv, byte pairLv, byte trackLv, uint responseWaitTime);
        /// <summary>FIT set the state of the fitness equipment.</summary>
        /// <param name="feState">State of the fe.</param>
        void FitSetFEState(byte feState);
        /// <summary>FIT set the state of the fitness equipment.</summary>
        /// <param name="feState">State of the fe.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool FitSetFEState(byte feState, uint responseWaitTime);
        /// <summary>Gets the channel.</summary>
        /// <param name="num">The number.</param>
        /// <returns>
        /// ANT channel
        /// </returns>
        IAntChannel GetChannel(int num);
        /// <summary>Gets the device capabilities.</summary>
        /// <returns>
        /// Device capabilities
        /// </returns>
        IDeviceCapabilities GetDeviceCapabilities();
        /// <summary>Gets the device capabilities.</summary>
        /// <param name="forceNewCopy">if set to <c>true</c> [force new copy].</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// Device capabilities
        /// </returns>
        IDeviceCapabilities GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime);
        /// <summary>Gets the device capabilities.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// Device capabilities
        /// </returns>
        IDeviceCapabilities GetDeviceCapabilities(uint responseWaitTime);
        /// <summary>Gets the device USB information.</summary>
        /// <returns>
        /// Device information
        /// </returns>
        IDeviceInfo GetDeviceUSBInfo();
        /// <summary>Gets the device USB information.</summary>
        /// <param name="deviceNum">The device number.</param>
        /// <returns>
        /// Device information
        /// </returns>
        IDeviceInfo GetDeviceUSBInfo(byte deviceNum);
        /// <summary>Gets the device USB PID.</summary>
        ushort DeviceUSBPID { get; }

        /// <summary>Gets the device USB VID.</summary>
        ushort DeviceUSBVID { get; }

        /// <summary>Gets the number channels.</summary>
        /// <value>The number channels.</value>
        int NumChannels { get; }
        /// <summary>Gets the type of the opened frame.</summary>
        /// <value>The type of the opened frame.</value>
        FramerType OpenedFrameType { get; }
        /// <summary>Gets the type of the opened port.</summary>
        /// <value>The type of the opened port.</value>
        PortType OpenedPortType { get; }

        /// <summary>Gets the opened USB baud rate.</summary>
        uint OpenedUSBBaudRate { get; }

        /// <summary>Gets the opened USB device number.</summary>
        int OpenedUSBDeviceNum { get; }

        /// <summary>Gets the serial number.</summary>
        uint SerialNumber { get; }

        /// <summary>Loads the crypto key from NVM.</summary>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="volatileKeyIndex">Index of the volatile key.</param>
        void LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex);
        /// <summary>Loads the crypto key from NVM.</summary>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="volatileKeyIndex">Index of the volatile key.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool LoadCryptoKeyFromNVM(byte nonVolatileKeyIndex, byte volatileKeyIndex, uint responseWaitTime);
        /// <summary>Opens the Rx scan mode.</summary>
        void OpenRxScanMode();
        /// <summary>Opens the Rx scan mode.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool OpenRxScanMode(uint responseWaitTime);
        /// <summary>Reads the user NVM.</summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///  ANT response
        /// </returns>
        AntResponse ReadUserNvm(ushort address, byte size);
        /// <summary>Reads the user NVM.</summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// ANT response
        /// </returns>
        AntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime);
        /// <summary>Requests the message.</summary>
        /// <param name="channelNum">The channel number.</param>
        /// <param name="messageID">The message identifier.</param>
        void RequestMessage(byte channelNum, RequestMessageID messageID);
        /// <summary>Requests the message.</summary>
        /// <param name="messageID">The message identifier.</param>
        void RequestMessage(RequestMessageID messageID);
        /// <summary>Requests the message and response.</summary>
        /// <param name="channelNum">The channel number.</param>
        /// <param name="messageID">The message identifier.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// ANT response
        /// </returns>
        AntResponse RequestMessageAndResponse(byte channelNum, RequestMessageID messageID, uint responseWaitTime);
        /// <summary>Requests the message and response.</summary>
        /// <param name="messageID">The message identifier.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// ANT response
        /// </returns>
        AntResponse RequestMessageAndResponse(RequestMessageID messageID, uint responseWaitTime);
        /// <summary>Resets the system.</summary>
        void ResetSystem();
        /// <summary>Resets the system.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool ResetSystem(uint responseWaitTime);
        /// <summary>Resets the USB.</summary>
        void ResetUSB();
        /// <summary>Sets the crypto identifier.</summary>
        /// <param name="encryptionID">The encryption identifier.</param>
        void SetCryptoID(byte[] encryptionID);
        /// <summary>Sets the crypto identifier.</summary>
        /// <param name="encryptionID">The encryption identifier.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoID(byte[] encryptionID, uint responseWaitTime);
        /// <summary>Sets the crypto information.</summary>
        /// <param name="encryptionParameter">The encryption parameter.</param>
        /// <param name="parameterData">The parameter data.</param>
        void SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData);
        /// <summary>Sets the crypto information.</summary>
        /// <param name="encryptionParameter">The encryption parameter.</param>
        /// <param name="parameterData">The parameter data.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoInfo(EncryptionInfo encryptionParameter, byte[] parameterData, uint responseWaitTime);
        /// <summary>Sets the crypto key.</summary>
        /// <param name="volatileKeyIndex">Index of the volatile key.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        void SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey);
        /// <summary>Sets the crypto key.</summary>
        /// <param name="volatileKeyIndex">Index of the volatile key.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoKey(byte volatileKeyIndex, byte[] encryptionKey, uint responseWaitTime);
        /// <summary>Sets the crypto RNG seed.</summary>
        /// <param name="cryptoRNGSeed">The crypto RNG seed.</param>
        void SetCryptoRNGSeed(byte[] cryptoRNGSeed);
        /// <summary>Sets the crypto RNG seed.</summary>
        /// <param name="cryptoRNGSeed">The crypto RNG seed.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoRNGSeed(byte[] cryptoRNGSeed, uint responseWaitTime);
        /// <summary>Sets the crypto user information.</summary>
        /// <param name="userInfoString">The user information string.</param>
        void SetCryptoUserInfo(byte[] userInfoString);
        /// <summary>Sets the crypto user information.</summary>
        /// <param name="userInfoString">The user information string.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetCryptoUserInfo(byte[] userInfoString, uint responseWaitTime);
        /// <summary>Sets the library configuration.</summary>
        /// <param name="libConfigFlags">The library configuration flags.</param>
        void SetLibConfig(LibConfigFlags libConfigFlags);
        /// <summary>Sets the library configuration.</summary>
        /// <param name="libConfigFlags">The library configuration flags.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetLibConfig(LibConfigFlags libConfigFlags, uint responseWaitTime);
        /// <summary>Sets the network key.</summary>
        /// <param name="netNumber">The net number.</param>
        /// <param name="networkKey">The network key.</param>
        void SetNetworkKey(byte netNumber, byte[] networkKey);
        /// <summary>Sets the network key.</summary>
        /// <param name="netNumber">The net number.</param>
        /// <param name="networkKey">The network key.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetNetworkKey(byte netNumber, byte[] networkKey, uint responseWaitTime);
        /// <summary>Sets the transmit power for all channels.</summary>
        /// <param name="transmitPower">The transmit power.</param>
        void SetTransmitPowerForAllChannels(TransmitPower transmitPower);
        /// <summary>Sets the transmit power for all channels.</summary>
        /// <param name="transmitPower">The transmit power.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SetTransmitPowerForAllChannels(TransmitPower transmitPower, uint responseWaitTime);
        /// <summary>Stores the crypto key to NVM.</summary>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        void StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey);
        /// <summary>Stores the crypto key to NVM.</summary>
        /// <param name="nonVolatileKeyIndex">Index of the non volatile key.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool StoreCryptoKeyToNVM(byte nonVolatileKeyIndex, byte[] encryptionKey, uint responseWaitTime);
        /// <summary>Converts to string.</summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        string ToString();
        /// <summary>Writes the raw message to device.</summary>
        /// <param name="msgID">The MSG identifier.</param>
        /// <param name="msgData">The MSG data.</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool WriteRawMessageToDevice(byte msgID, byte[] msgData);
    }
}
