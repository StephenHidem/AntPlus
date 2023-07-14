using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    [TestClass]
    public class CombinedSpeedAndCadenceSensorTests
    {
        private MockRepository? mockRepository;

        private CombinedSpeedAndCadenceSensor? _sensor;
        private Mock<IAntChannel>? mockAntChannel;
        private Mock<ILogger<CombinedSpeedAndCadenceSensor>>? mockLogger;

        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<CombinedSpeedAndCadenceSensor>>(MockBehavior.Loose);
            _sensor = new CombinedSpeedAndCadenceSensor(new ChannelId(0), mockAntChannel.Object, mockLogger.Object);
            byte[] dataPage = new byte[8];
            _sensor.Parse(dataPage);
        }

        [TestMethod]
        public void Parse_SpeedCadenceDistance_ExpectedSpeedCadenceDistance()
        {
            // Arrange
            byte[] dataPage = new byte[8] { 0x00, 0x04, 0x02, 0x00, 0x00, 0x04, 0x05, 0x00 };

            // Act
            _sensor?.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(_sensor?.InstantaneousCadence == 120);
            Assert.IsTrue(_sensor?.InstantaneousSpeed == 11);
            Assert.IsTrue(_sensor?.AccumulatedDistance == 11);
        }
    }
}
