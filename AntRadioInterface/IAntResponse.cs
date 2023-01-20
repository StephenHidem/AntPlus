using System;

namespace SmallEarthTech.AntRadioInterface
{
    /// <summary>The ANT radio response interface.</summary>
    public interface IAntResponse
    {
        /// <summary>
        /// The object that created this response (ie: The corresponding ANTChannel or ANTDevice instance).
        /// </summary>
        object Sender { get; }

        /// <summary>
        /// The channel parameter received in the message. Note: For some messages this is not applicable.
        /// </summary>
        byte AntChannel { get; }

        /// <summary>
        /// The time the message was received.
        /// </summary>
        DateTime TimeReceived { get; }

        /// <summary>
        /// The MessageID of the response
        /// </summary>
        byte ResponseId { get; }

        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        ChannelId ChannelId { get; }
        /// <summary>
        /// The raw contents of the response message
        /// </summary>
        byte[] Payload { get; }
        /// <summary>Gets the RSSI in dBm.</summary>
        /// <value>The rssi.</value>
        sbyte Rssi { get; }
        /// <summary>Gets the threshold configuration value.</summary>
        /// <value>The threshold configuration value.</value>
        sbyte ThresholdConfigurationValue { get; }
        /// <summary>Gets the timestamp.</summary>
        /// <value>The timestamp.</value>
        ushort Timestamp { get; }
    }
}