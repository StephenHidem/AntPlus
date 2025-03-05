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
        public object CollectionLock = new();

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

        private void MessageHandler(object? sender, AntResponse e)
        {
            _logger.LogAntResponse(LogLevel.Trace, e);

            // check for a valid payload
            if (e.Payload == null || e.Payload.Length < 3)
            {
                _logger.LogUnhandledAntResponse(e);
                return;
            }

            // switch on the response id
            switch (e.ResponseId)
            {
                case MessageId.ChannelEventOrResponse:
                    // check for a RF event on channel 0, channel closed event
                    if (e.Payload[0] != 0 || e.Payload[1] != 1 || (EventMsgId)e.Payload[2] != EventMsgId.ChannelClosed)
                    {
                        _logger.LogUnhandledAntResponse(e);
                        return;
                    }

                    // re-open the channel in scan mode
                    if (_antRadio is IAntControl antControl)
                    {
                        _logger.LogError("Re-opening channel 0 in scan mode.");
                        _ = antControl.OpenRxScanMode();
                    }
                    else
                    {
                        _logger.LogCritical("IAntControl not implemented. Can't re-open channel 0.");
                    }
                    break;
                case MessageId.BroadcastData:
                case MessageId.ExtBroadcastData:
                    // check for a valid channel id and payload length
                    if (e.ChannelId == null || e.Payload.Length != 8)
                    {
                        _logger.LogUnhandledAntResponse(e);
                        return;
                    }

                    // see if the device is in the collection
                    AntDevice device = this.FirstOrDefault(ant => ant.ChannelId.Id == e.ChannelId.Id);

                    // create the device if not in the collection
                    if (device == null)
                    {
                        // create an ANT device from the AntResponse parameter
                        device = CreateAntDevice(e);

                        Add(device);
                        device.DeviceWentOffline += DeviceOffline;
                    }

                    // dispatch the message to the device
                    device.Parse(e.Payload!);
                    break;
                default:
                    _logger.LogUnhandledAntResponse(e);
                    break;
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

        private AntDevice CreateAntDevice(AntResponse response)
        {
            return response.ChannelId!.DeviceType switch
            {
                BicyclePower.DeviceClass => BicyclePower.GetBicyclePowerSensor(response.Payload!, response.ChannelId, _sendMessageChannel!, _loggerFactory, _timeout),
                BikeSpeedSensor.DeviceClass => new BikeSpeedSensor(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<BikeSpeedSensor>(), _timeout),
                BikeCadenceSensor.DeviceClass => new BikeCadenceSensor(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<BikeCadenceSensor>(), _timeout),
                CombinedSpeedAndCadenceSensor.DeviceClass => new CombinedSpeedAndCadenceSensor(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<CombinedSpeedAndCadenceSensor>(), _timeout),
                FitnessEquipment.DeviceClass => FitnessEquipment.GetFitnessEquipment(response.Payload!, response.ChannelId, _sendMessageChannel!, _loggerFactory, _timeout) ?? (AntDevice)new UnknownDevice(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<UnknownDevice>(), _timeout),
                MuscleOxygen.DeviceClass => new MuscleOxygen(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<MuscleOxygen>(), _timeout),
                Geocache.DeviceClass => new Geocache(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<Geocache>(), _timeout),
                Tracker.DeviceClass => new Tracker(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<Tracker>(), _timeout),
                StrideBasedSpeedAndDistance.DeviceClass => new StrideBasedSpeedAndDistance(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<StrideBasedSpeedAndDistance>(), _timeout),
                HeartRate.DeviceClass => new HeartRate(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<HeartRate>(), _timeout),
                _ => new UnknownDevice(response.ChannelId, _sendMessageChannel!, _loggerFactory.CreateLogger<UnknownDevice>(), _timeout),
            };
        }
    }
}
