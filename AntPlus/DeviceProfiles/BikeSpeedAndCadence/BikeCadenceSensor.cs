using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    /// <summary>
    /// This class supports bike cadence sensors.
    /// </summary>
    /// <seealso cref="CommonSpeedCadence" />
    public partial class BikeCadenceSensor : CommonSpeedCadence
    {
        /// <summary>
        /// The device type value transmitted in the channel ID.
        /// </summary>
        public const byte DeviceClass = 122;

        /// <inheritdoc/>
        public override int ChannelCount => 8102;

        /// <summary>Gets the instantaneous cadence in rotations per minute.</summary>
        [ObservableProperty]
        private double instantaneousCadence;
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(BikeCadenceSensor).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.BikeCadence.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="BikeCadenceSensor"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        public BikeCadenceSensor(ChannelId channelId, IAntChannel antChannel, ILogger<BikeCadenceSensor> logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BikeCadenceSensor"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        public BikeCadenceSensor(ChannelId channelId, IAntChannel antChannel, ILogger<BikeCadenceSensor> logger, TimeoutOptions? options)
            : base(channelId, antChannel, logger, options)
        {
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            if (!isFirstDataMessage)
            {
                // this data is present in all data pages
                int deltaEventTime = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref prevEventTime);
                if (deltaEventTime != 0)
                {
                    int deltaRevCount = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 6), ref prevRevCount);
                    InstantaneousCadence = 60.0 * deltaRevCount * 1024.0 / deltaEventTime;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Bike Cadence Sensor";
        }
    }
}
