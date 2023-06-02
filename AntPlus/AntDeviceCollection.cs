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
        private readonly ushort timeout;

        /// <summary>Initializes a new instance of the <see cref="AntDeviceCollection" /> class.</summary>
        /// <param name="antRadio">The ANT radio interface.</param>
        /// <param name="antDeviceTimeout">ANT device timeout in milliseconds. The default is 2000 milliseconds.</param>
        /// <remarks>
        /// The timeout is set on each ANT device added to the collection. When the timeout
        /// expires, the <see cref="AntDevice.DeviceWentOffline"/> event is fired and the device is
        /// removed from the collection. Typically the timeout is set to 2000 milliseconds.
        /// Geocaches are a special case; geocaches broadcast at a 0.5Hz rate
        /// until the PIN page is requested, whereupon they broadcast at 4Hz. The geocache timeout is multiplied by 4.
        /// </remarks>
        public AntDeviceCollection(IAntRadio antRadio, ushort antDeviceTimeout = 2000)
        {
            timeout = antDeviceTimeout;
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
                    device.DeviceWentOffline += DeviceOffline;
                }
            }
            device.Parse(e.Payload);
        }

        private void DeviceOffline(object sender, System.EventArgs e)
        {
            AntDevice device = (AntDevice)sender;
            device.DeviceWentOffline -= DeviceOffline;
            _ = Remove(device);
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
                    return new HeartRate(channelId, channel, timeout);
                case BicyclePower.DeviceClass:
                    return new BicyclePower(channelId, channel, timeout);
                case BikeSpeedSensor.DeviceClass:
                    return new BikeSpeedSensor(channelId, channel, timeout);
                case BikeCadenceSensor.DeviceClass:
                    return new BikeCadenceSensor(channelId, channel, timeout);
                case CombinedSpeedAndCadenceSensor.DeviceClass:
                    return new CombinedSpeedAndCadenceSensor(channelId, channel, timeout);
                case FitnessEquipment.DeviceClass:
                    return new FitnessEquipment(channelId, channel, timeout);
                case MuscleOxygen.DeviceClass:
                    return new MuscleOxygen(channelId, channel, timeout);
                case Geocache.DeviceClass:
                    return new Geocache(channelId, channel, timeout * 4);
                case AssetTracker.DeviceClass:
                    return new AssetTracker(channelId, channel, timeout);
                case StrideBasedSpeedAndDistance.DeviceClass:
                    return new StrideBasedSpeedAndDistance(channelId, channel, timeout);
                default:
                    return new UnknownDevice(channelId, channel, timeout);
            }
        }
    }
}
