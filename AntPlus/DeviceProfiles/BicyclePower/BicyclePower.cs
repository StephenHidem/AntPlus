using AntRadioInterface;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// The bicycle power sensor type.
    /// </summary>
    public enum SensorType
    {
        /// <summary>Unknown</summary>
        Unknown,
        /// <summary>Power only</summary>
        PowerOnly,
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
    /// <seealso cref="AntPlus.AntDevice" />
    public class BicyclePower : AntDevice
    {
        /// <summary>
        /// The device class ID.
        /// </summary>
        public const byte DeviceClass = 11;

        /// <summary>Gets the sensor type.</summary>
        public SensorType Sensor { get; private set; } = SensorType.Unknown;
        /// <summary>Gets the power only sensor.</summary>
        public StandardPowerSensor PowerOnlySensor { get; private set; }
        /// <summary>Gets the crank torque sensor.</summary>
        public StandardCrankTorqueSensor CrankTorqueSensor { get; private set; }
        /// <summary>Gets the wheel torque sensor.</summary>
        public StandardWheelTorqueSensor WheelTorqueSensor { get; private set; }
        /// <summary>Gets the crank torque frequency (CTF) sensor.</summary>
        public CrankTorqueFrequencySensor CTFSensor { get; private set; }
        /// <summary>Gets the calibration class.</summary>
        public Calibration Calibration { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BicyclePower"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        public BicyclePower(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            Calibration = new Calibration(this);
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage"></param>
        public override void Parse(byte[] dataPage)
        {
            switch ((DataPage)dataPage[0])
            {
                case DataPage.Unknown:
                    break;
                case DataPage.Calibration:
                    Calibration.Parse(dataPage);
                    break;
                case DataPage.GetSetParameters:
                    PowerOnlySensor?.Parameters.Parse(dataPage);
                    break;
                case DataPage.MeasurementOutput:
                    Calibration.ParseMeasurementOutputData(dataPage);
                    break;
                case DataPage.PowerOnly:
                    if (Sensor == SensorType.Unknown)
                    {
                        Sensor = SensorType.PowerOnly;
                        PowerOnlySensor = new StandardPowerSensor(this);
                    }
                    PowerOnlySensor.Parse(dataPage);
                    break;
                case DataPage.WheelTorque:
                    if (Sensor == SensorType.Unknown || Sensor == SensorType.PowerOnly)
                    {
                        Sensor = SensorType.WheelTorque;
                        PowerOnlySensor = WheelTorqueSensor = new StandardWheelTorqueSensor(this);
                    }
                    WheelTorqueSensor.ParseTorque(dataPage);
                    break;
                case DataPage.CrankTorque:
                    if (Sensor == SensorType.Unknown || Sensor == SensorType.PowerOnly)
                    {
                        Sensor = SensorType.CrankTorque;
                        PowerOnlySensor = CrankTorqueSensor = new StandardCrankTorqueSensor(this);
                    }
                    CrankTorqueSensor.ParseTorque(dataPage);
                    break;
                case DataPage.TorqueEffectivenessAndPedalSmoothness:
                    PowerOnlySensor?.TorqueEffectiveness.Parse(dataPage);
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
                    PowerOnlySensor?.CommonDataPages.ParseCommonDataPage(dataPage);
                    break;
            }
        }
    }
}
