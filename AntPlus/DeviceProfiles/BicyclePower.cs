using AntPlus;
using AntPlus.DeviceProfiles;
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

        public enum ParameterSubpage
        {
            CrankParameters = 0x01,
            PowerPhaseConfiguration = 0x02,
            RiderPositionConfiguration = 0x04,
            AdvancedCapabilities1 = 0xFD,
            AdvancedCapabilities2 = 0xFE
        }

        public readonly struct CrankParameters
        {
            public enum CrankLengthStatus
            {
                Invalid = 0,
                Default = 1,
                ManuallySet = 2,
                AutoOrFixed = 3
            }

            public enum SensorMisMatchStatus
            {
                Undefined = 0,
                RightSensorOlder = 0x04,
                LeftSensorOlder = 0x08,
                Identical = 0x0C
            }

            public enum SensorAvailabilityStatus
            {
                Undefined = 0,
                LeftPresent = 0x10,
                RightPresent = 0x20,
                BothPresent = 0x30
            }

            public enum CustomCalibrationStatus
            {
                NotSupported = 0,
                NotRequired = 0x40,
                Required = 0x80
            }

            /// <summary>The crank length in millimeters.</summary>
            public double CrankLength { get; }
            public CrankLengthStatus CrankStatus { get; }
            public SensorMisMatchStatus MismatchStatus { get; }
            public SensorAvailabilityStatus AvailabilityStatus { get; }
            public CustomCalibrationStatus CustomCalibration { get; }
            public bool AutoCrankLength { get; }

            internal CrankParameters(byte[] page)
            {
                CrankLength = page[4] * 0.5 + 110.0;
                CrankStatus = (CrankLengthStatus)(page[5] & 0x03);
                MismatchStatus = (SensorMisMatchStatus)(page[5] & 0x0C);
                AvailabilityStatus = (SensorAvailabilityStatus)(page[5] & 0x30);
                CustomCalibration = (CustomCalibrationStatus)(page[5] & 0xC0);
                AutoCrankLength = (page[6] & 0x01) != 0;
            }
        }
        public readonly struct AdvancedCapabilities1
        {
            [Flags]
            public enum InteropProp
            {
                DefaultCrankLength = 1,
                RequiresCrankLength = 2,
            }

            [Flags]
            public enum InteroperableCapabilies
            {
                None = 0,
                FourHz = 1,
                EightHz = 2,
                AutoZero = 16,
                AutoCrankLength = 32,
                TEandPS = 64,
            }

            public InteropProp InteroperableProperties { get; }
            public byte CustomProperties { get; }
            public InteroperableCapabilies Mask { get; }
            public byte CustomCapabilitiesMask { get; }
            public InteroperableCapabilies Value { get; }
            public byte CustomCapabilitiesValue { get; }

            internal AdvancedCapabilities1(byte[] page)
            {
                InteroperableProperties = (InteropProp)(page[2] & 0x03);
                CustomProperties = page[3];
                Mask = (InteroperableCapabilies)page[4];
                CustomCapabilitiesMask = page[5];
                Value = (InteroperableCapabilies)page[6];
                CustomCapabilitiesValue = page[7];
            }
        }
        public readonly struct AdvancedCapabilities2
        {
            [Flags]
            public enum InteroperableCapabilies
            {
                None = 0,
                FourHz = 1,
                EightHz = 2,
                PowerPhase8Hz = 8,
                PCO8Hz = 16,
                RiderPosition8Hz = 32,
                TorqueBarycenter8Hz = 64,
            }

            public InteroperableCapabilies Mask { get; }
            public InteroperableCapabilies Value { get; }

            internal AdvancedCapabilities2(byte[] page)
            {
                Mask = (InteroperableCapabilies)page[4];
                Value = (InteroperableCapabilies)page[6];
            }
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

        public StandardPowerOnly PowerOnlySensor { get; private set; }
        public StandardWheelTorqueSensor WheelTorqueSensor { get; private set; }
        public StandardCrankTorqueSensor CrankTorqueSensor { get; private set; }
        public TorqueEffectivenessAndPedalSmoothness TEPS { get; private set; }
        public BicycleCalibrationData CalibrationData { get; private set; }

        public event EventHandler<CrankParameters> CrankParametersChanged;
        public event EventHandler<double> PeakTorqueThresholdChanged;
        public event EventHandler<int> RiderPositionTimeOffsetChaged;
        public event EventHandler<AdvancedCapabilities1> AdvancedCapabilities1Changed;
        public event EventHandler<AdvancedCapabilities2> AdvancedCapabilities2Changed;
        public event EventHandler<double> TorqueBarycenterAngleChanged;
        public event EventHandler<MeasurementOutputData> MeasurementOutputDataChanged;


        public event EventHandler<StandardPowerOnly> PowerOnlyChanged;
        public event EventHandler<StandardWheelTorqueSensor> WheelTorquePageChanged;
        public event EventHandler<StandardCrankTorqueSensor> CrankTorquePageChanged;
        public event EventHandler<TorqueEffectivenessAndPedalSmoothness> TEPSPageChanged;
        public event EventHandler<BicycleCalibrationData> BicycleCalibrationPageChanged;

        public BicyclePower(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
            // since we don't know the sensor type we create all
            PowerOnlySensor = new StandardPowerOnly();
            WheelTorqueSensor = new StandardWheelTorqueSensor();
            CrankTorqueSensor = new StandardCrankTorqueSensor();
            TEPS = new TorqueEffectivenessAndPedalSmoothness();

            CalibrationData = new BicycleCalibrationData(this);
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
                    switch ((ParameterSubpage)dataPage[1])
                    {
                        case ParameterSubpage.CrankParameters:
                            CrankParametersChanged?.Invoke(this, new CrankParameters(dataPage));
                            break;
                        case ParameterSubpage.PowerPhaseConfiguration:
                            PeakTorqueThresholdChanged?.Invoke(this, dataPage[2] * 0.5);
                            break;
                        case ParameterSubpage.RiderPositionConfiguration:
                            RiderPositionTimeOffsetChaged?.Invoke(this, dataPage[2]);
                            break;
                        case ParameterSubpage.AdvancedCapabilities1:
                            AdvancedCapabilities1Changed?.Invoke(this, new AdvancedCapabilities1(dataPage));
                            break;
                        case ParameterSubpage.AdvancedCapabilities2:
                            AdvancedCapabilities2Changed?.Invoke(this, new AdvancedCapabilities2(dataPage));
                            break;
                        default:
                            break;
                    }
                    break;
                case DataPage.MeasurementOutput:
                    MeasurementOutputDataChanged?.Invoke(this, new MeasurementOutputData(dataPage));
                    break;
                case DataPage.PowerOnly:
                    PowerOnlySensor.Parse(dataPage);
                    PowerOnlyChanged?.Invoke(this, PowerOnlySensor);
                    break;
                case DataPage.WheelTorque:
                    WheelTorqueSensor.Parse(dataPage);
                    WheelTorquePageChanged?.Invoke(this, WheelTorqueSensor);
                    break;
                case DataPage.CrankTorque:
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
