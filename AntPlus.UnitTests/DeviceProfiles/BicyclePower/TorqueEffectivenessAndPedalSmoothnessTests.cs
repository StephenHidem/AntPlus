using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class TorqueEffectivenessAndPedalSmoothnessTests
    {
        private readonly StandardPowerSensor _standardPowerSensor;

        public TorqueEffectivenessAndPedalSmoothnessTests()
        {
            byte[] page = [(byte)BicyclePower.DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0];
            _standardPowerSensor = BicyclePower.GetBicyclePowerSensor(page, new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<NullLoggerFactory>(), It.IsAny<int>()) as StandardPowerSensor;
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(200, 100)]
        [InlineData(0xFF, double.NaN)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedTorqueEffectiveness(int value, double expPct)
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, (byte)value, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expPct, _standardPowerSensor.LeftTorqueEffectiveness);
            Assert.Equal(expPct, _standardPowerSensor.RightTorqueEffectiveness);
        }

        [Theory]
        [InlineData(0, 0, 0, 0, false)]
        [InlineData(200, 200, 100, 100, false)]
        [InlineData(0xFF, 0xFF, double.NaN, double.NaN, false)]
        [InlineData(100, 0xFE, 50, double.NaN, true)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedPedalSmoothness(int left, int right, double expLeftPct, double expRightPct, bool expCombined)
        {
            // Arrange
            byte[] dataPage = [(byte)BicyclePower.DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, 0xFF, 0xFF, (byte)left, (byte)right, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expLeftPct, _standardPowerSensor.LeftPedalSmoothness);
            Assert.Equal(expRightPct, _standardPowerSensor.RightPedalSmoothness);
            Assert.Equal(expCombined, _standardPowerSensor.CombinedPedalSmoothness);
        }
    }
}
