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
        private readonly StandardPowerSensor _standardPowerSensor;

        public StandardPowerSensorTests()
        {
            byte[] page = [(byte)BicyclePower.DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0];
            _standardPowerSensor = BicyclePower.GetBicyclePowerSensor(page, new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<NullLoggerFactory>(), 2000) as StandardPowerSensor;
        }

        [Theory]
        [InlineData(0x32, 50, PedalDifferentiation.Unknown)]
        [InlineData(0xB2, 50, PedalDifferentiation.RightPedal)]
        [InlineData(0xFF, 0x7F, PedalDifferentiation.Unused)]
        public void Parse_PedalPower_ExpectedPedalPower(int value, int percent, PedalDifferentiation pedalDifferentiation)
        {
            // Arrange
            byte[] dataPage = [16, 1, (byte)value, 0, 0, 0, 0, 0];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(percent, _standardPowerSensor.PedalPower);
            Assert.Equal(pedalDifferentiation, _standardPowerSensor.PedalContribution);
        }

        [Fact]
        public void Parse_InstantaneousCadence_ExpectedCadence()
        {
            // Arrange
            byte[] dataPage = [16, 1, 0, 128, 0, 0, 0, 0];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(128, _standardPowerSensor.InstantaneousCadence);
        }

        [Fact]
        public void Parse_InstantaneousPower_ExpectedPower()
        {
            // Arrange
            byte[] dataPage = [16, 1, 0, 0, 0, 0, 0x11, 0x22];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(8721, _standardPowerSensor.InstantaneousPower);
        }

        [Fact]
        public void Parse_AveragePower_ExpectedAvgPower()
        {
            // Arrange
            byte[] dataPage = [16, 1, 0, 0, 0x11, 0x22, 0, 0];

            // Act
            _standardPowerSensor.Parse([16, 0, 0, 0, 0, 0, 0, 0]);
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(8721, _standardPowerSensor.AveragePower);
        }
    }
}
