using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    public class CombinedSpeedAndCadenceSensorTests
    {
        private readonly MockRepository mockRepository;

        private readonly CombinedSpeedAndCadenceSensor _sensor;
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<CombinedSpeedAndCadenceSensor>> mockLogger;

        public CombinedSpeedAndCadenceSensorTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<CombinedSpeedAndCadenceSensor>>(MockBehavior.Loose);
            _sensor = new CombinedSpeedAndCadenceSensor(new ChannelId(0), mockAntChannel.Object, mockLogger.Object, null);
            byte[] dataPage = new byte[8];
            _sensor.Parse(dataPage);
        }

        [Fact]
        public void Parse_SpeedCadenceDistance_ExpectedSpeedCadenceDistance()
        {
            // Arrange
            byte[] dataPage = [0x00, 0x04, 0x02, 0x00, 0x00, 0x04, 0x05, 0x00];

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(120, _sensor.InstantaneousCadence);
            Assert.Equal(11, _sensor.InstantaneousSpeed);
            Assert.Equal(11, _sensor.AccumulatedDistance);
        }
    }
}
