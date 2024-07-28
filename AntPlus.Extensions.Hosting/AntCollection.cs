using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment;
using static SmallEarthTech.AntPlus.Extensions.Hosting.HostBuilderExtensions;

namespace SmallEarthTech.AntPlus.Extensions.Hosting
{
    /// <summary>
    /// This class largely mirrors <see cref="AntDeviceCollection"/> but is designed specifically to be integrated into
    /// an application via dependency injection.
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection&lt;SmallEarthTech.AntPlus.AntDevice&gt;" />
    public partial class AntCollection : ObservableCollection<AntDevice>
    {
        /// <summary>
        /// The collection lock typically used by WPF applications to synchronize UI updates when devices are added or
        /// removed from the collection. For example, in the code behind for a window that is using this collection
        /// would include the following line in its constructor -
        /// <code>BindingOperations.EnableCollectionSynchronization(viewModel.AntDevices, viewModel.AntDevices.CollectionLock);</code>
        /// </summary>
        public object CollectionLock = new object();

        private readonly IServiceProvider _services;
        private readonly ILogger<AntCollection> _logger;
        private readonly int? _timeout;
        private readonly byte? _missedMessages;

        private IAntChannel[]? _channels;
        private SendMessageChannel? _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AntCollection"/> class.
        /// </summary>
        /// <param name="services">The service provider.</param>
        /// <param name="antRadio">The ANT radio.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The timeout options.</param>
        public AntCollection(IServiceProvider services, IAntRadio antRadio, ILogger<AntCollection> logger, IOptions<TimeoutOptions> options)
        {
            _services = services;
            _logger = logger;
            _timeout = options.Value.Timeout;
            _missedMessages = options.Value.MissedMessages;

            // gRPC clients need to run asynchronously
            Task.Run(async () =>
            {
                _channels = await antRadio.InitializeContinuousScanMode();
                _channel = new SendMessageChannel(_channels.Skip(1).ToArray(), _logger);
                _channels[0].ChannelResponse += Channel_ChannelResponse;
            });

            _logger.LogInformation("Created AntDeviceCollection: Timeout = {Timeout} MissedMessages = {MissedMessages}", _timeout, _missedMessages);
        }

        private void Channel_ChannelResponse(object? sender, AntResponse e)
        {
            if (e.ChannelId != null)
            {
                AntDevice? device;
                device = this.FirstOrDefault(ant => ant.ChannelId.Id == e.ChannelId.Id);
                if (device == null)
                {
                    // get the device type
                    Type? deviceType = GetAntDeviceType(e.ChannelId, e.Payload!);
                    if (deviceType == null) return;

                    // create ant device from services
                    if (_missedMessages != null)
                    {
                        device = (AntDevice?)ActivatorUtilities.CreateInstance(_services, deviceType, e.ChannelId, _channel!, _missedMessages);
                    }
                    else if (_timeout != null)
                    {
                        device = (AntDevice?)ActivatorUtilities.CreateInstance(_services, deviceType, e.ChannelId, _channel!, _timeout);
                    }
                    else
                    {
                        device = (AntDevice?)ActivatorUtilities.CreateInstance(_services, deviceType, e.ChannelId, _channel!);
                    }

                    if (device == null) return;     // some device types have additional qualifiers
                    Add(device);
                    device.DeviceWentOffline += DeviceOffline;
                }
                device.Parse(e.Payload);
            }
            else
            {
                _logger.LogCritical("ChannelId is null. Channel # = {ChannelNum}, Response ID = {Response}, Payload = {Payload}.",
                    e.ChannelNumber,
                    (MessageId)e.ResponseId,
                    e.Payload != null ? BitConverter.ToString(e.Payload) : "Null");

                // attempt to reopen channel 0 for Rx continuous scan mode
                //_ = await _antRadio.InitializeContinuousScanMode();
            }
        }

        private void DeviceOffline(object? sender, EventArgs e)
        {
            AntDevice device = (AntDevice)sender!;
            device.DeviceWentOffline -= DeviceOffline;
            _ = Remove(device);
        }

        /// <summary>Adds the specified <see cref="AntDevice"> to the end of the collection.</summary>
        /// <param name="item">The <see cref="AntDevice">.</param>
        public new void Add(AntDevice item)
        {
            lock (CollectionLock)
            {
                base.Add(item);
                _logger.LogDebug("{Device} added.", item);
            }
        }

