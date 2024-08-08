using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    [TestClass]
    public class BicyclePowerTests
    {
        private MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<NullLoggerFactory> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<NullLoggerFactory>(MockBehavior.Loose);
        }

        private BicyclePower CreateBicyclePower(BicyclePower.DataPage dataPage)
        {
            byte[] page = new byte[8] { (byte)dataPage, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, 2000);
        }

        [TestMethod]
        [DataRow(BicyclePower.DataPage.PowerOnly)]
        [DataRow(BicyclePower.DataPage.WheelTorque)]
        [DataRow(BicyclePower.DataPage.CrankTorque)]
        [DataRow(BicyclePower.DataPage.CrankTorqueFrequency)]
        public void Parse_CreateSensor_ExpectedSensor(BicyclePower.DataPage page)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower(page);

            // Assert
            switch (page)
            {
                case BicyclePower.DataPage.PowerOnly:
                    Assert.IsInstanceOfType<StandardPowerSensor>(bicyclePower);
                    break;
                case BicyclePower.DataPage.WheelTorque:
                    Assert.IsInstanceOfType<StandardPowerSensor>(bicyclePower);
                    break;
                case BicyclePower.DataPage.CrankTorque:
                    Assert.IsInstanceOfType<StandardPowerSensor>(bicyclePower);
                    break;
                case BicyclePower.DataPage.CrankTorqueFrequency:
                    Assert.IsInstanceOfType<CrankTorqueFrequencySensor>(bicyclePower);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }
    }
}
