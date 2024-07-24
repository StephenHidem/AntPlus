using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    [TestClass]
    public class BikeCadenceSensorTests
    {
        private MockRepository mockRepository;

        private BikeCadenceSensor _sensor;
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<BikeCadenceSensor>> mockLogger;

        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<BikeCadenceSensor>>(MockBehavior.Loose);
            _sensor = new BikeCadenceSensor(new ChannelId(0), mockAntChannel.Object, mockLogger.Object, null, 8);
            byte[] dataPage = new byte[8];
            _sensor.Parse(dataPage);
        }

        [TestMethod]
        public void Parse_Cadence_ExpectedCadence()
        {
            // Arrange
            byte[] dataPage = new byte[8] { 0, 1, 2, 3, 0x00, 0x04, 0x02, 0x00 };

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(_sensor.InstantaneousCadence == 120);
        }
    }
}
