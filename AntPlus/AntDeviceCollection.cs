using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntPlus.Extensions.Logging;
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
    /// <remarks>
    /// This class will create an <see cref="AntDevice"/> from one of the supported ANT device classes and add
    /// it to the collection as they are discovered. An important consideration is when the device is no longer
    /// available. You may supply a timeout duration or the number of missed messages. <b><u>You should prefer missed messages</u></b>
    /// as this scales the timeout duration based on the broadcast transmission rate of the particular ANT device.
    /// The timeout/missed messages will be applied globally to ANT devices created by this collection.
    /// </remarks>
    public partial class AntDeviceCollection : ObservableCollection<AntDevice>
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
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<AntDeviceCollection> _logger;
        private readonly int _timeout;
        private IAntChannel[]? _channels;
        private SendMessageChannel? _sendMessageChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AntDeviceCollection" /> class. The ANT radio is configured
        /// for continuous scan mode.
        /// </summary>
        /// <param name="antRadio">The ANT radio interface.</param>
        /// <param name="loggerFactory">Logger factory to generate type specific ILogger from. Can be null.</param>
        /// <param name="antDeviceTimeout">ANT device _timeout in milliseconds. The default is 2000 milliseconds.</param>
        /// <remarks>
        /// The ILoggerFactory is used to create <see cref="ILogger{TCategoryName}"/> instances for discovered ANT devices.
        /// If the factory is null, the <see cref="NullLoggerFactory"/> is used and no logging is generated.
        /// The <see cref="StartScanning"/> method must be called to initialize the ANT radio for continuous scan mode and setup
        /// reception of ANT messages.
        /// </remarks>
        public AntDeviceCollection(IAntRadio antRadio, ILoggerFactory loggerFactory, int antDeviceTimeout = 2000)
        {
            _antRadio = antRadio;
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            _logger = _loggerFactory.CreateLogger<AntDeviceCollection>();
            _timeout = antDeviceTimeout;
        }

        /// <summary>
        /// Initializes the ANT radio for continuous scan mode and setup ANT channels for use by this collection.
        /// </summary>
        /// <returns>Task of type void</returns>
        public async Task StartScanning()
        {
            _logger.LogMethodEntry();
            _channels = await _antRadio.InitializeContinuousScanMode();
            _sendMessageChannel = new SendMessageChannel(_channels.Skip(1).ToArray(), _logger);
            _channels[0].ChannelResponse += MessageHandler;
        }

        private void MessageHandler(object sender, AntResponse e)
        {
            _logger.LogAntResponse(LogLevel.Trace, e);
            if (e.ChannelId != null && e.Payload != null)
            {
                // see if the device is in the collection
                AntDevice device = this.FirstOrDefault(ant => ant.ChannelId.Id == e.ChannelId.Id);

                // create the device if not in the collection
                if (device == null)
                {
                    // create the device and add it to the collection
                    device = CreateAntDevice(e.ChannelId, e.Payload);
                    Add(device);
                    device.DeviceWentOffline += DeviceOffline;
                }
                device.Parse(e.Payload);    // dispatch the message to the device for processing
            }
            else
            {
                _logger.LogAntResponse(LogLevel.Warning, e);
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
                _logger.LogAntCollectionChange(item);
                base.Add(item);
            }
        }

        /// <summary>Removes the specified item from the collection.</summary>
        /// <param name="item">The item.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the original collection.</returns>
        public new bool Remove(AntDevice item)
        {
            lock (CollectionLock)
            {
                _logger.LogAntCollectionChange(item);
                return base.Remove(item);
            }
        }

        private AntDevice CreateAntDevice(ChannelId channelId, byte[] dataPage)
        {
            return channelId.DeviceType switch
            {
                BicyclePower.DeviceClass => BicyclePower.GetBicyclePowerSensor(dataPage, channelId, _sendMessageChannel!, _loggerFactory, _timeout),
                BikeSpeedSensor.DeviceClass => new BikeSpeedSensor(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<BikeSpeedSensor>(), _timeout),
                BikeCadenceSensor.DeviceClass => new BikeCadenceSensor(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<BikeCadenceSensor>(), _timeout),
                CombinedSpeedAndCadenceSensor.DeviceClass => new CombinedSpeedAndCadenceSensor(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<CombinedSpeedAndCadenceSensor>(), _timeout),
                FitnessEquipment.DeviceClass => FitnessEquipment.GetFitnessEquipment(dataPage, channelId, _sendMessageChannel!, _loggerFactory, _timeout) ?? (AntDevice)new UnknownDevice(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<UnknownDevice>(), _timeout),
                MuscleOxygen.DeviceClass => new MuscleOxygen(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<MuscleOxygen>(), _timeout),
                Geocache.DeviceClass => new Geocache(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<Geocache>(), _timeout),
                Tracker.DeviceClass => new Tracker(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<Tracker>(), _timeout),
                StrideBasedSpeedAndDistance.DeviceClass => new StrideBasedSpeedAndDistance(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<StrideBasedSpeedAndDistance>(), _timeout),
                HeartRate.DeviceClass => new HeartRate(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<HeartRate>(), _timeout),
                _ => new UnknownDevice(channelId, _sendMessageChannel!, _loggerFactory.CreateLogger<UnknownDevice>(), _timeout),
            };
        }
    }
}
