using AntRadioInterface;
using System;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    public enum SensorType
    {
        Unknown,
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
        public SensorType Sensor { get; private set; } = SensorType.Unknown;
        public StandardPowerSensor PowerOnlySensor { get; private set; }
        public StandardCrankTorqueSensor CrankTorqueSensor { get; private set; }
        public StandardWheelTorqueSensor WheelTorqueSensor { get; private set; }
        public CrankTorqueFrequencySensor CTFSensor { get; private set; }
        public Calibration Calibration { get; private set; }

        public BicyclePower(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            Calibration = new Calibration(this);
        }

        public override void Parse(byte[] dataPage)
        {
            // ignore duplicate/unchanged data pages
            //if (lastDataPage.SequenceEqual(dataPage))
            //{
            //    return;
            //}
            //lastDataPage = dataPage;

            switch ((DataPage)dataPage[0])
            {
                case DataPage.Unknown:
                    break;
                case DataPage.Calibration:
                    Calibration.Parse(dataPage);
                    break;
                case DataPage.GetSetParameters:
                    PowerOnlySensor?.ParseParameters(dataPage);
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
                    PowerOnlySensor?.ParseTEPS(dataPage);
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
