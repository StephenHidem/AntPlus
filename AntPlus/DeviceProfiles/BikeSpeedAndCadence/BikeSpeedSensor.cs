using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    /// <summary>
    /// This class supports bike speed sensors.
    /// </summary>
    /// <remarks>
    /// Set the wheel circumference if the default value is incorrect. The calculations rely
    /// on the wheel circumference.
    /// </remarks>
    /// <seealso cref="CommonSpeedCadence" />
    public partial class BikeSpeedSensor : CommonSpeedCadence
    {
        /// <summary>
        /// The device type value transmitted in the channel ID.
        /// </summary>
        public const byte DeviceClass = 123;

        /// <inheritdoc/>
        public override int ChannelCount => 8118;

        /// <summary>
        /// The wheel circumference in meters. The default is 2.2 meters.
        /// </summary>
        public double WheelCircumference { get; set; } = 2.2;
        /// <summary>Gets the instantaneous speed in meters per second.</summary>
        [ObservableProperty]
        private double instantaneousSpeed;
        /// <summary>Gets the accumulated distance in meters. This value is accumulated since distance was first reported.</summary>
        [ObservableProperty]
        private double accumulatedDistance;
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(BikeSpeedSensor).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.BikeSpeed.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="BikeSpeedSensor"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int)"/>
        public BikeSpeedSensor(ChannelId channelId, IAntChannel antChannel, ILogger<BikeSpeedSensor> logger, int timeout)
            : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BikeSpeedSensor"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, TimeoutOptions?)"/>
        public BikeSpeedSensor(ChannelId channelId, IAntChannel antChannel, ILogger<BikeSpeedSensor> logger, TimeoutOptions? timeoutOptions)
            : base(channelId, antChannel, logger, timeoutOptions)
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
                    InstantaneousSpeed = WheelCircumference * deltaRevCount * 1024.0 / deltaEventTime;
                    AccumulatedDistance += WheelCircumference * deltaRevCount;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Bike Speed Sensor";
        }
    }
}
