using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System.Threading.Tasks;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class ParametersTests
    {
        private readonly MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<NullLoggerFactory> mockLogger;

        public ParametersTests()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<NullLoggerFactory>();
        }

        private StandardPowerSensor CreateStandardPowerSensor()
        {
            byte[] page = [(byte)BicyclePower.DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0];
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, 2000) as StandardPowerSensor;
        }

        [Theory]
        [InlineData(0x00, 110.0)]
        [InlineData(0xFD, 236.5)]
        [InlineData(0xFE, double.NaN)]
        [InlineData(0xFF, double.NaN)]
        public void Parse_CrankParameters_ExpectedCrankLength(int val, double expCrankLen)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, (byte)val, 0, 0, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expCrankLen, sensor.Crank.CrankLength);
        }

        [Theory]
        [InlineData(0, StandardPowerSensor.CrankParameters.CrankLengthStatus.Invalid)]
        [InlineData(1, StandardPowerSensor.CrankParameters.CrankLengthStatus.Default)]
        [InlineData(2, StandardPowerSensor.CrankParameters.CrankLengthStatus.ManuallySet)]
        [InlineData(3, StandardPowerSensor.CrankParameters.CrankLengthStatus.AutoOrFixed)]
        public void Parse_CrankParameters_ExpectedCrankStatus(int val, StandardPowerSensor.CrankParameters.CrankLengthStatus expCrankStatus)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expCrankStatus, sensor.Crank.CrankStatus);
        }

        [Theory]
        [InlineData(0x00, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.Undefined)]
        [InlineData(0x04, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.RightSensorOlder)]
        [InlineData(0x08, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.LeftSensorOlder)]
        [InlineData(0x0C, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.Identical)]
        public void Parse_CrankParameters_ExpectedSensorMismatchStatus(int val, StandardPowerSensor.CrankParameters.SensorMisMatchStatus expSensorSWStatus)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expSensorSWStatus, sensor.Crank.MismatchStatus);
        }

        [Theory]
        [InlineData(0x00, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.Undefined)]
        [InlineData(0x10, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.LeftPresent)]
        [InlineData(0x20, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.RightPresent)]
        [InlineData(0x30, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.BothPresent)]
        public void Parse_CrankParameters_ExpectedSensorAvailabilityStatus(int val, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus expSensorAvailability)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expSensorAvailability, sensor.Crank.AvailabilityStatus);
        }

        [Theory]
        [InlineData(0x00, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.NotSupported)]
        [InlineData(0x40, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.NotRequired)]
        [InlineData(0x80, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.Required)]
        [InlineData(0xC0, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.Reserved)]
        public void Parse_CrankParameters_ExpectedCustomCalStatus(int val, StandardPowerSensor.CrankParameters.CustomCalibrationStatus expCustomCal)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expCustomCal, sensor.Crank.CustomCalibration);
        }

        [Theory]
        [InlineData(0, 0.0)]
        [InlineData(200, 100.0)]
        [InlineData(201, double.NaN)]
        public void Parse_PowerPhaseParameters_ExpectedPeakTorqueThreshold(int val, double expPeak)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.PowerPhaseConfiguration, (byte)val, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expPeak, sensor.PeakTorqueThreshold);
        }

        [Fact]
        public void Parse_RiderPositionConfigurationParameters_ExpectedRiderPositionOffset()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.RiderPositionConfiguration, 128, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(128, sensor.RiderPositionTimeOffset);
        }

        [Theory]
        [InlineData(0, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.None)]
        [InlineData(0x01, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.FourHz)]
        [InlineData(0x02, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.EightHz)]
        [InlineData(0x10, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoZero)]
        [InlineData(0x20, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoCrankLength)]
        [InlineData(0x40, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.TEandPS)]
        [InlineData(0xFF,
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
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities1, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expMask, sensor.AdvancedCapabilities1.Mask);
        }

        [Theory]
        [InlineData(0, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.None)]
        [InlineData(0x01, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.FourHz)]
        [InlineData(0x02, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.EightHz)]
        [InlineData(0x10, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoZero)]
        [InlineData(0x20, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.AutoCrankLength)]
        [InlineData(0x40, StandardPowerSensor.AdvCapabilities1.InteroperableCapabilities.TEandPS)]
        [InlineData(0xFF,
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
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities1, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expValue, sensor.AdvancedCapabilities1.Value);
        }

        [Theory]
        [InlineData(0, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.None)]
        [InlineData(0x01, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.FourHz)]
        [InlineData(0x02, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.EightHz)]
        [InlineData(0x08, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz)]
        [InlineData(0x10, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PCO8Hz)]
        [InlineData(0x20, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz)]
        [InlineData(0x40, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz)]
        [InlineData(0xFF,
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
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities2, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expMask, sensor.AdvancedCapabilities2.Mask);
        }

        [Theory]
        [InlineData(0, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.None)]
        [InlineData(0x01, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.FourHz)]
        [InlineData(0x02, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.EightHz)]
        [InlineData(0x08, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PowerPhase8Hz)]
        [InlineData(0x10, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.PCO8Hz)]
        [InlineData(0x20, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.RiderPosition8Hz)]
        [InlineData(0x40, StandardPowerSensor.AdvCapabilities2.InteroperableCapabilities.TorqueBarycenter8Hz)]
        [InlineData(0xFF,
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
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities2, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expValue, sensor.AdvancedCapabilities2.Value);
        }

        [Theory]
        [InlineData(0, StandardPowerSensor.AdvCapabilities1.InteropProp.None)]
        [InlineData(0x01, StandardPowerSensor.AdvCapabilities1.InteropProp.DefaultCrankLength)]
        [InlineData(0x02, StandardPowerSensor.AdvCapabilities1.InteropProp.RequiresCrankLength)]
        [InlineData(0x03, StandardPowerSensor.AdvCapabilities1.InteropProp.DefaultCrankLength | StandardPowerSensor.AdvCapabilities1.InteropProp.RequiresCrankLength)]
        public void Parse_AdvCap1Parameters_ExpectedProperties(int value, StandardPowerSensor.AdvCapabilities1.InteropProp expProp)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities1, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expProp, sensor.AdvancedCapabilities1.InteroperableProperties);
        }

        [Theory]
        [InlineData(SubPage.CrankParameters)]
        [InlineData(SubPage.AdvancedCapabilities1)]
        [InlineData(SubPage.AdvancedCapabilities2)]
        [InlineData(SubPage.RiderPositionConfiguration)]
        [InlineData(SubPage.PowerPhaseConfiguration)]
        public async Task GetParameters_RequestedSubpage_ExpectedDataPage(SubPage page)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [
                (byte)CommonDataPage.RequestDataPage,
                0xFF,
                0xFF,
                (byte)page,
                0xFF,
                4,
                (byte)BicyclePower.DataPage.GetSetParameters,
                (byte)SmallEarthTech.AntPlus.CommandType.DataPage
            ];
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.GetParameters(
                page);

            // Assert
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(172.5)]
        [InlineData(237)]
        public async Task SetCrankLength_Message_ExpectedDataPage(double length)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte expVal = (byte)((length - 110) / 0.5);
            byte[] dataPage = [
                (byte)BicyclePower.DataPage.GetSetParameters,
                (byte)SubPage.CrankParameters,
                0xFF,
                0xFF,
                expVal,
                0x00,
                0x00,
                0xFF
            ];
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.SetCrankLength(
                length);

            // Assert
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task SetTransitionTimeOffset_Message_ExpectedDataPage()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte offset = 128;
            byte[] dataPage = [
                (byte)BicyclePower.DataPage.GetSetParameters,
                (byte)SubPage.RiderPositionConfiguration,
                offset,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF
            ];
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, dataPage, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            await sensor.SetTransitionTimeOffset(
                offset);

            // Assert
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task SetPeakTorqueThreshold_Message_ExpectedDataPage()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            double threshold = 50;
            byte[] dataPage = [
                (byte)BicyclePower.DataPage.GetSetParameters,
                (byte)SubPage.PowerPhaseConfiguration,
                (byte)(threshold / 0.5),
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF
            ];
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
