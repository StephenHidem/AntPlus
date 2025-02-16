using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class TorqueEffectivenessAndPedalSmoothnessTests
    {
        private readonly MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<NullLoggerFactory> mockLogger;

        public TorqueEffectivenessAndPedalSmoothnessTests()
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
        [InlineData(0, 0)]
        [InlineData(200, 100)]
        [InlineData(0xFF, double.NaN)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedTorqueEffectiveness(int value, double expPct)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, (byte)value, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expPct, sensor.LeftTorqueEffectiveness);
            Assert.Equal(expPct, sensor.RightTorqueEffectiveness);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, false)]
        [InlineData(200, 200, 100, 100, false)]
        [InlineData(0xFF, 0xFF, double.NaN, double.NaN, false)]
        [InlineData(100, 0xFE, 50, double.NaN, true)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedPedalSmoothness(int left, int right, double expLeftPct, double expRightPct, bool expCombined)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [(byte)BicyclePower.DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, 0xFF, 0xFF, (byte)left, (byte)right, 0xFF, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expLeftPct, sensor.LeftPedalSmoothness);
            Assert.Equal(expRightPct, sensor.RightPedalSmoothness);
            Assert.Equal(expCombined, sensor.CombinedPedalSmoothness);
        }
    }
}
