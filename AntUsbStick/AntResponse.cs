using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;

namespace SmallEarthTech.AntUsbStick
{
    public class AntResponse : IAntResponse
    {
        public object Sender { get; }

        public byte AntChannel { get; }

        public DateTime TimeReceived { get; }

        public byte ResponseId { get; }

        public ChannelId ChannelId { get; }

        public byte[] Payload { get; } = null;
        public sbyte Rssi { get; }
        public sbyte ThresholdConfigurationValue { get; }
        public ushort Timestamp { get; }

        internal AntResponse(ANT_Response response)
        {
            Sender = response.sender;
            AntChannel = response.antChannel;
            TimeReceived = response.timeReceived;
            ResponseId = response.responseID;

            switch ((MessageId)ResponseId)
            {
                case MessageId.BroadcastData:
                case MessageId.AcknowledgedData:
                    // ensure payload is present
                    if (response.messageContents.Length < 11)
                    {
                        // invalid message
                        return;
                    }
                    Payload = response.messageContents.Skip(1).Take(8).ToArray();
                    var extendedInfo = response.messageContents.Skip(9);

                    // check if extended info is present - flag byte byte and at least 2 more bytes
                    if (extendedInfo.Count() >= 3)
                    {
                        FlagByte flags = (FlagByte)extendedInfo.ElementAt(0);
                        extendedInfo = extendedInfo.Skip(1);
                        if (flags.HasFlag(FlagByte.ChannelId) && extendedInfo.Count() >= 4)
                        {
                            ChannelId = new ChannelId(BitConverter.ToUInt32(extendedInfo.ToArray(), 0));
                            extendedInfo = extendedInfo.Skip(4);
                        }
                        if (flags.HasFlag(FlagByte.Rssi) && extendedInfo.Count() >= 4)
                        {
                            // check measurement type
                            if (extendedInfo.ElementAt(0) == 0x20)
                            {
                                Rssi = (sbyte)extendedInfo.ElementAt(1);
                                ThresholdConfigurationValue = (sbyte)extendedInfo.ElementAt(2);
                            }
                            extendedInfo = extendedInfo.Skip(4);
                        }
                        if (flags.HasFlag(FlagByte.RxTimestamp) && extendedInfo.Count() >= 2)
                        {
                            Timestamp = BitConverter.ToUInt16(extendedInfo.ToArray(), 0);
                        }
                    }
                    break;
                case MessageId.BurstData:
                    break;
                case MessageId.ExtBroadcastData:
                case MessageId.ExtAcknowledgedData:
                    // verify - legacy data messages are 13 bytes long (channel number + channel ID + payload)
                    if (response.messageContents[0] == 13)
                    {
                        Payload = response.messageContents.Skip(7).Take(8).ToArray();
                        ChannelId = new ChannelId(BitConverter.ToUInt32(response.messageContents, 3));
                    }
                    else
                    {
                        // invalid message
                        return;
                    }
                    break;
                case MessageId.ExtBurstData:
                    break;
                default:
                    if (ResponseId == 0x40)
                    {
                        Payload = response.messageContents;
                    }
                    break;
            }
        }
    }
}
