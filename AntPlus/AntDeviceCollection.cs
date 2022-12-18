using AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using AntRadioInterface;
using DeviceProfiles;
using DeviceProfiles.BicyclePower;
using DeviceProfiles.HeartRate;
using System.Collections.ObjectModel;
using System.Linq;

namespace AntPlus
{
    /// <summary>This is a thread safe observable collection of ANT devices.</summary>
    public class AntDeviceCollection : ObservableCollection<AntDevice>
    {
        public object collectionLock = new object();
        private readonly IAntRadio antRadio;
        private readonly IAntChannel channel;

        public AntDeviceCollection(IAntRadio antRadio)
        {
            this.antRadio = antRadio;
            channel = antRadio.GetChannel(0);
            channel.ChannelResponse += Channel_ChannelResponse;
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
                    return new HeartRate(channelId, antRadio.GetChannel(1));
                case BicyclePower.DeviceClass:
                    return new BicyclePower(channelId, antRadio.GetChannel(1));
                case BikeSpeedSensor.DeviceClass:
                    return new BikeSpeedSensor(channelId, antRadio.GetChannel(1));
                case BikeCadenceSensor.DeviceClass:
                    return new BikeCadenceSensor(channelId, antRadio.GetChannel(1));
                case CombinedSpeedAndCadenceSensor.DeviceClass:
                    return new CombinedSpeedAndCadenceSensor(channelId, antRadio.GetChannel(1));
                default:
                    return new UnknownDevice(channelId, antRadio.GetChannel(1));
            }
        }
    }
}
