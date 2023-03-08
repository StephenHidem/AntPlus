using System;
using System.Runtime.Serialization;

namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>The ANT response is a concrete class.</summary>
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
