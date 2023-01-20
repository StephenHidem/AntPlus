using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    /// <summary>
    /// This class supports bike cadence sensors.
    /// </summary>
    /// <seealso cref="AntPlus.DeviceProfiles.BikeSpeedAndCadence.CommonSpeedCadence" />
    public class BikeCadenceSensor : CommonSpeedCadence
    {
        /// <summary>
        /// The BikeCadenceSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 122;

        /// <summary>Gets the instantaneous cadence in rotations per minute.</summary>
        public double InstantaneousCadence { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BikeCadenceSensor"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">The ant channel.</param>
        public BikeCadenceSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
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
                    RaisePropertyChange(nameof(InstantaneousCadence));
                }
            }
        }
    }
}
