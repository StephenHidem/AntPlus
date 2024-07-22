using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System.Data;
using System.Threading.Tasks;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    [TestClass]
    public class ParametersTests
    {
        private MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<NullLoggerFactory> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<NullLoggerFactory>();
        }

        private StandardPowerSensor CreateStandardPowerSensor()
        {
            byte[] page = new byte[8] { (byte)DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, missedMessages: 8) as StandardPowerSensor;
        }

        [TestMethod]
        [DataRow(0x00, 110.0)]
        [DataRow(0xFD, 236.5)]
        [DataRow(0xFE, double.NaN)]
        [DataRow(0xFF, double.NaN)]
        public void Parse_CrankParameters_ExpectedCrankLength(int val, double expCrankLen)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, (byte)val, 0, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expCrankLen, sensor.Crank.CrankLength);
        }

        [TestMethod]
        [DataRow(0, StandardPowerSensor.CrankParameters.CrankLengthStatus.Invalid)]
        [DataRow(1, StandardPowerSensor.CrankParameters.CrankLengthStatus.Default)]
        [DataRow(2, StandardPowerSensor.CrankParameters.CrankLengthStatus.ManuallySet)]
        [DataRow(3, StandardPowerSensor.CrankParameters.CrankLengthStatus.AutoOrFixed)]
        public void Parse_CrankParameters_ExpectedCrankStatus(int val, StandardPowerSensor.CrankParameters.CrankLengthStatus expCrankStatus)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expCrankStatus, sensor.Crank.CrankStatus);
        }

        [TestMethod]
        [DataRow(0x00, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.Undefined)]
        [DataRow(0x04, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.RightSensorOlder)]
        [DataRow(0x08, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.LeftSensorOlder)]
        [DataRow(0x0C, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.Identical)]
        public void Parse_CrankParameters_ExpectedSensorMismatchStatus(int val, StandardPowerSensor.CrankParameters.SensorMisMatchStatus expSensorSWStatus)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expSensorSWStatus, sensor.Crank.MismatchStatus);
        }

        [TestMethod]
        [DataRow(0x00, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.Undefined)]
        [DataRow(0x10, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.LeftPresent)]
        [DataRow(0x20, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.RightPresent)]
        [DataRow(0x30, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.BothPresent)]
        public void Parse_CrankParameters_ExpectedSensorAvailabilityStatus(int val, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus expSensorAvailability)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expSensorAvailability, sensor.Crank.AvailabilityStatus);
        }

        [TestMethod]
        [DataRow(0x00, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.NotSupported)]
        [DataRow(0x40, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.NotRequired)]
        [DataRow(0x80, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.Required)]
        [DataRow(0xC0, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.Reserved)]
        public void Parse_CrankParameters_ExpectedCustomCalStatus(int val, StandardPowerSensor.CrankParameters.CustomCalibrationStatus expCustomCal)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expCustomCal, sensor.Crank.CustomCalibration);
        }

        [TestMethod]
        [DataRow(0, 0.0)]
        [DataRow(200, 100.0)]
        [DataRow(201, double.NaN)]
        public void Parse_PowerPhaseParameters_ExpectedPeakTorqueThreshold(int val, double expPeak)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.PowerPhaseConfiguration, (byte)val, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPeak, sensor.PeakTorqueThreshold);
        }

        [TestMethod]
        public void Parse_RiderPositionConfigurationParameters_ExpectedRiderPositionOffset()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.RiderPositionConfiguration, 128, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(128, sensor.RiderPositionTimeOffset);
        }

        [TestMethod]
        [DataRow(0, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.None)]
        [DataRow(0x01, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.FourHz)]
        [DataRow(0x02, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.EightHz)]
        [DataRow(0x10, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoZero)]
        [DataRow(0x20, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoCrankLength)]
        [DataRow(0x40, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.TEandPS)]
        [DataRow(0xFF,
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.FourHz |
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.EightHz |
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoZero |
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoCrankLength |
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.TEandPS
            )]
        public void Parse_AdvCap1Parameters_ExpectedMask(int mask, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities expMask)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities1, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expMask, sensor.AdvancedCapabilities1.Mask);
        }

        [TestMethod]
        [DataRow(0, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.None)]
        [DataRow(0x01, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.FourHz)]
        [DataRow(0x02, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.EightHz)]
        [DataRow(0x10, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoZero)]
        [DataRow(0x20, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoCrankLength)]
        [DataRow(0x40, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.TEandPS)]
        [DataRow(0xFF,
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.FourHz |
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.EightHz |
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoZero |
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoCrankLength |
            StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.TEandPS
            )]
        public void Parse_AdvCap1Parameters_ExpectedValue(int value, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities expValue)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities1, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expValue, sensor.AdvancedCapabilities1.Value);
        }

        [TestMethod]
        [DataRow(0, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.None)]
        [DataRow(0x01, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.FourHz)]
        [DataRow(0x02, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.EightHz)]
        [DataRow(0x08, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz)]
        [DataRow(0x10, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PCO8Hz)]
        [DataRow(0x20, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz)]
        [DataRow(0x40, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz)]
        [DataRow(0xFF,
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.FourHz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.EightHz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PCO8Hz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz
            )]
        public void Parse_AdvCap2Parameters_ExpectedMask(int mask, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities expMask)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities2, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expMask, sensor.AdvancedCapabilities2.Mask);
        }

        [TestMethod]
        [DataRow(0, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.None)]
        [DataRow(0x01, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.FourHz)]
        [DataRow(0x02, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.EightHz)]
        [DataRow(0x08, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz)]
        [DataRow(0x10, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PCO8Hz)]
        [DataRow(0x20, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz)]
        [DataRow(0x40, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz)]
        [DataRow(0xFF,
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.FourHz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.EightHz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PCO8Hz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz |
            StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz
            )]
        public void Parse_AdvCap2Parameters_ExpectedValue(int value, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities expValue)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities2, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expValue, sensor.AdvancedCapabilities2.Value);
        }

        [TestMethod]
        [DataRow(0, StandardPowerSensor.AdvCapabilities1.InteropProp.None)]
        [DataRow(0x01, StandardPowerSensor.AdvCapabilities1.InteropProp.DefaultCrankLength)]
        [DataRow(0x02, StandardPowerSensor.AdvCapabilities1.InteropProp.RequiresCrankLength)]
        [DataRow(0x03, StandardPowerSensor.AdvCapabilities1.InteropProp.DefaultCrankLength | StandardPowerSensor.AdvCapabilities1.InteropProp.RequiresCrankLength)]
        public void Parse_AdvCap1Parameters_ExpectedProperties(int value, StandardPowerSensor.AdvCapabilities1.InteropProp expProp)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities1, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expProp, sensor.AdvancedCapabilities1.InteroperableProperties);
        }

        [TestMethod]
        [DataRow(SubPage.CrankParameters)]
        [DataRow(SubPage.AdvancedCapabilities1)]
        [DataRow(SubPage.AdvancedCapabilities2)]
        [DataRow(SubPage.RiderPositionConfiguration)]
        [DataRow(SubPage.PowerPhaseConfiguration)]
        public async Task GetParameters_RequestedSubpage_ExpectedDataPage(SubPage page)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] {
                (byte)CommonDataPage.RequestDataPage,
                0xFF,
                0xFF,
                (byte)page,
                0xFF,
                4,
                (byte)DataPage.GetSetParameters,
                (byte)SmallEarthTech.AntPlus.CommandType.DataPage
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.GetParameters(
                page);

            // Assert
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(172.5)]
        [DataRow(237)]
        public async Task SetCrankLength_Message_ExpectedDataPage(double length)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte expVal = (byte)((length - 110) / 0.5);
            byte[] dataPage = new byte[8] {
                (byte)DataPage.GetSetParameters,
                (byte)SubPage.CrankParameters,
                0xFF,
                0xFF,
                expVal,
                0x00,
                0x00,
                0xFF
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.SetCrankLength(
                length);

            // Assert
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task SetTransitionTimeOffset_Message_ExpectedDataPage()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte offset = 128;
            byte[] dataPage = new byte[8] {
                (byte)DataPage.GetSetParameters,
                (byte)SubPage.RiderPositionConfiguration,
                offset,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.SetTransitionTimeOffset(
                offset);

            // Assert
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task SetPeakTorqueThreshold_Message_ExpectedDataPage()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            double threshold = 50;
            byte[] dataPage = new byte[8] {
                (byte)DataPage.GetSetParameters,
                (byte)SubPage.PowerPhaseConfiguration,
                (byte)(threshold / 0.5),
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.SetPeakTorqueThreshold(
                threshold);

            // Assert
            mockRepository.VerifyAll();
        }
    }
}
