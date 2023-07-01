using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence
{
    /// <summary>
    /// This class supports combined speed and cadence sensors.
    /// </summary>
    /// <seealso cref="AntDevice" />
    public class CombinedSpeedAndCadenceSensor : AntDevice
    {
        /// <summary>
        /// The CombinedSpeedAndCadenceSensor device class ID.
        /// </summary>
        public const byte DeviceClass = 121;

        private bool isFirstDataMessage = true;     // used for accumulated values
        private byte[] lastDataPage = new byte[8];
        private ushort prevSpeedEventTime;
        private ushort prevSpeedRevCount;
        private ushort prevCadenceEventTime;
        private ushort prevCadenceRevCount;
        private readonly ILogger<CombinedSpeedAndCadenceSensor> _logger;

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
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(CombinedSpeedAndCadenceSensor).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.SpeedAndCadence.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedSpeedAndCadenceSensor"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline"/>.</param>
        public CombinedSpeedAndCadenceSensor(ChannelId channelId, IAntChannel antChannel, ILogger<CombinedSpeedAndCadenceSensor> logger, int timeout = 2000) : base(channelId, antChannel, timeout)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            int deltaEventTime;
            int deltaRevCount;

            base.Parse(dataPage);

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

            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
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
