using System;

namespace AntRadioInterface
{
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

        /// <summary>
        /// The raw contents of the response message
        /// </summary>
        ChannelId ChannelId { get; }
        byte[] Payload { get; }
        sbyte Rssi { get; }
        sbyte ThresholdConfigurationValue { get; }
        ushort Timestamp { get; }
    }
}