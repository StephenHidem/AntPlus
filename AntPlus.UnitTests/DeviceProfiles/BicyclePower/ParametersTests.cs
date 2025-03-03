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
        private readonly MockRepository _mockRepository;
        private readonly ChannelId _channelId = new(0);
        private readonly Mock<IAntChannel> _mockAntChannel;
        private readonly StandardPowerSensor _standardPowerSensor;

        public ParametersTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose);
            _mockAntChannel = _mockRepository.Create<IAntChannel>();
            byte[] page = [(byte)BicyclePower.DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0];
            _standardPowerSensor = BicyclePower.GetBicyclePowerSensor(page, _channelId, _mockAntChannel.Object, Mock.Of<NullLoggerFactory>(), 2000) as StandardPowerSensor;
        }

        [Theory]
        [InlineData(0x00, 110.0)]
        [InlineData(0xFD, 236.5)]
        [InlineData(0xFE, double.NaN)]
        [InlineData(0xFF, double.NaN)]
        public void Parse_CrankParameters_ExpectedCrankLength(int val, double expCrankLen)
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, (byte)val, 0, 0, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expCrankLen, _standardPowerSensor.Crank.CrankLength);
        }

        [Theory]
        [InlineData(0, StandardPowerSensor.CrankParameters.CrankLengthStatus.Invalid)]
        [InlineData(1, StandardPowerSensor.CrankParameters.CrankLengthStatus.Default)]
        [InlineData(2, StandardPowerSensor.CrankParameters.CrankLengthStatus.ManuallySet)]
        [InlineData(3, StandardPowerSensor.CrankParameters.CrankLengthStatus.AutoOrFixed)]
        public void Parse_CrankParameters_ExpectedCrankStatus(int val, StandardPowerSensor.CrankParameters.CrankLengthStatus expCrankStatus)
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expCrankStatus, _standardPowerSensor.Crank.CrankStatus);
        }

        [Theory]
        [InlineData(0x00, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.Undefined)]
        [InlineData(0x04, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.RightSensorOlder)]
        [InlineData(0x08, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.LeftSensorOlder)]
        [InlineData(0x0C, StandardPowerSensor.CrankParameters.SensorMisMatchStatus.Identical)]
        public void Parse_CrankParameters_ExpectedSensorMismatchStatus(int val, StandardPowerSensor.CrankParameters.SensorMisMatchStatus expSensorSWStatus)
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expSensorSWStatus, _standardPowerSensor.Crank.MismatchStatus);
        }

        [Theory]
        [InlineData(0x00, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.Undefined)]
        [InlineData(0x10, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.LeftPresent)]
        [InlineData(0x20, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.RightPresent)]
        [InlineData(0x30, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus.BothPresent)]
        public void Parse_CrankParameters_ExpectedSensorAvailabilityStatus(int val, StandardPowerSensor.CrankParameters.SensorAvailabilityStatus expSensorAvailability)
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expSensorAvailability, _standardPowerSensor.Crank.AvailabilityStatus);
        }

        [Theory]
        [InlineData(0x00, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.NotSupported)]
        [InlineData(0x40, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.NotRequired)]
        [InlineData(0x80, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.Required)]
        [InlineData(0xC0, StandardPowerSensor.CrankParameters.CustomCalibrationStatus.Reserved)]
        public void Parse_CrankParameters_ExpectedCustomCalStatus(int val, StandardPowerSensor.CrankParameters.CustomCalibrationStatus expCustomCal)
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.CrankParameters, 0xFF, 0xFF, 0, (byte)val, 0, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expCustomCal, _standardPowerSensor.Crank.CustomCalibration);
        }

        [Theory]
        [InlineData(0, 0.0)]
        [InlineData(200, 100.0)]
        [InlineData(201, double.NaN)]
        public void Parse_PowerPhaseParameters_ExpectedPeakTorqueThreshold(int val, double expPeak)
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.PowerPhaseConfiguration, (byte)val, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expPeak, _standardPowerSensor.PeakTorqueThreshold);
        }

        [Fact]
        public void Parse_RiderPositionConfigurationParameters_ExpectedRiderPositionOffset()
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.RiderPositionConfiguration, 128, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(128, _standardPowerSensor.RiderPositionTimeOffset);
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
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities1, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expMask, _standardPowerSensor.AdvancedCapabilities1.Mask);
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
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities1, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expValue, _standardPowerSensor.AdvancedCapabilities1.Value);
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
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities2, 0xFF, 0xFF, (byte)mask, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expMask, _standardPowerSensor.AdvancedCapabilities2.Mask);
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
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities2, 0xFF, 0xFF, 0xFF, 0xFF, (byte)value, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expValue, _standardPowerSensor.AdvancedCapabilities2.Value);
        }

        [Theory]
        [InlineData(0, StandardPowerSensor.AdvCapabilities1.InteropProp.None)]
        [InlineData(0x01, StandardPowerSensor.AdvCapabilities1.InteropProp.DefaultCrankLength)]
        [InlineData(0x02, StandardPowerSensor.AdvCapabilities1.InteropProp.RequiresCrankLength)]
        [InlineData(0x03, StandardPowerSensor.AdvCapabilities1.InteropProp.DefaultCrankLength | StandardPowerSensor.AdvCapabilities1.InteropProp.RequiresCrankLength)]
        public void Parse_AdvCap1Parameters_ExpectedProperties(int value, StandardPowerSensor.AdvCapabilities1.InteropProp expProp)
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.GetSetParameters, (byte)SubPage.AdvancedCapabilities1, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expProp, _standardPowerSensor.AdvancedCapabilities1.InteroperableProperties);
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
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(_channelId, dataPage, It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);

            // Act
            await _standardPowerSensor.GetParameters(page);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(172.5)]
        [InlineData(237)]
        public async Task SetCrankLength_Message_ExpectedDataPage(double length)
        {
            // Arrange
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
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(_channelId, dataPage, It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);

            // Act
            await _standardPowerSensor.SetCrankLength(length);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public async Task SetTransitionTimeOffset_Message_ExpectedDataPage()
        {
            // Arrange
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
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(_channelId, dataPage, It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);

            // Act
            await _standardPowerSensor.SetTransitionTimeOffset(offset);

            // Assert
            _mockRepository.VerifyAll();
        }

        [Fact]
        public async Task SetPeakTorqueThreshold_Message_ExpectedDataPage()
        {
            // Arrange
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
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(_channelId, dataPage, It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);

            // Act
            await _standardPowerSensor.SetPeakTorqueThreshold(threshold);

            // Assert
            _mockRepository.VerifyAll();
        }
    }
}
