using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System.IO;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The bicycle power sensor type.
    /// </summary>
    public enum SensorType
    {
        /// <summary>Unknown</summary>
        Unknown,
        /// <summary>Power</summary>
        Power,
        /// <summary>Wheel torque</summary>
        WheelTorque,
        /// <summary>Crank torque</summary>
        CrankTorque,
        /// <summary>Crank torque frequency</summary>
        CrankTorqueFrequency
    }

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

    /// <summary>
    /// The primary class managing bicycle power sensors.
    /// </summary>
    /// <seealso cref="AntDevice" />
    public class BicyclePower : AntDevice
    {
        /// <summary>
        /// The device class ID.
        /// </summary>
        public const byte DeviceClass = 11;

        /// <summary>Gets the sensor type.</summary>
        public SensorType Sensor { get; private set; } = SensorType.Unknown;
        /// <summary>Gets the power sensor.</summary>
        public StandardPowerSensor PowerSensor { get; private set; }
        /// <summary>Gets the crank torque sensor.</summary>
        public StandardCrankTorqueSensor CrankTorqueSensor { get; private set; }
        /// <summary>Gets the wheel torque sensor.</summary>
        public StandardWheelTorqueSensor WheelTorqueSensor { get; private set; }
        /// <summary>Gets the crank torque frequency (CTF) sensor.</summary>
        public CrankTorqueFrequencySensor CTFSensor { get; private set; }

        /// <summary>Gets the calibration class.</summary>
        public Calibration Calibration { get; private set; }
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(BicyclePower).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.BicyclePower.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="BicyclePower"/> class. The default timeout is 2 seconds.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline"/>.</param>
        public BicyclePower(ChannelId channelId, IAntChannel antChannel, ILogger<BicyclePower> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
            Calibration = new Calibration(this);
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            switch ((DataPage)dataPage[0])
            {
                case DataPage.Unknown:
                    break;
                case DataPage.Calibration:
                    Calibration.Parse(dataPage);
                    break;
                case DataPage.GetSetParameters:
                    PowerSensor?.Parameters.Parse(dataPage);
                    break;
                case DataPage.MeasurementOutput:
                    Calibration.ParseMeasurementOutputData(dataPage);
                    break;
                case DataPage.PowerOnly:
                    if (Sensor == SensorType.Unknown)
                    {
                        Sensor = SensorType.Power;
                        PowerSensor = new StandardPowerSensor(this, logger);
                    }
                    PowerSensor.Parse(dataPage);
                    break;
                case DataPage.WheelTorque:
                    if (Sensor == SensorType.Unknown || Sensor == SensorType.Power)
                    {
                        Sensor = SensorType.WheelTorque;
                        PowerSensor = WheelTorqueSensor = new StandardWheelTorqueSensor(this, logger);
                    }
                    WheelTorqueSensor.ParseTorque(dataPage);
                    break;
                case DataPage.CrankTorque:
                    if (Sensor == SensorType.Unknown || Sensor == SensorType.Power)
                    {
                        Sensor = SensorType.CrankTorque;
                        PowerSensor = CrankTorqueSensor = new StandardCrankTorqueSensor(this, logger);
                    }
                    CrankTorqueSensor.ParseTorque(dataPage);
                    break;
                case DataPage.TorqueEffectivenessAndPedalSmoothness:
                    PowerSensor?.TorqueEffectiveness.Parse(dataPage);
                    break;
                case DataPage.CrankTorqueFrequency:
                    Sensor = SensorType.CrankTorqueFrequency;
                    if (CTFSensor == null)
                    {
                        CTFSensor = new CrankTorqueFrequencySensor(this);
                    }
                    CTFSensor.Parse(dataPage);
                    break;
                case DataPage.RightForceAngle:
                case DataPage.LeftForceAngle:
                case DataPage.PedalPosition:
                case DataPage.TorqueBarycenter:
                    CrankTorqueSensor.ParseCyclingDynamics(dataPage);
                    break;
                default:
                    PowerSensor?.CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }
    }
}