        /// <summary>Removes the specified <see cref="AntDevice"> from the collection.</summary>
        /// <param name="item">The <see cref="AntDevice">.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the original collection.</returns>
        public new bool Remove(AntDevice item)
        {
            lock (CollectionLock)
            {
                bool result = base.Remove(item);
                _logger.LogDebug("{Device} remove. Result = {Result}", item, result);
                return result;
            }
        }

        /// <summary>
        /// Gets the type of the ANT device.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="page">The page received from the device.</param>
        /// <returns>The type of the device. Fitness equipment may return null if <see cref="GetFitnessEquipmentType(byte[])"/> can't determine type.</returns>
        private Type? GetAntDeviceType(ChannelId channelId, byte[] page)
        {
            return channelId.DeviceType switch
            {
                HeartRate.DeviceClass => typeof(HeartRate),
                BicyclePower.DeviceClass => GetBicyclePowerType(page),
                BikeSpeedSensor.DeviceClass => typeof(BikeSpeedSensor),
                BikeCadenceSensor.DeviceClass => typeof(BikeCadenceSensor),
                CombinedSpeedAndCadenceSensor.DeviceClass => typeof(CombinedSpeedAndCadenceSensor),
                FitnessEquipment.DeviceClass => GetFitnessEquipmentType(page),
                MuscleOxygen.DeviceClass => typeof(MuscleOxygen),
                Geocache.DeviceClass => typeof(Geocache),
                Tracker.DeviceClass => typeof(Tracker),
                StrideBasedSpeedAndDistance.DeviceClass => typeof(StrideBasedSpeedAndDistance),
                _ => typeof(UnknownDevice),
            };
        }

        /// <summary>
        /// Gets the type of the bicycle power sensor.
        /// </summary>
        /// <remarks>
        /// <see cref="CrankTorqueFrequencySensor"/>s only broadcast their main page. Other bicycle power sensors broadcast
        /// any number of other pages. This allows the method to determine the sensor type.
        /// </remarks>
        /// <param name="page">The page broadcast by the device.</param>
        /// <returns></returns>
        private Type GetBicyclePowerType(byte[] page)
        {
            if ((DeviceProfiles.BicyclePower.DataPage)page[0] == DeviceProfiles.BicyclePower.DataPage.CrankTorqueFrequency)
            {
                // CTF sensor
                return typeof(CrankTorqueFrequencySensor);
            }
            else
            {
                return typeof(StandardPowerSensor);
            }
        }

        /// <summary>
        /// Gets the type of the fitness equipment.
        /// </summary>
        /// <remarks>
        /// The fitness equipment type can be determined from the <see cref="FitnessEquipment.DataPage.GeneralFEData"/> page
        /// or pages specific to the equipment type. Any other page will return null.
        /// </remarks>
        /// <param name="page">The page broadcast by the equipment.</param>
        /// <returns></returns>
        private Type? GetFitnessEquipmentType(byte[] page)
        {
            switch ((FitnessEquipment.DataPage)page[0])
            {
                case FitnessEquipment.DataPage.GeneralFEData:
                    switch ((FitnessEquipmentType)page[1])
                    {
                        case FitnessEquipmentType.Treadmill:
                            return typeof(Treadmill);
                        case FitnessEquipmentType.Elliptical:
                            return typeof(Elliptical);
                        case FitnessEquipmentType.Rower:
                            return typeof(Rower);
                        case FitnessEquipmentType.Climber:
                            return typeof(Climber);
                        case FitnessEquipmentType.NordicSkier:
                            return typeof(NordicSkier);
                        case FitnessEquipmentType.TrainerStationaryBike:
                            return typeof(TrainerStationaryBike);
                        default:
                            _logger.LogError("Unknown equipment type = {EquipmentType}", page[1]);
                            return null;
                    }
                case FitnessEquipment.DataPage.TreadmillData:
                    return typeof(Treadmill);
                case FitnessEquipment.DataPage.EllipticalData:
                    return typeof(Elliptical);
                case FitnessEquipment.DataPage.RowerData:
                    return typeof(Rower);
                case FitnessEquipment.DataPage.ClimberData:
                    return typeof(Climber);
                case FitnessEquipment.DataPage.NordicSkierData:
                    return typeof(NordicSkier);
                case FitnessEquipment.DataPage.TrainerStationaryBikeData:
                    return typeof(TrainerStationaryBike);
                case FitnessEquipment.DataPage.TrainerTorqueData:
                    return typeof(TrainerStationaryBike);
                default:
                    _logger.LogError("Unknown equipment type. Data page = {DataPage}", page);
                    return null;
            }
        }
    }
}
