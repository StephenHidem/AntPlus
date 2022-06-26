using DeviceProfiles;
using System;
using System.Collections.ObjectModel;
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

    /// <summary>This is a thread safe observable collection of ANT devices.</summary>
    public class AntDeviceCollection : ObservableCollection<AntDevice>
    {
        private object collectionLock = new object();

        protected override void ClearItems()
        {
            lock (collectionLock)
            {
                base.ClearItems();
            }
        }

        protected override void InsertItem(int index, AntDevice item)
        {
            lock (collectionLock)
            {
                base.InsertItem(index, item);
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            lock (collectionLock)
            {
                base.MoveItem(oldIndex, newIndex);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (collectionLock)
            {
                base.RemoveItem(index);
            }
        }

        protected override void SetItem(int index, AntDevice item)
        {
            lock (collectionLock)
            {
                base.SetItem(index, item);
            }
        }
    }
}
