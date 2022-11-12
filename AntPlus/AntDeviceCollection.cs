using AntRadioInterface;
using DeviceProfiles;
using System.Collections.ObjectModel;
using System.Linq;

namespace AntPlus
{
    /// <summary>This is a thread safe observable collection of ANT devices.</summary>
    public class AntDeviceCollection : ObservableCollection<AntDevice>
    {
        public object collectionLock = new object();
        private readonly IAntChannel _channel;

        public AntDeviceCollection(IAntChannel channel)
        {
            _channel = channel;
            _channel.ChannelResponse += Channel_ChannelResponse;
        }

        private void Channel_ChannelResponse(object sender, IAntResponse e)
        {
            AntDevice device;
            lock (collectionLock)
            {
                device = this.FirstOrDefault(ant => ant.ChannelId.Id == e.ChannelId.Id);
                if (device == null)
                {
                    device = CreateAntDevice(e.ChannelId);
                    Add(device);
                }
            }
            device.Parse(e.Payload);
        }

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

        private AntDevice CreateAntDevice(ChannelId channelId)
        {
            switch (channelId.DeviceType)
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
