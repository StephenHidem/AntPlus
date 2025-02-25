using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    public class BikeSpeedSensorTests
    {
        private readonly BikeSpeedSensor _sensor;

        public BikeSpeedSensorTests()
        {
            _sensor = new BikeSpeedSensor(new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<ILogger<BikeSpeedSensor>>(), It.IsAny<int>());
            byte[] dataPage = new byte[8];
            _sensor.Parse(dataPage);
        }

        [Fact]
        public void Parse_SpeedAndDistance_ExpectedSpeedAndDistance()
        {
            // Arrange
            byte[] dataPage = [0, 1, 2, 3, 0x00, 0x04, 0x05, 0x00];

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(11, _sensor.InstantaneousSpeed);
            Assert.Equal(11, _sensor.AccumulatedDistance);
        }
    }
}
