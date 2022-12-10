using AntPlus;
using AntRadioInterface;
using System;
using System.Linq;

namespace DeviceProfiles
{
    public partial class BicyclePower : AntDevice
    {
        public const byte DeviceClass = 11;

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
            LeftForceAngle,
            PedalPosition
        }

        public enum SensorType
        {
            PowerOnly,
            WheelTorque,
            CrankTorque,
            CrankTorqueFrequency
        }

        public readonly struct MeasurementOutputData
        {
            public enum DataType
            {
                ProgressCountdown = 0,
                TimeCountdown = 1,
                WholeSensorTorque = 8,
                LeftTorque = 9,
                RightTorque = 10,
                TorqueYAxis = 11,
                TorqueOutboardness = 12,
                WholeSensorForce = 16,
                LeftForce = 17,
                RightForce = 18,
                CrankAngle = 20,
                LeftCrankAngle = 21,
                RightCrankAngle = 22,
                ZeroOffset = 24,
                Temperature = 25,
                Voltage = 26,
                LeftForceForward = 32,
                RightForceForward = 33,
                LeftForceDownward = 34,
                RightForceDownward = 35,
                LeftPedalAngle = 40,
                RightPedalAngle = 41
            }

            public int NumberOfMeasurementTypes { get; }
            public DataType MeasurementType { get; }
            public double Timestamp { get; }
            public double Measurement { get; }

            internal MeasurementOutputData(byte[] page)
            {
                NumberOfMeasurementTypes = page[1];
                MeasurementType = (DataType)page[2];
                Timestamp = BitConverter.ToUInt16(page, 4) / 2048.0;
                Measurement = BitConverter.ToUInt16(page, 6) * Math.Pow(2, (sbyte)page[3]);
            }
        }

        public SensorType Sensor { get; private set; } = SensorType.PowerOnly;
        public StandardPowerOnly PowerOnlySensor { get; private set; }
        public StandardWheelTorqueSensor WheelTorqueSensor { get; private set; }
        public StandardCrankTorqueSensor CrankTorqueSensor { get; private set; }
        public CrankTorqueFrequencySensor CTFSensor { get; private set; }
        public BPParameters Parameters { get; private set; }
        public TorqueEffectivenessAndPedalSmoothness TEPS { get; private set; }
        public BicycleCalibrationData CalibrationData { get; private set; }

        // events - transient or value change
        public event EventHandler<double> TorqueBarycenterAngleChanged;
        public event EventHandler<MeasurementOutputData> MeasurementOutputDataChanged;

        // events - class related
        public event EventHandler<StandardPowerOnly> PowerOnlyChanged;
        public event EventHandler<StandardWheelTorqueSensor> WheelTorquePageChanged;
        public event EventHandler<StandardCrankTorqueSensor> CrankTorquePageChanged;
        public event EventHandler<CrankTorqueFrequencySensor> CrankTorqueFrequencyPageChanged;
        public event EventHandler<TorqueEffectivenessAndPedalSmoothness> TEPSPageChanged;
        public event EventHandler<BicycleCalibrationData> BicycleCalibrationPageChanged;
        public event EventHandler<BPParameters> ParametersChanged;

        public BicyclePower(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            // since we don't know the sensor type we create all
            PowerOnlySensor = new StandardPowerOnly();
            WheelTorqueSensor = new StandardWheelTorqueSensor();
            CrankTorqueSensor = new StandardCrankTorqueSensor();
            CTFSensor = new CrankTorqueFrequencySensor(this);
            TEPS = new TorqueEffectivenessAndPedalSmoothness();
            CalibrationData = new BicycleCalibrationData(this);
            Parameters = new BPParameters(this);
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
                    CalibrationData.Parse(dataPage);
                    BicycleCalibrationPageChanged?.Invoke(this, CalibrationData);
                    break;
                case DataPage.GetSetParameters:
                    Parameters.Parse(dataPage);
                    ParametersChanged?.Invoke(this, Parameters);
                    break;
                case DataPage.MeasurementOutput:
                    MeasurementOutputDataChanged?.Invoke(this, new MeasurementOutputData(dataPage));
                    break;
                case DataPage.PowerOnly:
                    PowerOnlySensor.Parse(dataPage);
                    PowerOnlyChanged?.Invoke(this, PowerOnlySensor);
                    break;
                case DataPage.WheelTorque:
                    Sensor = SensorType.WheelTorque;
                    WheelTorqueSensor.Parse(dataPage);
                    WheelTorquePageChanged?.Invoke(this, WheelTorqueSensor);
                    break;
                case DataPage.CrankTorque:
                    Sensor = SensorType.CrankTorque;
                    CrankTorqueSensor.Parse(dataPage);
                    CrankTorquePageChanged?.Invoke(this, CrankTorqueSensor);
                    break;
                case DataPage.TorqueEffectivenessAndPedalSmoothness:
                    TEPS.Parse(dataPage);
                    TEPSPageChanged?.Invoke(this, TEPS);
                    break;
                case DataPage.TorqueBarycenter:
                    TorqueBarycenterAngleChanged?.Invoke(this, dataPage[2] * 0.5 + 30.0);
                    break;
                case DataPage.CrankTorqueFrequency:
                    Sensor = SensorType.CrankTorqueFrequency;
                    CTFSensor.Parse(dataPage);
                    CrankTorqueFrequencyPageChanged?.Invoke(this, CTFSensor);
                    break;
                case DataPage.RightForceAngle:
                    break;
                case DataPage.LeftForceAngle:
                    break;
                case DataPage.PedalPosition:
                    break;
                default:
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
