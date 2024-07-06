using Microsoft.Extensions.Logging;
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
        private Mock<ILogger<BicyclePower>> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<ILogger<BicyclePower>>(MockBehavior.Loose);
        }

        private BicyclePower CreateBicyclePower(DataPage dataPage = DataPage.PowerOnly)
        {
            byte[] page = new byte[8] { (byte)dataPage, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object);
        }

        [TestMethod]
        [DataRow(DataPage.PowerOnly)]
        [DataRow(DataPage.WheelTorque)]
        [DataRow(DataPage.CrankTorque)]
        [DataRow(DataPage.CrankTorqueFrequency)]
        public void Parse_CreateSensor_ExpectedSensor(DataPage page)
        {
            // Arrange
            var bicyclePower = CreateBicyclePower(page);
            //byte[] dataPage = new byte[8];
            //dataPage[0] = (byte)page;

            // Act
            //bicyclePower.Parse(
            //    dataPage);

            // Assert
            switch (page)
            {
                case DataPage.PowerOnly:
                    Assert.IsInstanceOfType<StandardPowerSensor>(bicyclePower);
                    break;
                case DataPage.WheelTorque:
                    Assert.IsInstanceOfType<StandardPowerSensor>(bicyclePower);
                    break;
                case DataPage.CrankTorque:
                    Assert.IsInstanceOfType<StandardPowerSensor>(bicyclePower);
                    break;
                case DataPage.CrankTorqueFrequency:
                    Assert.IsInstanceOfType<CrankTorqueFrequencySensor>(bicyclePower);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }
    }
}
