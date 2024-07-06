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
        /// The BikeCadenceSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 122;

        /// <summary>Gets the instantaneous cadence in rotations per minute.</summary>
        [ObservableProperty]
        private double instantaneousCadence;
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(BikeCadenceSensor).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.BikeCadence.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="BikeCadenceSensor"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">The ant channel.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        public BikeCadenceSensor(ChannelId channelId, IAntChannel antChannel, ILogger<BikeCadenceSensor> logger, ushort timeout = 2000) : base(channelId, antChannel, logger, timeout)
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
