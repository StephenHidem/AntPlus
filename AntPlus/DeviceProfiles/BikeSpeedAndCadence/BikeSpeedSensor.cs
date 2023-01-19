using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    /// <summary>
    /// This class supports bike speed sensors.
    /// </summary>
    /// <remarks>
    /// Set the wheel circumference if the default value is incorrect. The calculations rely
    /// on the wheel circumference.
    /// </remarks>
    public class BikeSpeedSensor : CommonSpeedCadence
    {
        /// <summary>
        /// The BikeSpeedSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 123;

        /// <summary>
        /// The wheel circumference in meters. The default is 2.2 meters.
        /// </summary>
        public double WheelCircumference { get; set; } = 2.2;
        /// <summary>Gets the instantaneous speed in meters per second.</summary>
        public double InstantaneousSpeed { get; private set; }
        /// <summary>Gets the accumulated distance in meters. This value is accumulated since distance was first reported.</summary>
        public double AccumulatedDistance { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BikeSpeedSensor"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">The ant channel.</param>
        public BikeSpeedSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
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
                    RaisePropertyChange(nameof(InstantaneousSpeed));
                    RaisePropertyChange(nameof(AccumulatedDistance));
                }
            }
        }
    }
}
