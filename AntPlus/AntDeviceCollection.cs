using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntPlus.DeviceProfiles.Geocache;
using SmallEarthTech.AntPlus.DeviceProfiles.HeartRate;
using SmallEarthTech.AntPlus.DeviceProfiles.MuscleOxygen;
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
        /// <code>BindingOperations.EnableCollectionSynchronization(App.AntDevices, App.AntDevices.collectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
        public object collectionLock = new object();
        private readonly IAntRadio antRadio;
        private readonly IAntChannel channel;

        /// <summary>Initializes a new instance of the <see cref="AntDeviceCollection" /> class.</summary>
        /// <param name="antRadio">The ANT radio interface.</param>
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

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            lock (collectionLock)
            {
                base.ClearItems();
            }
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, AntDevice item)
        {
            lock (collectionLock)
            {
                base.InsertItem(index, item);
            }
        }

        /// <inheritdoc/>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            lock (collectionLock)
            {
                base.MoveItem(oldIndex, newIndex);
            }
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            lock (collectionLock)
            {
                base.RemoveItem(index);
            }
        }

        /// <inheritdoc/>
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
                case FitnessEquipment.DeviceClass:
                    return new FitnessEquipment(channelId, antRadio.GetChannel(1));
                case MuscleOxygen.DeviceClass:
                    return new MuscleOxygen(channelId, antRadio.GetChannel(1));
                case Geocache.DeviceClass:
                    return new Geocache(channelId, antRadio.GetChannel(1));
                case AssetTracker.DeviceClass:
                    return new AssetTracker(channelId, antRadio.GetChannel(1));
                default:
                    return new UnknownDevice(channelId, antRadio.GetChannel(1));
            }
        }
    }
}
