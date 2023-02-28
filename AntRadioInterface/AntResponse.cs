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

        //public AntResponse(ANT_Response response)
        //{
        //    Sender = response.sender;
        //    ChannelNumber = response.antChannel;
        //    TimeReceived = response.timeReceived;
        //    ResponseId = response.responseID;

        //    switch ((MessageId)ResponseId)
        //    {
        //        case MessageId.BroadcastData:
        //        case MessageId.AcknowledgedData:
        //            // ensure payload is present
        //            if (response.messageContents.Length < 11)
        //            {
        //                // invalid message
        //                return;
        //            }
        //            Payload = response.messageContents.Skip(1).Take(8).ToArray();
        //            var extendedInfo = response.messageContents.Skip(9);

        //            // check if extended info is present - flag byte byte and at least 2 more bytes
        //            if (extendedInfo.Count() >= 3)
        //            {
        //                FlagByte flags = (FlagByte)extendedInfo.ElementAt(0);
        //                extendedInfo = extendedInfo.Skip(1);
        //                if (flags.HasFlag(FlagByte.ChannelId) && extendedInfo.Count() >= 4)
        //                {
        //                    ChannelId = new ChannelId(BitConverter.ToUInt32(extendedInfo.ToArray(), 0));
        //                    extendedInfo = extendedInfo.Skip(4);
        //                }
        //                if (flags.HasFlag(FlagByte.Rssi) && extendedInfo.Count() >= 4)
        //                {
        //                    // check measurement type
        //                    if (extendedInfo.ElementAt(0) == 0x20)
        //                    {
        //                        Rssi = (sbyte)extendedInfo.ElementAt(1);
        //                        ThresholdConfigurationValue = (sbyte)extendedInfo.ElementAt(2);
        //                    }
        //                    extendedInfo = extendedInfo.Skip(4);
        //                }
        //                if (flags.HasFlag(FlagByte.RxTimestamp) && extendedInfo.Count() >= 2)
        //                {
        //                    Timestamp = BitConverter.ToUInt16(extendedInfo.ToArray(), 0);
        //                }
        //            }
        //            break;
        //        case MessageId.BurstData:
        //            break;
        //        case MessageId.ExtBroadcastData:
        //        case MessageId.ExtAcknowledgedData:
        //            // verify - legacy data messages are 13 bytes long (channel number + channel ID + payload)
        //            if (response.messageContents[0] == 13)
        //            {
        //                Payload = response.messageContents.Skip(7).Take(8).ToArray();
        //                ChannelId = new ChannelId(BitConverter.ToUInt32(response.messageContents, 3));
        //            }
        //            else
        //            {
        //                // invalid message
        //                return;
        //            }
        //            break;
        //        case MessageId.ExtBurstData:
        //            break;
        //        default:
        //            if (ResponseId == 0x40)
        //            {
        //                Payload = response.messageContents;
        //            }
        //            break;
        //    }
        //}
    }
}
