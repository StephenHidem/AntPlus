using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// Main data pages.
    /// </summary>
    public enum DataPage
    {
        /// <summary>Unknown</summary>
        Unknown,
        /// <summary>Calibration</summary>
        Calibration,
        /// <summary>Get/Set Parameters</summary>
        GetSetParameters,
        /// <summary>Measurement Output</summary>
        MeasurementOutput,
        /// <summary>Power Only</summary>
        PowerOnly = 0x10,
        /// <summary>Wheel Torque</summary>
        WheelTorque,
        /// <summary>Crank Torque</summary>
        CrankTorque,
        /// <summary>Torque effectiveness and pedal smoothness</summary>
        TorqueEffectivenessAndPedalSmoothness,
        /// <summary>Torque barycenter</summary>
        TorqueBarycenter,
        /// <summary>Crank torque frequency</summary>
        CrankTorqueFrequency = 0x20,
        /// <summary>Right force angle</summary>
        RightForceAngle = 0xE0,
        /// <summary>Left force angle</summary>
        LeftForceAngle = 0xE1,
        /// <summary>Pedal position</summary>
        PedalPosition = 0xE2
    }

    /// <summary>Base class for bicycle power sensors.</summary>
    abstract public class BicyclePower : AntDevice
    {
        /// <summary>
        /// The device class ID.
        /// </summary>
        public const byte DeviceClass = 11;

        /// <summary>Gets the calibration class.</summary>
        public Calibration Calibration { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="BicyclePower" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">The ant channel.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="timeout">The timeout.</param>
        public BicyclePower(ChannelId channelId, IAntChannel antChannel, ILogger<BicyclePower> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
            Calibration = new Calibration(this, logger);
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            switch ((DataPage)dataPage[0])
            {
                case DataPage.Calibration:
                    Calibration.Parse(dataPage);
                    break;
                case DataPage.MeasurementOutput:
                    Calibration.ParseMeasurementOutputData(dataPage);
                    break;
                default:
                    break;
            }
        }

        /// <summary>Gets the bicycle power sensor.</summary>
        /// <param name="dataPage">The data page.</param>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">The ANT channel.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static BicyclePower GetBicyclePowerSensor(byte[] dataPage, ChannelId channelId, IAntChannel antChannel, ILogger<BicyclePower> logger, int timeout = 2000)
        {
            if ((DataPage)dataPage[0] == DataPage.CrankTorqueFrequency)
            {
                // return CTF sensor
                CrankTorqueFrequencySensor sensor = new CrankTorqueFrequencySensor(channelId, antChannel, logger, timeout);
                sensor.Parse(dataPage);
                return sensor;
            }
            else
            {
                StandardPowerSensor sensor = new StandardPowerSensor(channelId, antChannel, logger, timeout);
                sensor.Parse(dataPage);
                return sensor;
            }
        }
    }
}
