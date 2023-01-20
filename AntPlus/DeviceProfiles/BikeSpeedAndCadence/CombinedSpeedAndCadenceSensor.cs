using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    /// <summary>
    /// This class supports combined speed and cadence sensors.
    /// </summary>
    /// <seealso cref="AntPlus.AntDevice" />
    public class CombinedSpeedAndCadenceSensor : AntDevice
    {
        /// <summary>
        /// The CombinedSpeedAndCadenceSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 121;

        private bool isFirstDataMessage = true;     // used for accumulated values
        private ushort prevSpeedEventTime;
        private ushort prevSpeedRevCount;
        private ushort prevCadenceEventTime;
        private ushort prevCadenceRevCount;

        /// <summary>Gets the instantaneous cadence in revolutions per minute.</summary>
        public double InstantaneousCadence { get; private set; }
        /// <summary>
        /// The wheel circumference in meters. The default is 2.2 meters.
        /// </summary>
        public double WheelCircumference { get; set; } = 2.2;
        /// <summary>Gets the instantaneous speed in meters per second.</summary>
        public double InstantaneousSpeed { get; private set; }
        /// <summary>Gets the accumulated distance in meters.</summary>
        public double AccumulatedDistance { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedSpeedAndCadenceSensor"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        public CombinedSpeedAndCadenceSensor(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage"></param>
        public override void Parse(byte[] dataPage)
        {
            int deltaEventTime;
            int deltaRevCount;

            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
                return;
            }
            lastDataPage = dataPage;

            if (isFirstDataMessage)
            {
                isFirstDataMessage = false;
                prevSpeedEventTime = BitConverter.ToUInt16(dataPage, 4);
                prevCadenceEventTime = BitConverter.ToUInt16(dataPage, 0);
                prevSpeedRevCount = BitConverter.ToUInt16(dataPage, 6);
                prevCadenceRevCount = BitConverter.ToUInt16(dataPage, 2);
                lastDataPage = dataPage;
                return;
            }

            deltaEventTime = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 0), ref prevCadenceEventTime);
            if (deltaEventTime != 0)
            {
                deltaRevCount = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 2), ref prevCadenceRevCount);
                InstantaneousCadence = 60.0 * deltaRevCount * 1024.0 / deltaEventTime;
            }

            deltaEventTime = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref prevSpeedEventTime);
            if (deltaEventTime != 0)
            {
                deltaRevCount = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 6), ref prevSpeedRevCount);
                InstantaneousSpeed = WheelCircumference * deltaRevCount * 1024.0 / deltaEventTime;
                AccumulatedDistance += WheelCircumference * deltaRevCount;
            }
            RaisePropertyChange(string.Empty);
        }
    }
}
