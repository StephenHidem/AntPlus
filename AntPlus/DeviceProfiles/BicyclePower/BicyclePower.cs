using AntPlus;
using AntRadioInterface;
using System;
using System.Linq;

namespace DeviceProfiles.BicyclePower
{
    public enum SensorType
    {
        PowerOnly,
        WheelTorque,
        CrankTorque,
        CrankTorqueFrequency
    }

    public enum DataPage
    {
        Unknown,
        Calibration,
        GetSetParameters,
        MeasurementOutput,
        PowerOnly = 0x10,
        WheelTorque,
        CrankTorque,
        TorqueEffectivenessAndPedalSmoothness,
        TorqueBarycenter,
        CrankTorqueFrequency = 0x20,
        RightForceAngle = 0xE0,
        LeftForceAngle = 0xE1,
        PedalPosition = 0xE2
    }

    public class BicyclePower : AntDevice
    {
        public const byte DeviceClass = 11;

        // supported sensors
        public SensorType Sensor { get; private set; } = SensorType.PowerOnly;
        public StandardPowerSensor BicyclePowerSensor { get; private set; }
        public CrankTorqueFrequencySensor CTFSensor { get; private set; }

        public Calibration Calibration { get; private set; }

        // events - class related
        public event EventHandler<CrankTorqueFrequencySensor> CrankTorqueFrequencyPageChanged;
        public event EventHandler<Calibration> BicycleCalibrationPageChanged;

        public BicyclePower(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            Calibration = new Calibration(this);
        }

        public override void Parse(byte[] dataPage)
        {
            // ignore duplicate/unchanged data pages
            if (lastDataPage.SequenceEqual(dataPage))
            {
                return;
            }
            lastDataPage = dataPage;

            switch ((DataPage)dataPage[0])
            {
                case DataPage.Unknown:
                    break;
                case DataPage.Calibration:
                    Calibration.Parse(dataPage);
                    BicycleCalibrationPageChanged?.Invoke(this, Calibration);
                    break;
                case DataPage.GetSetParameters:
                    BicyclePowerSensor.ParseParameters(dataPage);
                    break;
                case DataPage.MeasurementOutput:
                    BicyclePowerSensor?.ParseMeasurementOutputData(dataPage);
                    break;
                case DataPage.PowerOnly:
                    if (BicyclePowerSensor == null)
                    {
                        Sensor = SensorType.PowerOnly;
                        BicyclePowerSensor = new StandardPowerSensor(this);
                    }
                    BicyclePowerSensor.Parse(dataPage);
                    break;
                case DataPage.WheelTorque:
                    if (Sensor == SensorType.PowerOnly || BicyclePowerSensor == null)
                    {
                        Sensor = SensorType.WheelTorque;
                        BicyclePowerSensor = new StandardWheelTorqueSensor(this);
                    }
                    ((StandardWheelTorqueSensor)BicyclePowerSensor).ParseTorque(dataPage);
                    break;
                case DataPage.CrankTorque:
                    if (Sensor == SensorType.PowerOnly || BicyclePowerSensor == null)
                    {
                        Sensor = SensorType.CrankTorque;
                        BicyclePowerSensor = new StandardCrankTorqueSensor(this);
                    }
                    ((StandardCrankTorqueSensor)BicyclePowerSensor).ParseTorque(dataPage);
                    break;
                case DataPage.TorqueEffectivenessAndPedalSmoothness:
                    BicyclePowerSensor?.ParseTEPS(dataPage);
                    break;
                case DataPage.CrankTorqueFrequency:
                    Sensor = SensorType.CrankTorqueFrequency;
                    if (CTFSensor == null)
                    {
                        CTFSensor = new CrankTorqueFrequencySensor(this);
                    }
                    CTFSensor.Parse(dataPage);
                    CrankTorqueFrequencyPageChanged?.Invoke(this, CTFSensor);
                    break;
                case DataPage.RightForceAngle:
                case DataPage.LeftForceAngle:
                case DataPage.PedalPosition:
                case DataPage.TorqueBarycenter:
                    ((StandardCrankTorqueSensor)BicyclePowerSensor).ParseCyclingDynamics(dataPage);
                    break;
                default:
                    BicyclePowerSensor?.ParseCommonDataPage(dataPage);
                    break;
            }
        }

        public override void ChannelEventHandler(EventMsgId eventMsgId)
        {
            throw new NotImplementedException();
        }

        public override void ChannelResponseHandler(byte messageId, ResponseMsgId responseMsgId)
        {
            throw new NotImplementedException();
        }
    }
}
