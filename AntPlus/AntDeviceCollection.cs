using DeviceProfiles;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AntPlus
{
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


        public void HandleDataMessage(uint channelId, byte[] dataPage)
        {
            AntDevice device;
            lock (collectionLock)
            {
                device = this.FirstOrDefault(ant => ant.ChannelId == channelId);
            }
            if (device == null)
            {
                device = CreateAntDevice(channelId);
                Add(device);
            }
            device.Parse(dataPage);
        }

        private AntDevice CreateAntDevice(uint channelId)
        {
            byte deviceType = (byte)(BitConverter.GetBytes(channelId)[2] & 0x7F);
            switch (deviceType)
            {
                case HeartRate.DeviceClass:
                    return new HeartRate(channelId);
                case BicyclePower.DeviceClass:
                    return new BicyclePower(channelId);
                default:
                    return new UnknownDevice(channelId);
            }
        }
    }
}
