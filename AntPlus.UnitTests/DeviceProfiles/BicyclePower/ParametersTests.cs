using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System.Data;
using System.Threading.Tasks;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.Parameters;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.Parameters.CrankParameters;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    [TestClass]
    public class ParametersTests
    {
        private MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<BicyclePower>> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<BicyclePower>>();
        }

        private StandardPowerSensor CreateStandardPowerSensor()
        {
            byte[] page = new byte[8] { (byte)DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object) as StandardPowerSensor;
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
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, (byte)val, 0, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expCrankLen, sensor.Parameters.Crank.CrankLength);
        }

        [TestMethod]
        [DataRow(0, CrankLengthStatus.Invalid)]
        [DataRow(1, CrankLengthStatus.Default)]
        [DataRow(2, CrankLengthStatus.ManuallySet)]
        [DataRow(3, CrankLengthStatus.AutoOrFixed)]
        public void Parse_CrankParameters_ExpectedCrankStatus(int val, CrankLengthStatus expCrankStatus)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expCrankStatus, sensor.Parameters.Crank.CrankStatus);
        }

        [TestMethod]
        [DataRow(0x00, SensorMisMatchStatus.Undefined)]
        [DataRow(0x04, SensorMisMatchStatus.RightSensorOlder)]
        [DataRow(0x08, SensorMisMatchStatus.LeftSensorOlder)]
        [DataRow(0x0C, SensorMisMatchStatus.Identical)]
        public void Parse_CrankParameters_ExpectedSensorMismatchStatus(int val, SensorMisMatchStatus expSensorSWStatus)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expSensorSWStatus, sensor.Parameters.Crank.MismatchStatus);
        }

        [TestMethod]
        [DataRow(0x00, SensorAvailabilityStatus.Undefined)]
        [DataRow(0x10, SensorAvailabilityStatus.LeftPresent)]
        [DataRow(0x20, SensorAvailabilityStatus.RightPresent)]
        [DataRow(0x30, SensorAvailabilityStatus.BothPresent)]
        public void Parse_CrankParameters_ExpectedSensorAvailabilityStatus(int val, SensorAvailabilityStatus expSensorAvailability)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expSensorAvailability, sensor.Parameters.Crank.AvailabilityStatus);
        }

        [TestMethod]
        [DataRow(0x00, CustomCalibrationStatus.NotSupported)]
        [DataRow(0x40, CustomCalibrationStatus.NotRequired)]
        [DataRow(0x80, CustomCalibrationStatus.Required)]
        [DataRow(0xC0, CustomCalibrationStatus.Reserved)]
        public void Parse_CrankParameters_ExpectedCustomCalStatus(int val, CustomCalibrationStatus expCustomCal)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expCustomCal, sensor.Parameters.Crank.CustomCalibration);
        }

        [TestMethod]
        [DataRow(0, 0.0)]
        [DataRow(200, 100.0)]
        [DataRow(201, double.NaN)]
        public void Parse_PowerPhaseParameters_ExpectedPeakTorqueThreshold(int val, double expPeak)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.PowerPhaseConfiguration, (byte)val, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPeak, sensor.Parameters.PeakTorqueThreshold);
        }

        [TestMethod]
        public void Parse_RiderPositionConfigurationParameters_ExpectedRiderPositionOffset()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.RiderPositionConfiguration, 128, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(128, sensor.Parameters.RiderPositionTimeOffset);
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
        public void Parse_AdvCap1Parameters_ExpectedMask(int mask, AdvCapabilities1.InteroperableCapabilities expMask)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities1, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expMask, sensor.Parameters.AdvancedCapabilities1.Mask);
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
        public void Parse_AdvCap1Parameters_ExpectedValue(int value, AdvCapabilities1.InteroperableCapabilities expValue)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities1, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expValue, sensor.Parameters.AdvancedCapabilities1.Value);
        }

        [TestMethod]
        [DataRow(0, AdvCapabilities2.InteroperableCapabilities.None)]
        [DataRow(0x01, AdvCapabilities2.InteroperableCapabilities.FourHz)]
        [DataRow(0x02, AdvCapabilities2.InteroperableCapabilities.EightHz)]
        [DataRow(0x08, AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz)]
        [DataRow(0x10, AdvCapabilities2.InteroperableCapabilities.PCO8Hz)]
        [DataRow(0x20, AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz)]
        [DataRow(0x40, AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz)]
        [DataRow(0xFF,
            AdvCapabilities2.InteroperableCapabilities.FourHz |
            AdvCapabilities2.InteroperableCapabilities.EightHz |
            AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz |
            AdvCapabilities2.InteroperableCapabilities.PCO8Hz |
            AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz |
            AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz
            )]
        public void Parse_AdvCap2Parameters_ExpectedMask(int mask, AdvCapabilities2.InteroperableCapabilities expMask)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities2, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expMask, sensor.Parameters.AdvancedCapabilities2.Mask);
        }

        [TestMethod]
        [DataRow(0, AdvCapabilities2.InteroperableCapabilities.None)]
        [DataRow(0x01, AdvCapabilities2.InteroperableCapabilities.FourHz)]
        [DataRow(0x02, AdvCapabilities2.InteroperableCapabilities.EightHz)]
        [DataRow(0x08, AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz)]
        [DataRow(0x10, AdvCapabilities2.InteroperableCapabilities.PCO8Hz)]
        [DataRow(0x20, AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz)]
        [DataRow(0x40, AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz)]
        [DataRow(0xFF,
            AdvCapabilities2.InteroperableCapabilities.FourHz |
            AdvCapabilities2.InteroperableCapabilities.EightHz |
            AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz |
            AdvCapabilities2.InteroperableCapabilities.PCO8Hz |
            AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz |
            AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz
            )]
        public void Parse_AdvCap2Parameters_ExpectedValue(int value, AdvCapabilities2.InteroperableCapabilities expValue)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities2, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expValue, sensor.Parameters.AdvancedCapabilities2.Value);
        }

        [TestMethod]
        [DataRow(0, AdvCapabilities1.InteropProp.None)]
        [DataRow(0x01, AdvCapabilities1.InteropProp.DefaultCrankLength)]
        [DataRow(0x02, AdvCapabilities1.InteropProp.RequiresCrankLength)]
        [DataRow(0x03, AdvCapabilities1.InteropProp.DefaultCrankLength | AdvCapabilities1.InteropProp.RequiresCrankLength)]
        public void Parse_AdvCap1Parameters_ExpectedProperties(int value, AdvCapabilities1.InteropProp expProp)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.GetSetParameters, (byte)Subpage.AdvancedCapabilities1, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expProp, sensor.Parameters.AdvancedCapabilities1.InteroperableProperties);
        }

        [TestMethod]
        [DataRow(Subpage.CrankParameters)]
        [DataRow(Subpage.AdvancedCapabilities1)]
        [DataRow(Subpage.AdvancedCapabilities2)]
        [DataRow(Subpage.RiderPositionConfiguration)]
        [DataRow(Subpage.PowerPhaseConfiguration)]
        public async Task GetParameters_RequestedSubpage_ExpectedDataPage(Subpage page)
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
                ac.SendExtAcknowledgedData(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.Parameters.GetParameters(
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
                (byte)Subpage.CrankParameters,
                0xFF,
                0xFF,
                expVal,
                0x00,
                0x00,
                0xFF
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.Parameters.SetCrankLength(
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
                (byte)Subpage.RiderPositionConfiguration,
                offset,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.Parameters.SetTransitionTimeOffset(
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
                (byte)Subpage.PowerPhaseConfiguration,
                (byte)(threshold / 0.5),
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.Parameters.SetPeakTorqueThreshold(
                threshold);

            // Assert
            mockRepository.VerifyAll();
        }
    }
}
