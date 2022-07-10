using DeviceProfiles;
using System;
using System.Linq;

namespace AntPlus
{
    internal class AntDataMessageHandler
    {
        private readonly AntDeviceCollection antDevices = new AntDeviceCollection();

        [Flags]
        private enum FlagByte
        {
            None = 0,
            RxTimestamp = 0x20,
            Rssi = 0x40,
            ChannelId = 0x80,
        }

        public void Parse(byte[] message)
        {
            // validate message length; not null, can get basic info, and length matches (length should be 1 greater because message ID is not included in length)
            if (message == null || message.Length < 3 || message.Length <= message[0])
            {
                return;
            }

            byte channelNumber = message[2];
            byte[] payload = new byte[8];
            uint channelId = 0;
            sbyte rssi = -128;
            sbyte thresholdConfigurationValue = -128;
            ushort timestamp;

            switch ((MessageId)message[1])
            {
                case MessageId.BroadcastData:
                case MessageId.AcknowledgedData:
                    // ensure payload is present
                    if (message.Length < 11)
                    {
                        // invalid message
                        return;
                    }
                    payload = message.Skip(3).Take(8).ToArray();
                    var extendedInfo = message.Skip(12);

                    // check if extended info is present - flag byte byte and at least 2 more bytes
                    if (extendedInfo.Count() >= 3)
                    {
                        FlagByte flags = (FlagByte)extendedInfo.ElementAt(0);
                        extendedInfo = extendedInfo.Skip(1);
                        if (flags.HasFlag(FlagByte.ChannelId) && extendedInfo.Count() >= 4)
                        {
                            channelId = BitConverter.ToUInt32((byte[])extendedInfo, 0);
                            extendedInfo = extendedInfo.Skip(4);
                        }
                        if (flags.HasFlag(FlagByte.Rssi) && extendedInfo.Count() >= 3)
                        {
                            // check measurement type
                            if (extendedInfo.ElementAt(0) == 0x20)
                            {
                                rssi = (sbyte)extendedInfo.ElementAt(1);
                                thresholdConfigurationValue = (sbyte)extendedInfo.ElementAt(2);
                            }
                            extendedInfo = extendedInfo.Skip(3);
                        }
                        if (flags.HasFlag(FlagByte.RxTimestamp) && extendedInfo.Count() >= 2)
                        {
                            timestamp = BitConverter.ToUInt16((byte[])extendedInfo, 0);
                        }
                    }
                    break;
                case MessageId.BurstData:
                    break;
                case MessageId.ExtBroadcastData:
                case MessageId.ExtAcknowledgedData:
                    // verify - legacy data messages are 13 bytes long (channel number + channel ID + payload)
                    if (message[0] == 13)
                    {
                        payload = message.Skip(7).Take(8).ToArray();
                        channelId = BitConverter.ToUInt32(message, 3);
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
                    break;
            }

            AntDevice ant = antDevices.FirstOrDefault(a => a.ChannelId.Id == channelId);
            if (ant == null)
            {
                ant = new UnknownDevice(new ChannelId(channelId));
                antDevices.Add(ant);
            }
            ant.Parse(payload);
        }
    }
}
