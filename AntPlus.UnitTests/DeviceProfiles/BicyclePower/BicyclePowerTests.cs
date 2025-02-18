using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class BicyclePowerTests
    {
        private readonly MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<NullLoggerFactory> mockLogger;

        public BicyclePowerTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<NullLoggerFactory>(MockBehavior.Loose);
        }

        private BicyclePower CreateBicyclePower(BicyclePower.DataPage dataPage)
        {
            byte[] page = [(byte)dataPage, 0, 0, 0, 0, 0, 0, 0];
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, 2000);
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
