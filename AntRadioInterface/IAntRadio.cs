using System;
using System.Threading.Tasks;

namespace SmallEarthTech.AntRadioInterface
{
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
        /// <summary>The encrypt negotiation success</summary>
        EncryptNegotiationSuccess = 0x38,
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

        /// <summary>Initializes the ANT radio for continuous scan mode.</summary>
        /// <returns>
        /// Returns an array of ANT channels. The first element of the array (ANT channel 0) is used for continuous
        /// scan mode to receive broadcast messages from ANT master devices. The remaining channels
        /// should be configured so messages may be sent to ANT master devices.
        /// </returns>
        /// <remarks>
        /// Implementors typically would perform the following setup -
        /// <code>
        /// public IAntChannel[] InitializeContinuousScanMode()
        /// {
        ///     IAntChannel[] channels = new IAntChannel[NumChannels];
        ///     
        ///     // configure channel 0 for continuous scan mode
        ///     SetNetworkKey(0, new byte[] { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 });
        ///     EnableRxExtendedMessages(true);
        ///     channels[0] = GetChannel(0);
        ///     channels[0].AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
        ///     channels[0].SetChannelID(new ChannelId(0), 500);
        ///     channels[0].SetChannelFreq(57, 500);
        ///     OpenRxScanMode();
        ///     
        ///     // assign channels for devices to use for sending messages
        ///     for (int i = 1; i &lt; NumChannels; i++)
        ///     {
        ///         channels[i] = GetChannel(i);
        ///         _ = channels[i].AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
        ///     }
        ///     return channels;
        /// }
        /// </code>
        /// </remarks>
        Task<IAntChannel[]> InitializeContinuousScanMode();
        /// <summary>Cancels the transfers.</summary>
        /// <param name="cancelWaitTime">The cancel wait time.</param>
        void CancelTransfers(int cancelWaitTime);
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
        Task<DeviceCapabilities> GetDeviceCapabilities();
        /// <summary>Gets the device capabilities.</summary>
        /// <param name="forceNewCopy">if set to <c>true</c> [force new copy].</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// Device capabilities
        /// </returns>
        Task<DeviceCapabilities> GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime);
        /// <summary>Gets the device capabilities.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// Device capabilities
        /// </returns>
        Task<DeviceCapabilities> GetDeviceCapabilities(uint responseWaitTime);
        /// <summary>Gets the number channels.</summary>
        /// <value>The number channels.</value>
        int NumChannels { get; }
        /// <summary>Gets the type of the opened frame.</summary>
        /// <value>The type of the opened frame.</value>
        FramerType OpenedFrameType { get; }
        /// <summary>Gets the type of the opened port.</summary>
        /// <value>The type of the opened port.</value>
        PortType OpenedPortType { get; }

        /// <summary>Gets the serial number.</summary>
        uint SerialNumber { get; }

        /// <summary>Reads the user NVM.</summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        /// <see cref="AntResponse"/>
        /// </returns>
        AntResponse ReadUserNvm(ushort address, byte size);
        /// <summary>Reads the user NVM.</summary>
        /// <param name="address">The address.</param>
        /// <param name="size">The size.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// <see cref="AntResponse"/>
        /// </returns>
        AntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime);
        /// <summary>Requests a message and waits to return the response.</summary>
        /// <param name="channelNum">The radio channel number to use.</param>
        /// <param name="messageID">The message identifier.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// <see cref="AntResponse"/>
        /// </returns>
        AntResponse RequestMessageAndResponse(byte channelNum, RequestMessageID messageID, uint responseWaitTime);
        /// <summary>Requests a message and waits to return the response.</summary>
        /// <param name="messageID">The message identifier.</param>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>
        /// <see cref="AntResponse"/>
        /// </returns>
        AntResponse RequestMessageAndResponse(RequestMessageID messageID, uint responseWaitTime);
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
