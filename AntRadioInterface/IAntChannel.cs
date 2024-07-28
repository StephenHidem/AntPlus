using System;
using System.Threading.Tasks;

namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>
    /// Channel Type flags. A valid channel type is one BASE parameter (Slave XOR Receive)
    /// combined by '|' (bitwise OR) with any desired ADV parameters
    /// </summary>
    [Flags]
    public enum ChannelType : byte
    {
        /// <summary>The base slave receive</summary>
        BaseSlaveReceive = 0x00,
        /// <summary>The base master transmit</summary>
        BaseMasterTransmit = 0x10,
        /// <summary>The advanced shared channel</summary>
        AdvShared = 0x20,
        /// <summary>The advanced Tx/Rx only or Rx always wild card</summary>
        AdvTxRxOnlyOrRxAlwaysWildCard = 0x40,
    };

    /// <summary>The extended channel type enum.</summary>
    [Flags]
    public enum ChannelTypeExtended : byte
    {
        /// <summary>The advanced always search</summary>
        AdvAlwaysSearch = 0x01,
        /// <summary>The advanced frequency agility</summary>
        AdvFrequencyAgility = 0x04,
        /// <summary>The advanced fast start</summary>
        AdvFastStart = 0x10,
        /// <summary>The advanced asynchronous Tx</summary>
        AdvAsyncTx = 0x20,
    }

    /// <summary>
    /// The int status codes returned by the acknowledged and broadcast messaging functions.
    /// </summary>
    public enum MessagingReturnCode : int
    {
        /// <summary>Failed</summary>
        Fail = 0,
        /// <summary>Passed</summary>
        Pass = 1,
        /// <summary>Timeout</summary>
        Timeout = 2,
        /// <summary>Cancelled</summary>
        Cancelled = 3,
        /// <summary>Invalid parameters</summary>
        InvalidParams = 4,
    }

    /// <summary>
    /// ANT channel sharing enumeration. This is obtained from the transmission type in the channel ID.
    /// </summary>
    public enum ChannelSharing
    {
        /// <summary>The reserved</summary>
        Reserved = 0,
        /// <summary>
        /// The independent channel
        /// </summary>
        IndependentChannel = 1,
        /// <summary>
        /// The shared channel one byte address
        /// </summary>
        SharedChannelOneByteAddress = 2,
        /// <summary>
        /// The shared channel two byte address
        /// </summary>
        SharedChannelTwoByteAddress = 3,
    }

    /// <summary>
    /// Basic Channel status message codes, the bottom two bits of the received status message.
    /// </summary>
    public enum BasicChannelStatusCode : byte
    {
        /// <summary>Unassigned</summary>
        Unassigned = 0x0,
        /// <summary>Assigned</summary>
        Assigned = 0x1,
        /// <summary>Searching</summary>
        Searching = 0x2,
        /// <summary>Tracking</summary>
        Tracking = 0x3,
    }

    /// <summary>
    /// Contains the information returned by a channel status request message
    /// </summary>
    public struct ChannelStatus
    {
        /// <summary>
        /// Bits 0-1 of the status response
        /// </summary>
        public BasicChannelStatusCode BasicStatus;
        /// <summary>
        /// Bits 2-3 of the status response. Invalid on AP1.
        /// </summary>
        public byte networkNumber;
        /// <summary>
        /// Bits 4-7 of the status response. Not a valid channelType on AP1.
        /// </summary>
        public ChannelType ChannelType;

        /// <summary>
        /// Creates and fills the ChannelStatus
        /// </summary>
        /// <param name="BasicStatus">Channel status code</param>
        /// <param name="networkNumber">ANT network number</param>
        /// <param name="ChannelType">Channel type</param>
        public ChannelStatus(BasicChannelStatusCode BasicStatus, byte networkNumber, ChannelType ChannelType)
        {
            this.BasicStatus = BasicStatus;
            this.networkNumber = networkNumber;
            this.ChannelType = ChannelType;
        }
    }

    /// <summary>The ANT channel interface.</summary>
    public interface IAntChannel : IDisposable
    {
        /// <summary>
        /// The channel response event. Triggered every time a message is received from the ANT device.
        /// </summary>
        event EventHandler<AntResponse>? ChannelResponse;

        #region ANT Channel Functions

        /// <summary>Gets the channel number.</summary>
        /// <returns>ANT channel number.</returns>
        byte ChannelNumber { get; }

        /// <summary>
        /// Returns current channel status.
        /// Throws exception on timeout.
        /// </summary>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>
        /// Channel status
        /// </returns>
        ChannelStatus RequestStatus(UInt32 responseWaitTime);

        /// <summary>Requests the channel identifier.</summary>
        /// <param name="responseWaitTime">The response wait time.</param>
        /// <returns>The <see cref="ChannelId"/>.</returns>
        ChannelId RequestChannelID(uint responseWaitTime);

        /// <overloads>Assign channel</overloads>
        /// <summary>
        /// Assign an ANT channel along with its main parameters.
        /// Throws exception if the network number is invalid.
        /// </summary>
        /// <param name="channelTypeByte">Channel Type byte</param>
        /// <param name="networkNumber">Network to assign to channel, must be less than device's max networks-1</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool AssignChannel(ChannelType channelTypeByte, byte networkNumber, UInt32 responseWaitTime);

        /// <overloads>Assign channel (extended)</overloads>
        /// <summary>
        /// Assign an ANT channel, using extended channel assignment
        /// Throws exception if the network number is invalid.
        /// </summary>
        /// <param name="channelTypeByte">Channel Type byte</param>
        /// <param name="networkNumber">Network to assign to channel, must be less than device's max networks - 1</param>
        /// <param name="extAssignByte">Extended assignment byte</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool AssignChannelExt(ChannelType channelTypeByte, byte networkNumber, ChannelTypeExtended extAssignByte, UInt32 responseWaitTime);

        /// <overloads>Unassign channel</overloads>
        /// <summary>
        /// Unassign this channel.
        /// </summary>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool UnassignChannel(UInt32 responseWaitTime);

        /// <summary>Set the Channel ID of this channel.
        /// Throws exception if device type is &gt; 127.</summary>
        /// <param name="channelId">Channel ID class</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool SetChannelID(ChannelId channelId, uint responseWaitTime);

        /// <overloads>Sets the Channel ID, using serial number as device number</overloads>
        /// <summary>
        /// Identical to setChannelID, except last two bytes of serial number are used for device number.
        /// Not available on all ANT devices.
        /// Throws exception if device type is > 127.
        /// </summary>
        /// <param name="channelId">Channel ID class</param>
        /// <param name="waitResponseTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool SetChannelID_UsingSerial(ChannelId channelId, UInt32 waitResponseTime);

        /// <overloads>Sets channel message period</overloads>
        /// <summary>
        /// Set this channel's messaging period
        /// </summary>
        /// <param name="messagePeriod">Desired period in seconds * 32768</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool SetChannelPeriod(UInt16 messagePeriod, UInt32 responseWaitTime);

        /// <overloads>Sets the RSSI threshold (ARCT)</overloads>
        /// <summary>
        /// Set this channel's RSSI threshold (ARCT)
        /// </summary>
        /// <param name="thresholdRSSI">Desired RSSI threshold value</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool SetSearchThresholdRSSI(byte thresholdRSSI, UInt32 responseWaitTime);

        /// <overloads>Sets channel RF Frequency</overloads>
        /// <summary>
        /// Set this channel's RF frequency, with the given offset from 2400Mhz.
        /// Note: Changing this frequency may affect the ability to certify the product in certain areas of the world.
        /// </summary>
        /// <param name="RFFreqOffset">Offset to add to 2400Mhz</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool SetChannelFreq(byte RFFreqOffset, UInt32 responseWaitTime);

        /// <overloads>Sets the channel transmission power</overloads>
        /// <summary>
        /// Set the transmission power of this channel
        /// Throws exception if device is not capable of per-channel transmit power.
        /// </summary>
        /// <param name="transmitPower">Transmission power to set to</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool SetChannelTransmitPower(TransmitPower transmitPower, UInt32 responseWaitTime);

        /// <overloads>Sets the channel search timeout</overloads>
        /// <summary>
        /// Set the search timeout
        /// </summary>
        /// <param name="searchTimeout">timeout in 2.5 second units (in newer devices 255=infinite)</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool SetChannelSearchTimeout(byte searchTimeout, UInt32 responseWaitTime);

        /// <overloads>Opens the channel</overloads>
        /// <summary>
        /// Opens this channel
        /// </summary>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool OpenChannel(UInt32 responseWaitTime);

        /// <overloads>Sends broadcast message</overloads>
        /// <summary>
        /// Sends the given data on the broadcast transmission.
        /// Throws exception if data > 8-bytes in length
        /// </summary>
        /// <param name="data">data to send (length 8 or less)</param>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SendBroadcastData(byte[] data);

        /// <overloads>Sends burst transfer</overloads>
        /// <summary>
        /// Sends the given data as a burst transmission. Returns: 0=fail, 1=pass, 2=timeout, 3=cancelled
        /// </summary>
        /// <param name="data">data to send, can be any length</param>
        /// <param name="completeWaitTime">Time in ms to wait for completion of transfer</param>
        /// <returns>0=fail, 1=pass, 2=timeout, 3=cancelled</returns>
        MessagingReturnCode SendBurstTransfer(byte[] data, UInt32 completeWaitTime);

        /// <overloads>Sends burst transfer</overloads>
        /// <summary>
        /// Sends the given data as a burst transmission. Returns: 0=fail, 1=pass, 2=timeout, 3=cancelled
        /// </summary>
        /// <param name="data">data to send, can be any length</param>
        /// <param name="completeWaitTime">Time in ms to wait for completion of transfer</param>
        /// <returns>0=fail, 1=pass, 2=timeout, 3=cancelled</returns>
        Task<MessagingReturnCode> SendBurstTransferAsync(byte[] data, UInt32 completeWaitTime);

        /// <summary>Sends the given data as an extended broadcast transmission.
        /// Throws exception if data &gt; 8-bytes in length</summary>
        /// <param name="channelId">The channel ID of the ANT device</param>
        /// <param name="data">data to send (length 8 or less)</param>
        /// <overloads>Sends extended broadcast message</overloads>
        /// <returns>
        /// true if successful
        /// </returns>
        bool SendExtBroadcastData(ChannelId channelId, byte[] data);

        /// <summary>
        /// Sends the given data as an acknowledged transmission.
        /// Throws exception if data > 8-bytes in length
        /// </summary>
        /// <param name="data">data to send (length 8 or less)</param>
        /// <param name="ackWaitTime">Time in ms to wait for acknowledgement</param>
        /// <returns>0=fail, 1=pass, 2=timeout, 3=cancelled</returns>
        MessagingReturnCode SendAcknowledgedData(byte[] data, UInt32 ackWaitTime);

        /// <summary>
        /// Sends the given data as an acknowledged transmission.
        /// Throws exception if data > 8-bytes in length
        /// </summary>
        /// <param name="data">data to send (length 8 or less)</param>
        /// <param name="ackWaitTime">Time in ms to wait for acknowledgement</param>
        /// <returns>0=fail, 1=pass, 2=timeout, 3=cancelled</returns>
        Task<MessagingReturnCode> SendAcknowledgedDataAsync(byte[] data, UInt32 ackWaitTime);

        /// <summary>Sends the given data as an extended acknowledged transmission.
        /// Throws exception if data &gt; 8-bytes in length</summary>
        /// <param name="channelId">Channel ID assigned to a device</param>
        /// <param name="data">Data to send (length 8 or less)</param>
        /// <param name="ackWaitTime">Time in ms to wait for acknowledgement</param>
        /// <returns>0=fail, 1=pass, 2=timeout, 3=cancelled</returns>
        MessagingReturnCode SendExtAcknowledgedData(ChannelId channelId, byte[] data, uint ackWaitTime);

        /// <summary>Sends the given data as an extended acknowledged transmission.
        /// Throws exception if data &gt; 8-bytes in length</summary>
        /// <param name="channelId">Channel ID assigned to a device</param>
        /// <param name="data">Data to send (length 8 or less)</param>
        /// <param name="ackWaitTime">Time in ms to wait for acknowledgement</param>
        /// <returns>0=fail, 1=pass, 2=timeout, 3=cancelled</returns>
        Task<MessagingReturnCode> SendExtAcknowledgedDataAsync(ChannelId channelId, byte[] data, uint ackWaitTime);

        /// <summary>Sends the given data as an extended burst transmission. Returns: 0=fail, 1=pass, 2=timeout, 3=cancelled</summary>
        /// <param name="channelId">Channel ID assigned to a device</param>
        /// <param name="data">data to send, can be any length</param>
        /// <param name="completeWaitTime">Time in ms to wait for completion of transfer</param>
        /// <returns>0=fail, 1=pass, 2=timeout, 3=cancelled</returns>
        /// <overloads>Sends extended burst data</overloads>
        MessagingReturnCode SendExtBurstTransfer(ChannelId channelId, byte[] data, UInt32 completeWaitTime);

        /// <summary>Sends the given data as an extended burst transmission. Returns: 0=fail, 1=pass, 2=timeout, 3=cancelled</summary>
        /// <param name="channelId">Channel ID assigned to a device</param>
        /// <param name="data">data to send, can be any length</param>
        /// <param name="completeWaitTime">Time in ms to wait for completion of transfer</param>
        /// <returns>0=fail, 1=pass, 2=timeout, 3=cancelled</returns>
        /// <overloads>Sends extended burst data</overloads>
        Task<MessagingReturnCode> SendExtBurstTransferAsync(ChannelId channelId, byte[] data, UInt32 completeWaitTime);

        /// <overloads>Closes the channel</overloads>
        /// <summary>
        /// Close this channel
        /// </summary>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool CloseChannel(UInt32 responseWaitTime);

        /// <overloads>Sets the channel low priority search timeout</overloads>
        /// <summary>
        /// Sets the search timeout for the channel's low-priority search, where it will not interrupt other open channels.
        /// When this period expires the channel will drop to high-priority search.
        /// This feature is not available in all ANT devices.
        /// </summary>
        /// <param name="lowPriorityTimeout">Timeout period in 2.5 second units</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool SetLowPrioritySearchTimeout(byte lowPriorityTimeout, UInt32 responseWaitTime);

        /// <summary>
        /// Add the given channel ID to the channel's inclusion/exclusion list.
        /// The channelID is then included or excluded from the wild card search depending on how the list is configured.
        /// Throws exception if listIndex &gt; 3.
        /// </summary>
        /// <param name="channelId">Channel ID assigned to a device</param>
        /// <param name="listIndex">position in inclusion/exclusion list to add channelID at (Max size of list is 4)</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        /// <overloads>Adds a channel ID to the device inclusion/exclusion list</overloads>
        bool IncludeExcludeListAddChannel(ChannelId channelId, byte listIndex, UInt32 responseWaitTime);

        /// <overloads>Configures the device inclusion/exclusion list</overloads>
        /// <summary>
        /// Configures the inclusion/exclusion list. If isExclusionList is true the channel IDs will be
        /// excluded from any wild card search on this channel. Otherwise the IDs are the only IDs accepted in the search.
        /// Throws exception if list size is greater than 4.
        /// </summary>
        /// <param name="listSize">The desired size of the list, max size is 4, 0=none</param>
        /// <param name="isExclusionList">True = exclusion list, False = inclusion list</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool IncludeExcludeListConfigure(byte listSize, bool isExclusionList, UInt32 responseWaitTime);

        /// <overloads>Configures proximity search</overloads>
        /// <summary>
        /// Enables a one time proximity requirement for searching.  Only ANT devices within the set proximity bin can be acquired.
        /// Search threshold values are not correlated to specific distances as this will be dependent on the system design.
        /// This feature is not available on all ANT devices.
        /// Throws exception if given bin value is > 10.
        /// </summary>
        /// <param name="thresholdBin">Threshold bin. Value from 0-10 (0= disabled). A search threshold value of 1 (i.e. bin 1) will yield the smallest radius search and is generally recommended as there is less chance of connecting to the wrong device. </param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool SetProximitySearch(byte thresholdBin, UInt32 responseWaitTime);

        /// <overloads>Configures the three operating RF frequencies for ANT frequency agility mode</overloads>
        /// <summary>
        /// This function configures the three operating RF frequencies for ANT frequency agility mode
        /// and should be used with the AdvFrequencyAgility extended channel assignment flag.
        /// Should not be used with shared, or Tx/Rx only channel types.
        /// This feature is not available on all ANT devices.
        /// </summary>
        /// <param name="freq1">Operating RF frequency 1</param>
        /// <param name="freq2">Operating RF frequency 2</param>
        /// <param name="freq3">Operating RF frequency 3</param>
        /// <param name="responseWaitTime">Time to wait for device success response</param>
        /// <returns>True on success. Note: Always returns true with a response time of 0</returns>
        bool ConfigFrequencyAgility(byte freq1, byte freq2, byte freq3, UInt32 responseWaitTime);

        #endregion
    }
}
