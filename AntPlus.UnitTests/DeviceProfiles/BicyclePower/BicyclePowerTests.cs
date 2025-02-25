using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class BicyclePowerTests
    {
        private readonly Mock<IAntChannel> _mockAntChannel;
        private readonly Mock<NullLoggerFactory> _mockLogger;

        public BicyclePowerTests()
        {
            _mockAntChannel = new Mock<IAntChannel> { CallBase = true };
            _mockLogger = new Mock<NullLoggerFactory> { CallBase = true };
        }

        private BicyclePower CreateBicyclePower(BicyclePower.DataPage dataPageIdentifier)
        {
            byte[] page = [(byte)dataPageIdentifier, 0, 0, 0, 0, 0, 0, 0];
            return BicyclePower.GetBicyclePowerSensor(page, new ChannelId(0), _mockAntChannel.Object, _mockLogger.Object, It.IsAny<int>());
        }

        [Theory]
        [InlineData(BicyclePower.DataPage.PowerOnly)]
        [InlineData(BicyclePower.DataPage.WheelTorque)]
        [InlineData(BicyclePower.DataPage.CrankTorque)]
        [InlineData(BicyclePower.DataPage.CrankTorqueFrequency)]
        public void Parse_CreateSensor_ExpectedSensor(BicyclePower.DataPage page)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower(page);

            // Assert
            switch (page)
            {
                case BicyclePower.DataPage.PowerOnly:
                    Assert.IsType<StandardPowerSensor>(bicyclePower);
                    break;
                case BicyclePower.DataPage.WheelTorque:
                    Assert.IsType<StandardPowerSensor>(bicyclePower);
                    break;
                case BicyclePower.DataPage.CrankTorque:
                    Assert.IsType<StandardPowerSensor>(bicyclePower);
                    break;
                case BicyclePower.DataPage.CrankTorqueFrequency:
                    Assert.IsType<CrankTorqueFrequencySensor>(bicyclePower);
                    break;
                default:
                    Assert.Fail("Unexpected data page type.");
                    break;
            }
        }
    }
}
