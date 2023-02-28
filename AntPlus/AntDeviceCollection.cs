using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntPlus.DeviceProfiles.Geocache;
using SmallEarthTech.AntPlus.DeviceProfiles.HeartRate;
using SmallEarthTech.AntPlus.DeviceProfiles.MuscleOxygen;
using SmallEarthTech.AntPlus.DeviceProfiles.StrideBasedSpeedAndDistance;
using SmallEarthTech.AntPlus.DeviceProfiles.UnknownDevice;
using SmallEarthTech.AntRadioInterface;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmallEarthTech.AntPlus
{
    /// <summary>
    /// This is a thread safe observable collection of ANT devices.
    /// </summary>
    public class AntDeviceCollection : ObservableCollection<AntDevice>
    {
        /// <summary>
        /// The collection lock.
        /// </summary>
        /// <remarks>
        /// An application should use the collection lock to ensure thread safe access to the
        /// collection. For example, the code behind for a WPF window should include -
        /// <code>BindingOperations.EnableCollectionSynchronization(App.AntDevices, App.AntDevices.CollectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
        public object CollectionLock = new object();
        private readonly IAntChannel channel;

        /// <summary>Initializes a new instance of the <see cref="AntDeviceCollection" /> class.</summary>
        /// <param name="antRadio">The ANT radio interface.</param>
        public AntDeviceCollection(IAntRadio antRadio)
        {
            antRadio.GetChannel(0).ChannelResponse += Channel_ChannelResponse;
            channel = antRadio.GetChannel(1);
        }

        private void Channel_ChannelResponse(object sender, AntResponse e)
        {
            AntDevice device;
            lock (CollectionLock)
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

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            lock (CollectionLock)
            {
                base.ClearItems();
            }
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, AntDevice item)
        {
            lock (CollectionLock)
            {
                base.InsertItem(index, item);
            }
        }

        /// <inheritdoc/>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            lock (CollectionLock)
            {
                base.MoveItem(oldIndex, newIndex);
            }
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            lock (CollectionLock)
            {
                base.RemoveItem(index);
            }
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, AntDevice item)
        {
            lock (CollectionLock)
            {
                base.SetItem(index, item);
            }
        }

        private AntDevice CreateAntDevice(ChannelId channelId)
        {
            switch (channelId.DeviceType)
            {
                case HeartRate.DeviceClass:
                    return new HeartRate(channelId, channel);
                case BicyclePower.DeviceClass:
                    return new BicyclePower(channelId, channel);
                case BikeSpeedSensor.DeviceClass:
                    return new BikeSpeedSensor(channelId, channel);
                case BikeCadenceSensor.DeviceClass:
                    return new BikeCadenceSensor(channelId, channel);
                case CombinedSpeedAndCadenceSensor.DeviceClass:
                    return new CombinedSpeedAndCadenceSensor(channelId, channel);
                case FitnessEquipment.DeviceClass:
                    return new FitnessEquipment(channelId, channel);
                case MuscleOxygen.DeviceClass:
                    return new MuscleOxygen(channelId, channel);
                case Geocache.DeviceClass:
                    return new Geocache(channelId, channel);
                case AssetTracker.DeviceClass:
                    return new AssetTracker(channelId, channel);
                case StrideBasedSpeedAndDistance.DeviceClass:
                    return new StrideBasedSpeedAndDistance(channelId, channel);
                default:
                    return new UnknownDevice(channelId, channel);
            }
        }
    }
}
