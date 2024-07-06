using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly IAntRadio _antRadio;
        private int channelNum = 1;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<AntDeviceCollection> logger;
        private readonly ushort timeout;
        private IAntChannel[] channels;

        /// <summary>
        /// Initializes a new instance of the <see cref="AntDeviceCollection" /> class. The ANT radio is configured
        /// for continuous scan mode.
        /// </summary>
        /// <param name="antRadio">The ANT radio interface.</param>
        /// <param name="loggerFactory">Logger factory to generate type specific ILogger from. Can be null.</param>
        /// <param name="antDeviceTimeout">ANT device timeout in milliseconds. The default is 2000 milliseconds.</param>
        /// <remarks>
        /// The ILoggerFactory is used to create <see cref="ILogger{TCategoryName}"/> instances for discovered ANT devices.
        /// If the factory is null, the <see cref="NullLoggerFactory"/> is used and no logging is generated.
        /// <para>
        /// The timeout is set on each ANT device added to the collection. When the timeout
        /// expires, the <see cref="AntDevice.DeviceWentOffline"/> event is fired and the device is
        /// removed from the collection. Typically the timeout is set to 2000 milliseconds.
        /// </para>
        /// Geocaches are a special case; geocaches broadcast at a 0.5Hz rate
        /// until the PIN page is requested, whereupon they broadcast at 4Hz. The geocache timeout is multiplied by 4.
        /// </remarks>
        public AntDeviceCollection(IAntRadio antRadio, ILoggerFactory loggerFactory, ushort antDeviceTimeout = 2000)
        {
            _antRadio = antRadio;
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            logger = _loggerFactory.CreateLogger<AntDeviceCollection>();
            logger.LogInformation("Created AntDeviceCollection");
            timeout = antDeviceTimeout;
            Task.Run(async () =>
            {
                channels = await antRadio.InitializeContinuousScanMode();
                channels[0].ChannelResponse += Channel_ChannelResponse;
            });
        }

        private async void Channel_ChannelResponse(object sender, AntResponse e)
        {
            if (e.ChannelId != null)
            {
                AntDevice device;
                device = this.FirstOrDefault(ant => ant.ChannelId.Id == e.ChannelId.Id);
                if (device == null)
                {
                    // provide channel ID and payload
                    device = CreateAntDevice(e.ChannelId, e.Payload);
                    if (device == null) return;     // some device types have additional qualifiers
                    Add(device);
                    device.DeviceWentOffline += DeviceOffline;
                }
                device.Parse(e.Payload);
            }
            else
            {
                logger.LogCritical("ChannelId is null. Channel # = {ChannelNum}, Response ID = {Response}, Payload = {Payload}.",
                    e.ChannelNumber,
                    (MessageId)e.ResponseId,
                    e.Payload != null ? BitConverter.ToString(e.Payload) : "Null");

                // attempt to reopen channel 0 for Rx continuous scan mode
                _ = await _antRadio.InitializeContinuousScanMode();
            }
        }

        private void DeviceOffline(object sender, EventArgs e)
        {
            AntDevice device = (AntDevice)sender;
            device.DeviceWentOffline -= DeviceOffline;
            _ = Remove(device);
        }

        /// <summary>Adds the specified item to the end of the collection.</summary>
        /// <param name="item">The item.</param>
        public new void Add(AntDevice item)
        {
            lock (CollectionLock)
            {
                base.Add(item);
                logger.LogDebug("{Device} added.", item);
            }
        }

        /// <summary>Removes the specified item from the collection.</summary>
        /// <param name="item">The item.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the original collection.</returns>
        public new bool Remove(AntDevice item)
        {
            lock (CollectionLock)
            {
                bool result = base.Remove(item);
                logger.LogDebug("{Device} remove.Result = {Result}", item, result);
                return result;
            }
        }

        private AntDevice CreateAntDevice(ChannelId channelId, byte[] dataPage)
        {
            IAntChannel channel = channels[channelNum++];
            if (channelNum == channels.Length) { channelNum = 1; }

            switch (channelId.DeviceType)
            {
                case HeartRate.DeviceClass:
                    return new HeartRate(channelId, channel, _loggerFactory.CreateLogger<HeartRate>(), timeout);
                case BicyclePower.DeviceClass:
                    return BicyclePower.GetBicyclePowerSensor(dataPage, channelId, channel, _loggerFactory.CreateLogger<BicyclePower>(), timeout);
                case BikeSpeedSensor.DeviceClass:
                    return new BikeSpeedSensor(channelId, channel, _loggerFactory.CreateLogger<BikeSpeedSensor>(), timeout);
                case BikeCadenceSensor.DeviceClass:
                    return new BikeCadenceSensor(channelId, channel, _loggerFactory.CreateLogger<BikeCadenceSensor>(), timeout);
                case CombinedSpeedAndCadenceSensor.DeviceClass:
                    return new CombinedSpeedAndCadenceSensor(channelId, channel, _loggerFactory.CreateLogger<CombinedSpeedAndCadenceSensor>(), timeout);
                case Equipment.DeviceClass:
                    return Equipment.GetEquipment(dataPage, channelId, channel, _loggerFactory.CreateLogger<Equipment>(), timeout);
                case MuscleOxygen.DeviceClass:
                    return new MuscleOxygen(channelId, channel, _loggerFactory.CreateLogger<MuscleOxygen>(), timeout);
                case Geocache.DeviceClass:
                    return new Geocache(channelId, channel, _loggerFactory.CreateLogger<Geocache>(), timeout * 4);
                case Tracker.DeviceClass:
                    return new Tracker(channelId, channel, _loggerFactory.CreateLogger<Tracker>(), timeout);
                case StrideBasedSpeedAndDistance.DeviceClass:
                    return new StrideBasedSpeedAndDistance(channelId, channel, _loggerFactory.CreateLogger<StrideBasedSpeedAndDistance>(), timeout);
                default:
                    return new UnknownDevice(channelId, channel, _loggerFactory.CreateLogger<UnknownDevice>(), timeout);
            }
        }
    }
}
