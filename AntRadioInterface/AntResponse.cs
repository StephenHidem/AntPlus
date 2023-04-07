using System;
using System.Runtime.Serialization;

namespace SmallEarthTech.AntRadioInterface
{
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
        /// <summary>The channel event or response</summary>
        ChannelEventOrResponse = 0x40,
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

    /// <summary>Derive from AntResponse to support a concrete implementation.</summary>
    [DataContract(Name = "AntResponse", Namespace = "http://www.smallearthtech.com")]
    public class AntResponse
    {
        /// <summary>
        /// The object that created this response (ie: The corresponding ANTChannel or ANTDevice instance).
        /// </summary>
        public object Sender { get; protected set; }

        /// <summary>
        /// The channel parameter received in the message. Note: For some messages this is not applicable.
        /// </summary>
        [DataMember]
        public byte ChannelNumber { get; protected set; }

        /// <summary>
        /// The time the message was received.
        /// </summary>
        [DataMember]
        public DateTime TimeReceived { get; protected set; }

        /// <summary>
        /// The MessageID of the response
        /// </summary>
        [DataMember]
        public byte ResponseId { get; protected set; }

        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
        [DataMember]
        public ChannelId ChannelId { get; protected set; }

        /// <summary>
        /// The data page payload.
        /// </summary>
        [DataMember]
        public byte[] Payload { get; protected set; } = null;

        /// <summary>
        /// Gets the RSSI in dBm.
        /// </summary>
        [DataMember]
        public sbyte Rssi { get; protected set; }

        /// <summary>
        /// Gets the threshold configuration value.
        /// </summary>
        [DataMember]
        public sbyte ThresholdConfigurationValue { get; protected set; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        [DataMember]
        public ushort Timestamp { get; protected set; }
    }
}
