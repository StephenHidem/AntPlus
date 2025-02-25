using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    public class BikeCadenceSensorTests
    {
        private readonly BikeCadenceSensor _sensor;

        public BikeCadenceSensorTests()
        {
            _sensor = new BikeCadenceSensor(new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<ILogger<BikeCadenceSensor>>(), It.IsAny<int>());
            byte[] dataPage = new byte[8];
            _sensor.Parse(dataPage);
        }

        [Fact]
        public void Parse_Cadence_ExpectedCadence()
        {
            // Arrange
            byte[] dataPage = [0, 1, 2, 3, 0x00, 0x04, 0x02, 0x00];

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(120, _sensor.InstantaneousCadence);
        }
    }
}
