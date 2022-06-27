using DeviceProfiles;
using System;
using System.Linq;

namespace AntPlus
{
    internal class AntDataMessageHandler
    {
        private readonly AntDeviceCollection antDevices = new AntDeviceCollection();

        public void Parse(byte[] message)
        {
            byte length = message[0];
            byte channelNumber = message[2];
            byte[] payload = new byte[8];
            uint channelId = 0;

            switch ((MessageId)message[1])
            {
                case MessageId.BroadcastData:
                case MessageId.AcknowledgedData:
                    payload = message.Skip(3).Take(8).ToArray();
                    channelId = BitConverter.ToUInt32(message, 12);
                    break;
                case MessageId.BurstData:
                    break;
                case MessageId.ExtBroadcastData:
                case MessageId.ExtAcknowledgedData:
                    payload = message.Skip(7).Take(8).ToArray();
                    channelId = BitConverter.ToUInt32(message, 3);
                    break;
                case MessageId.ExtBurstData:
                    break;
                default:
                    break;
            }

            AntDevice ant = antDevices.FirstOrDefault(a => a.ChannelId == channelId);
            if (ant == null)
            {
                ant = new UnknownDevice(channelId);
                antDevices.Add(ant);
            }
            ant.Parse(payload);
        }
    }
}
