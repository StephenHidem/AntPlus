using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.StandardPowerSensor;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class StandardPowerSensorTests
    {
        private readonly MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<NullLoggerFactory> mockLogger;

        public StandardPowerSensorTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<NullLoggerFactory>(MockBehavior.Loose);
        }

        private StandardPowerSensor CreateStandardPowerSensor()
        {
            byte[] page = [(byte)BicyclePower.DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0];
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, 2000) as StandardPowerSensor;
        }

        [Theory]
        [InlineData(0x32, 50, PedalDifferentiation.Unknown)]
        [InlineData(0xB2, 50, PedalDifferentiation.RightPedal)]
        [InlineData(0xFF, 0x7F, PedalDifferentiation.Unused)]
        public void Parse_PedalPower_ExpectedPedalPower(int value, int percent, PedalDifferentiation pedalDifferentiation)
        {
            // Arrange
            var standardPowerSensor = CreateStandardPowerSensor();
            byte[] dataPage = [16, 1, (byte)value, 0, 0, 0, 0, 0];

            // Act
            standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(percent, standardPowerSensor.PedalPower);
            Assert.Equal(pedalDifferentiation, standardPowerSensor.PedalContribution);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_InstantaneousCadence_ExpectedCadence()
        {
            // Arrange
            var standardPowerSensor = CreateStandardPowerSensor();
            byte[] dataPage = [16, 1, 0, 128, 0, 0, 0, 0];

            // Act
            standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(128, standardPowerSensor.InstantaneousCadence);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_InstantaneousPower_ExpectedPower()
        {
            // Arrange
            var standardPowerSensor = CreateStandardPowerSensor();
            byte[] dataPage = [16, 1, 0, 0, 0, 0, 0x11, 0x22];

            // Act
            standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(8721, standardPowerSensor.InstantaneousPower);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_AveragePower_ExpectedAvgPower()
        {
            // Arrange
            var standardPowerSensor = CreateStandardPowerSensor();
            byte[] dataPage = [16, 1, 0, 0, 0x11, 0x22, 0, 0];

            // Act
            standardPowerSensor.Parse(
                [16, 0, 0, 0, 0, 0, 0, 0]);
            standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(8721, standardPowerSensor.AveragePower);
            mockRepository.VerifyAll();
        }
    }
}
