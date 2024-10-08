﻿using Microsoft.Extensions.Logging;
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
    /// <remarks>
    /// This class will create an <see cref="AntDevice"/> from one of the supported ANT device classes and add
    /// it to the collection as they are discovered. An important consideration is when the device is no longer
    /// available. You may supply a timeout duration or the number of missed messages. <b><u>You should prefer missed messages</u></b>
    /// as this scales the timeout duration based on the broadcast transmission rate of the particular ANT device.
    /// The timeout/missed messages will be applied globally to ANT devices created by this collection.
    /// </remarks>
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
        private int _channelNum = 1;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<AntDeviceCollection> _logger;
        private readonly int _timeout;
        private IAntChannel[]? _channels;

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
        /// </remarks>
        public AntDeviceCollection(IAntRadio antRadio, ILoggerFactory loggerFactory, int antDeviceTimeout = 2000)
        {
            _antRadio = antRadio;
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            _logger = _loggerFactory.CreateLogger<AntDeviceCollection>();
            _timeout = antDeviceTimeout;

            // gRPC clients need to run asynchronously
            Task.Run(async () =>
            {
                _channels = await antRadio.InitializeContinuousScanMode();
                _channels[0].ChannelResponse += Channel_ChannelResponse;
            });

            _logger.LogInformation("Created AntDeviceCollection: antDeviceTimeout = {0}", antDeviceTimeout);
        }

        private async void Channel_ChannelResponse(object sender, AntResponse e)
        {
            if (e.ChannelId != null)
            {
                AntDevice? device;
                device = this.FirstOrDefault(ant => ant.ChannelId.Id == e.ChannelId.Id);
                if (device == null)
                {
                    // provide channel ID and payload
                    device = CreateAntDevice(e.ChannelId, e.Payload!);
                    if (device == null) return;     // some device types have additional qualifiers
                    Add(device);
                    device.DeviceWentOffline += DeviceOffline;
                }
                device.Parse(e.Payload!);
            }
            else
            {
                _logger.LogCritical("ChannelId is null. Channel # = {ChannelNum}, Response ID = {Response}, Payload = {Payload}.",
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
                _logger.LogDebug("{Device} added.", item);
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
                _logger.LogDebug("{Device} remove. Result = {Result}", item, result);
                return result;
            }
        }

        private AntDevice? CreateAntDevice(ChannelId channelId, byte[] dataPage)
        {
            IAntChannel channel = _channels![_channelNum++];
            if (_channelNum == _channels.Length) { _channelNum = 1; }
            return channelId.DeviceType switch
            {
                HeartRate.DeviceClass => new HeartRate(channelId, channel, _loggerFactory.CreateLogger<HeartRate>(), _timeout),
                BicyclePower.DeviceClass => BicyclePower.GetBicyclePowerSensor(dataPage, channelId, channel, _loggerFactory, _timeout),
                BikeSpeedSensor.DeviceClass => new BikeSpeedSensor(channelId, channel, _loggerFactory.CreateLogger<BikeSpeedSensor>(), _timeout),
                BikeCadenceSensor.DeviceClass => new BikeCadenceSensor(channelId, channel, _loggerFactory.CreateLogger<BikeCadenceSensor>(), _timeout),
                CombinedSpeedAndCadenceSensor.DeviceClass => new CombinedSpeedAndCadenceSensor(channelId, channel, _loggerFactory.CreateLogger<CombinedSpeedAndCadenceSensor>(), _timeout),
                FitnessEquipment.DeviceClass => FitnessEquipment.GetFitnessEquipment(dataPage, channelId, channel, _loggerFactory, _timeout),
                MuscleOxygen.DeviceClass => new MuscleOxygen(channelId, channel, _loggerFactory.CreateLogger<MuscleOxygen>(), _timeout),
                Geocache.DeviceClass => new Geocache(channelId, channel, _loggerFactory.CreateLogger<Geocache>(), _timeout),
                Tracker.DeviceClass => new Tracker(channelId, channel, _loggerFactory.CreateLogger<Tracker>(), _timeout),
                StrideBasedSpeedAndDistance.DeviceClass => new StrideBasedSpeedAndDistance(channelId, channel, _loggerFactory.CreateLogger<StrideBasedSpeedAndDistance>(), _timeout),
                _ => new UnknownDevice(channelId, channel, _loggerFactory.CreateLogger<UnknownDevice>(), _timeout),
            };
        }
    }
}
