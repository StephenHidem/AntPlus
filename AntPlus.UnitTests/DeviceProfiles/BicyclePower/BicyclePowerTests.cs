using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.Parameters;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.Parameters.CrankParameters;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.StandardCrankTorqueSensor.PedalPositionPage;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePower
{
    [TestClass]
    public class BicyclePowerTests
    {
        private MockRepository mockRepository;

        private ChannelId mockChannelId;
        private Mock<IAntChannel> mockAntChannel;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockChannelId = new ChannelId(0);
            mockAntChannel = mockRepository.Create<IAntChannel>();
        }

        private SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.BicyclePower CreateBicyclePower()
        {
            return new SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.BicyclePower(
                mockChannelId,
                mockAntChannel.Object);
        }

        [TestMethod]
        [DataRow(16, SensorType.Power)]
        [DataRow(17, SensorType.WheelTorque)]
        [DataRow(18, SensorType.CrankTorque)]
        [DataRow(32, SensorType.CrankTorqueFrequency)]
        public void Parse_CreateSensor_ExpectedSensor(int page, SensorType sensorType)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)page;

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(sensorType, bicyclePower.Sensor);
            switch (sensorType)
            {
                case SensorType.Power:
                    Assert.IsNotNull(bicyclePower.PowerSensor);
                    Assert.IsNull(bicyclePower.WheelTorqueSensor);
                    Assert.IsNull(bicyclePower.CrankTorqueSensor);
                    Assert.IsNull(bicyclePower.CTFSensor);
                    break;
                case SensorType.WheelTorque:
                    Assert.IsNotNull(bicyclePower.PowerSensor);
                    Assert.IsNotNull(bicyclePower.WheelTorqueSensor);
                    Assert.IsNull(bicyclePower.CrankTorqueSensor);
                    Assert.IsNull(bicyclePower.CTFSensor);
                    break;
                case SensorType.CrankTorque:
                    Assert.IsNotNull(bicyclePower.PowerSensor);
                    Assert.IsNotNull(bicyclePower.CrankTorqueSensor);
                    Assert.IsNull(bicyclePower.WheelTorqueSensor);
                    Assert.IsNull(bicyclePower.CTFSensor);
                    break;
                case SensorType.CrankTorqueFrequency:
                    Assert.IsNull(bicyclePower.PowerSensor);
                    Assert.IsNotNull(bicyclePower.CTFSensor);
                    Assert.IsNull(bicyclePower.WheelTorqueSensor);
                    Assert.IsNull(bicyclePower.CrankTorqueSensor);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        [TestMethod]
        [DataRow(new byte[] { 1, 0xAC, 0x00, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, Calibration.CalibrationResponse.Succeeded, Calibration.AutoZero.Off)]
        [DataRow(new byte[] { 1, 0xAC, 0x01, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, Calibration.CalibrationResponse.Succeeded, Calibration.AutoZero.On)]
        [DataRow(new byte[] { 1, 0xAC, 0xFF, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, Calibration.CalibrationResponse.Succeeded, Calibration.AutoZero.NotSupported)]
        [DataRow(new byte[] { 1, 0xAF, 0x00, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, Calibration.CalibrationResponse.Failed, Calibration.AutoZero.Off)]
        [DataRow(new byte[] { 1, 0xAF, 0x01, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, Calibration.CalibrationResponse.Failed, Calibration.AutoZero.On)]
        [DataRow(new byte[] { 1, 0xAF, 0xFF, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, Calibration.CalibrationResponse.Failed, Calibration.AutoZero.NotSupported)]
        public void Parse_GeneralCalibrationResponse_ExpectedResponse(byte[] dataPage, Calibration.CalibrationResponse response, Calibration.AutoZero autoZero)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(response, bicyclePower.Calibration.CalibrationStatus);
            Assert.AreEqual(autoZero, bicyclePower.Calibration.AutoZeroStatus);
            Assert.IsTrue(bicyclePower.Calibration.CalibrationData == 0x1122);
        }

        [TestMethod]
        [DataRow(0, MeasurementOutputData.DataType.ProgressCountdown)]
        [DataRow(1, MeasurementOutputData.DataType.TimeCountdown)]
        [DataRow(8, MeasurementOutputData.DataType.WholeSensorTorque)]
        [DataRow(42, MeasurementOutputData.DataType.Reserved)]
        [DataRow(254, MeasurementOutputData.DataType.Reserved)]
        public void Parse_MeasurementOutputData_ExpectedDataType(int val, MeasurementOutputData.DataType expDataType)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8] { 3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            dataPage[2] = (byte)val;

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expDataType, bicyclePower.Calibration.Measurements[0].MeasurementType);
        }

        [TestMethod]
        [DataRow(sbyte.MinValue, short.MaxValue)]
        [DataRow(sbyte.MaxValue, short.MaxValue)]
        [DataRow(sbyte.MinValue, short.MinValue)]
        [DataRow(sbyte.MaxValue, short.MinValue)]
        public void Parse_MeasurementOutputData_ExpectedScaledMeasurement(int scale, int meas)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8] { 3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            dataPage[3] = (byte)scale;
            dataPage[6] = BitConverter.GetBytes((short)meas)[0];
            dataPage[7] = BitConverter.GetBytes((short)meas)[1];
            double expMeas = meas * Math.Pow(2, scale);

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expMeas, bicyclePower.Calibration.Measurements[0].Measurement);
        }

        [TestMethod]
        [DataRow(ushort.MinValue, 0)]
        [DataRow(ushort.MaxValue, 31.99951171875)]
        [DataRow(32768, 16)]
        public void Parse_MeasurementOutputData_ExpectedTimestamp(int timeStamp, double timeSpan)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8] { 3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            dataPage[4] = BitConverter.GetBytes((ushort)timeStamp)[0];
            dataPage[5] = BitConverter.GetBytes((ushort)timeStamp)[1];

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(timeSpan, bicyclePower.Calibration.Measurements[0].Timestamp);
        }

        [TestMethod]
        [DataRow(0x00, 110.0)]
        [DataRow(0xFD, 236.5)]
        [DataRow(0xFE, double.NaN)]
        [DataRow(0xFF, double.NaN)]
        public void Parse_GetSetParameters_ExpectedCrankLength(int val, double expCrankLen)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, (byte)val, 0, 0, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expCrankLen, bicyclePower.PowerSensor.Parameters.Crank.CrankLength);
        }

        [TestMethod]
        [DataRow(0, CrankLengthStatus.Invalid)]
        [DataRow(1, CrankLengthStatus.Default)]
        [DataRow(2, CrankLengthStatus.ManuallySet)]
        [DataRow(3, CrankLengthStatus.AutoOrFixed)]
        public void Parse_GetSetParameters_ExpectedCrankStatus(int val, CrankLengthStatus expCrankStatus)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expCrankStatus, bicyclePower.PowerSensor.Parameters.Crank.CrankStatus);
        }

        [TestMethod]
        [DataRow(0x00, SensorMisMatchStatus.Undefined)]
        [DataRow(0x04, SensorMisMatchStatus.RightSensorOlder)]
        [DataRow(0x08, SensorMisMatchStatus.LeftSensorOlder)]
        [DataRow(0x0C, SensorMisMatchStatus.Identical)]
        public void Parse_GetSetParameters_ExpectedSensorMismatchStatus(int val, SensorMisMatchStatus expSensorSWStatus)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expSensorSWStatus, bicyclePower.PowerSensor.Parameters.Crank.MismatchStatus);
        }

        [TestMethod]
        [DataRow(0x00, SensorAvailabilityStatus.Undefined)]
        [DataRow(0x10, SensorAvailabilityStatus.LeftPresent)]
        [DataRow(0x20, SensorAvailabilityStatus.RightPresent)]
        [DataRow(0x30, SensorAvailabilityStatus.BothPresent)]
        public void Parse_GetSetParameters_ExpectedSensorAvailabilityStatus(int val, SensorAvailabilityStatus expSensorAvailibility)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expSensorAvailibility, bicyclePower.PowerSensor.Parameters.Crank.AvailabilityStatus);
        }

        [TestMethod]
        [DataRow(0x00, CustomCalibrationStatus.NotSupported)]
        [DataRow(0x40, CustomCalibrationStatus.NotRequired)]
        [DataRow(0x80, CustomCalibrationStatus.Required)]
        [DataRow(0xC0, CustomCalibrationStatus.Reserved)]
        public void Parse_GetSetParameters_ExpectedCustomCalStatus(int val, CustomCalibrationStatus expCustomCal)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expCustomCal, bicyclePower.PowerSensor.Parameters.Crank.CustomCalibration);
        }

        [TestMethod]
        [DataRow(0, 0.0)]
        [DataRow(200, 100.0)]
        [DataRow(201, double.NaN)]
        public void Parse_GetSetParameters_ExpectedPowerPhaseConfig(int val, double expPeak)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.PowerPhaseConfiguration, (byte)val, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPeak, bicyclePower.PowerSensor.Parameters.PeakTorqueThreshold);
        }

        [TestMethod]
        [DataRow(0, 0.0)]
        [DataRow(200, 100.0)]
        [DataRow(201, double.NaN)]
        public void Parse_GetSetParameters_ExpectedPeakTorqueThreshold(int val, double expPeak)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.PowerPhaseConfiguration, (byte)val, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPeak, bicyclePower.PowerSensor.Parameters.PeakTorqueThreshold);
        }

        [TestMethod]
        public void Parse_GetSetParametersRiderPositionConfiguration_ExpectedRiderPositionOffset()
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.RiderPositionConfiguration, 128, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(bicyclePower.PowerSensor.Parameters.RiderPositionTimeOffset == 128);
        }

        [TestMethod]
        [DataRow(0, AdvCapabilities1.InteroperableCapabilities.None)]
        [DataRow(0x01, AdvCapabilities1.InteroperableCapabilities.FourHz)]
        [DataRow(0x02, AdvCapabilities1.InteroperableCapabilities.EightHz)]
        [DataRow(0x10, AdvCapabilities1.InteroperableCapabilities.AutoZero)]
        [DataRow(0x20, AdvCapabilities1.InteroperableCapabilities.AutoCrankLength)]
        [DataRow(0x40, AdvCapabilities1.InteroperableCapabilities.TEandPS)]
        [DataRow(0xFF,
            AdvCapabilities1.InteroperableCapabilities.FourHz |
            AdvCapabilities1.InteroperableCapabilities.EightHz |
            AdvCapabilities1.InteroperableCapabilities.AutoZero |
            AdvCapabilities1.InteroperableCapabilities.AutoCrankLength |
            AdvCapabilities1.InteroperableCapabilities.TEandPS
            )]
        public void Parse_GetSetParametersAdvCap1_ExpectedMask(int mask, AdvCapabilities1.InteroperableCapabilities expMask)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities1, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expMask, bicyclePower.PowerSensor.Parameters.AdvancedCapabilities1.Mask);
        }

        [TestMethod]
        [DataRow(0, AdvCapabilities1.InteropProp.None)]
        [DataRow(0x01, AdvCapabilities1.InteropProp.DefaultCrankLength)]
        [DataRow(0x02, AdvCapabilities1.InteropProp.RequiresCrankLength)]
        [DataRow(0x03, AdvCapabilities1.InteropProp.DefaultCrankLength | AdvCapabilities1.InteropProp.RequiresCrankLength)]
        public void Parse_GetSetParametersAdvCap1_ExpectedProperties(int value, AdvCapabilities1.InteropProp expProp)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities1, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expProp, bicyclePower.PowerSensor.Parameters.AdvancedCapabilities1.InteroperableProperties);
        }

        [TestMethod]
        [DataRow(0, AdvCapabilities1.InteroperableCapabilities.None)]
        [DataRow(0x01, AdvCapabilities1.InteroperableCapabilities.FourHz)]
        [DataRow(0x02, AdvCapabilities1.InteroperableCapabilities.EightHz)]
        [DataRow(0x10, AdvCapabilities1.InteroperableCapabilities.AutoZero)]
        [DataRow(0x20, AdvCapabilities1.InteroperableCapabilities.AutoCrankLength)]
        [DataRow(0x40, AdvCapabilities1.InteroperableCapabilities.TEandPS)]
        [DataRow(0xFF,
            AdvCapabilities1.InteroperableCapabilities.FourHz |
            AdvCapabilities1.InteroperableCapabilities.EightHz |
            AdvCapabilities1.InteroperableCapabilities.AutoZero |
            AdvCapabilities1.InteroperableCapabilities.AutoCrankLength |
            AdvCapabilities1.InteroperableCapabilities.TEandPS
            )]
        public void Parse_GetSetParametersAdvCap1_ExpectedValue(int value, AdvCapabilities1.InteroperableCapabilities expValue)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities1, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expValue, bicyclePower.PowerSensor.Parameters.AdvancedCapabilities1.Value);
        }

        [TestMethod]
        [DataRow(0, AdvCapabilities2.InteroperableCapabilies.None)]
        [DataRow(0x01, AdvCapabilities2.InteroperableCapabilies.FourHz)]
        [DataRow(0x02, AdvCapabilities2.InteroperableCapabilies.EightHz)]
        [DataRow(0x08, AdvCapabilities2.InteroperableCapabilies.PowerPhase8Hz)]
        [DataRow(0x10, AdvCapabilities2.InteroperableCapabilies.PCO8Hz)]
        [DataRow(0x20, AdvCapabilities2.InteroperableCapabilies.RiderPosition8Hz)]
        [DataRow(0x40, AdvCapabilities2.InteroperableCapabilies.TorqueBarycenter8Hz)]
        [DataRow(0xFF,
            AdvCapabilities2.InteroperableCapabilies.FourHz |
            AdvCapabilities2.InteroperableCapabilies.EightHz |
            AdvCapabilities2.InteroperableCapabilies.PowerPhase8Hz |
            AdvCapabilities2.InteroperableCapabilies.PCO8Hz |
            AdvCapabilities2.InteroperableCapabilies.RiderPosition8Hz |
            AdvCapabilities2.InteroperableCapabilies.TorqueBarycenter8Hz
            )]
        public void Parse_GetSetParametersAdvCap2_ExpectedMask(int mask, AdvCapabilities2.InteroperableCapabilies expMask)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities2, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expMask, bicyclePower.PowerSensor.Parameters.AdvancedCapabilities2.Mask);
        }

        [TestMethod]
        [DataRow(0, AdvCapabilities2.InteroperableCapabilies.None)]
        [DataRow(0x01, AdvCapabilities2.InteroperableCapabilies.FourHz)]
        [DataRow(0x02, AdvCapabilities2.InteroperableCapabilies.EightHz)]
        [DataRow(0x08, AdvCapabilities2.InteroperableCapabilies.PowerPhase8Hz)]
        [DataRow(0x10, AdvCapabilities2.InteroperableCapabilies.PCO8Hz)]
        [DataRow(0x20, AdvCapabilities2.InteroperableCapabilies.RiderPosition8Hz)]
        [DataRow(0x40, AdvCapabilities2.InteroperableCapabilies.TorqueBarycenter8Hz)]
        [DataRow(0xFF,
            AdvCapabilities2.InteroperableCapabilies.FourHz |
            AdvCapabilities2.InteroperableCapabilies.EightHz |
            AdvCapabilities2.InteroperableCapabilies.PowerPhase8Hz |
            AdvCapabilities2.InteroperableCapabilies.PCO8Hz |
            AdvCapabilities2.InteroperableCapabilies.RiderPosition8Hz |
            AdvCapabilities2.InteroperableCapabilies.TorqueBarycenter8Hz
            )]
        public void Parse_GetSetParametersAdvCap2_ExpectedValue(int value, AdvCapabilities2.InteroperableCapabilies expValue)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities2, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expValue, bicyclePower.PowerSensor.Parameters.AdvancedCapabilities2.Value);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(200, 100)]
        [DataRow(0xFF, double.NaN)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedTorqueEffectiveness(int value, double expPct)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, (byte)value, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPct, bicyclePower.PowerSensor.TorqueEffectiveness.LeftTorqueEffectiveness);
            Assert.AreEqual(expPct, bicyclePower.PowerSensor.TorqueEffectiveness.RightTorqueEffectiveness);
        }

        [TestMethod]
        [DataRow(0, 0, 0, false)]
        [DataRow(200, 200, 100, false)]
        [DataRow(0xFF, 0xFF, double.NaN, false)]
        [DataRow(100, 0xFE, 50, true)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedPedalSmoothness(int left, int right, double expPct, bool expCombined)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.PowerOnly;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, 0xFF, 0xFF, (byte)left, (byte)right, 0xFF, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPct, bicyclePower.PowerSensor.TorqueEffectiveness.LeftPedalSmoothness);
            Assert.AreEqual(expPct, bicyclePower.PowerSensor.TorqueEffectiveness.RightPedalSmoothness);
            Assert.AreEqual(expCombined, bicyclePower.PowerSensor.TorqueEffectiveness.CombinedPedalSmoothness);
        }

        [TestMethod]
        [DataRow(64, 192, 90, 270)]
        [DataRow(0xC0, 0xC0, double.NaN, double.NaN)]
        public void Parse_CyclingDynamics_ExpectedAngle(int start, int end, double startAngle, double endAngle)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.CrankTorque;
            bicyclePower.Parse(dataPage);

            // Act
            dataPage = new byte[8] { (byte)DataPage.RightForceAngle, 0xFF, (byte)start, (byte)end, 0xFF, 0xFF, 0xFF, 0xFF };
            bicyclePower.Parse(
                dataPage);
            dataPage[0] = (byte)DataPage.LeftForceAngle;
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(startAngle, bicyclePower.CrankTorqueSensor.RightForceAngle.StartAngle);
            Assert.AreEqual(endAngle, bicyclePower.CrankTorqueSensor.RightForceAngle.EndAngle);
            Assert.AreEqual(startAngle, bicyclePower.CrankTorqueSensor.LeftForceAngle.StartAngle);
            Assert.AreEqual(endAngle, bicyclePower.CrankTorqueSensor.LeftForceAngle.EndAngle);
        }

        [TestMethod]
        [DataRow(64, 192, 90, 270)]
        [DataRow(0xC0, 0xC0, double.NaN, double.NaN)]
        public void Parse_CyclingDynamics_ExpectedPeakAngle(int start, int end, double startAngle, double endAngle)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.CrankTorque;
            bicyclePower.Parse(dataPage);

            // Act
            dataPage = new byte[8] { (byte)DataPage.RightForceAngle, 0xFF, 0xFF, 0xFF, (byte)start, (byte)end, 0xFF, 0xFF };
            bicyclePower.Parse(
                dataPage);
            dataPage[0] = (byte)DataPage.LeftForceAngle;
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(startAngle, bicyclePower.CrankTorqueSensor.RightForceAngle.StartPeakAngle);
            Assert.AreEqual(endAngle, bicyclePower.CrankTorqueSensor.RightForceAngle.EndPeakAngle);
            Assert.AreEqual(startAngle, bicyclePower.CrankTorqueSensor.LeftForceAngle.StartPeakAngle);
            Assert.AreEqual(endAngle, bicyclePower.CrankTorqueSensor.LeftForceAngle.EndPeakAngle);
        }

        [TestMethod]
        [DataRow(0x00, Position.Seated)]
        [DataRow(0x40, Position.TransitionToSeated)]
        [DataRow(0x80, Position.Standing)]
        [DataRow(0xC0, Position.TransitionToStanding)]
        public void Parse_CyclingDynamicsPedalPosition_ExpectedRiderPosition(int pos, Position expPos)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.CrankTorque;
            bicyclePower.Parse(dataPage);

            // Act
            dataPage = new byte[8] { (byte)DataPage.PedalPosition, 0xFF, (byte)pos, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPos, bicyclePower.CrankTorqueSensor.PedalPosition.RiderPosition);
        }

        [TestMethod]
        public void Parse_CyclingDynamicsPedalPosition_ExpectedCadencePCO()
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.CrankTorque;
            bicyclePower.Parse(dataPage);

            // Act
            dataPage = new byte[8] { (byte)DataPage.PedalPosition, 0xFF, 0xFF, 128, 64, 0xE0, 0xFF, 0xFF };
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(bicyclePower.CrankTorqueSensor.PedalPosition.AverageCadence == 128);
            Assert.IsTrue(bicyclePower.CrankTorqueSensor.PedalPosition.RightPlatformCenterOffset == 64);
            Assert.IsTrue(bicyclePower.CrankTorqueSensor.PedalPosition.LeftPlatformCenterOffset == -32);
        }

        [TestMethod]
        [DataRow(0, 30.0)]
        [DataRow(128, 94.0)]
        [DataRow(255, 157.5)]
        public void Parse_CyclingDynamicsTorqueBarycenter_Expected(int val, double expBarycenterTorque)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.CrankTorque;
            bicyclePower.Parse(dataPage);
            dataPage = new byte[8] { (byte)DataPage.TorqueBarycenter, (byte)val, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expBarycenterTorque, bicyclePower.CrankTorqueSensor.TorqueBarycenterAngle);
        }

        [TestMethod]
        public void Parse_CrankTorqueSensorMessage_ExpectedValues()
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.CrankTorque;
            bicyclePower.Parse(dataPage);

            byte expInstCad = 60;
            double expAvgCad = 60;
            double expAvgAngVel = 2 * Math.PI;
            double expAvgTorq = 44.875;
            double expAvgPow = 282;
            dataPage = new byte[8] { (byte)DataPage.CrankTorque, 1, 1, 60, 0x00, 0x08, 0x9C, 0x05 };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(bicyclePower.CrankTorqueSensor.AverageCadence == expAvgCad);

            Assert.IsTrue(bicyclePower.CrankTorqueSensor.InstantaneousCadence == expInstCad);
            Assert.IsTrue(bicyclePower.CrankTorqueSensor.AverageAngularVelocity == expAvgAngVel);
            Assert.IsTrue(bicyclePower.CrankTorqueSensor.AverageTorque == expAvgTorq);
            Assert.IsTrue(Math.Round(bicyclePower.CrankTorqueSensor.AveragePower) == expAvgPow);
        }

        [TestMethod]
        public void Parse_WheelTorqueSensorMessage_ExpectedValues()
        {
            // Arrange
            var bicyclePower = CreateBicyclePower();
            byte[] dataPage = new byte[8];
            dataPage[0] = (byte)DataPage.WheelTorque;
            bicyclePower.Parse(dataPage);

            byte expInstCad = 60;
            double expAvgSpeed = 15;
            double expAvgPower = 178;
            double expAvgAngVel = 2 * Math.PI / (0x439 / 2048.0);
            double expAvgTorq = 14.9375;
            double expDist = 2.2;
            dataPage = new byte[8] { (byte)DataPage.WheelTorque, 1, 1, 60, 0x39, 0x04, 0xDE, 0x01 };

            // Act
            bicyclePower.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(bicyclePower.WheelTorqueSensor.AccumulatedDistance == expDist);
            Assert.IsTrue(Math.Round(bicyclePower.WheelTorqueSensor.AverageSpeed, 2) == expAvgSpeed);

            Assert.IsTrue(bicyclePower.WheelTorqueSensor.InstantaneousCadence == expInstCad);
            Assert.IsTrue(bicyclePower.WheelTorqueSensor.AverageAngularVelocity == expAvgAngVel);
            Assert.IsTrue(bicyclePower.WheelTorqueSensor.AverageTorque == expAvgTorq);
            Assert.IsTrue(Math.Round(bicyclePower.WheelTorqueSensor.AveragePower) == expAvgPower);
        }
    }
}
