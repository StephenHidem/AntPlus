using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    public class StationaryBikeTests
    {
        private readonly StationaryBike _stationaryBike;

        public StationaryBikeTests()
        {
            _stationaryBike = new(new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<ILogger<StationaryBike>>(), It.IsAny<int>());
        }

        [Fact]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            byte[] dataPage = [0x15, 0xFF, 0xFF, 0xFF, 60, 150, 0, 0];

            // Act
            _stationaryBike.Parse(dataPage);

            // Assert
            Assert.Equal(60, _stationaryBike.Cadence);
            Assert.Equal(150, _stationaryBike.InstantaneousPower);
        }

        [Fact]
        public void Parse_PowerHighValue_Correct()
        {
            // Arrange - 274W = 0x0112
            byte[] dataPage = [0x15, 0xFF, 0xFF, 0xFF, 87, 0x12, 0x01, 0];

            // Act
            _stationaryBike.Parse(dataPage);

            // Assert
            Assert.Equal(87, _stationaryBike.Cadence);
            Assert.Equal(274, _stationaryBike.InstantaneousPower);
        }

        [Fact]
        public void Parse_CommonDataPage_Handled()
        {
            // Arrange - common page 0x50 (Manufacturer Info)
            byte[] dataPage = [0x50, 0xFF, 0xFF, 16, 32, 0, 100, 0];

            // Act & Assert - should not throw
            _stationaryBike.Parse(dataPage);
        }

        [Fact]
        public void ToString_ReturnsExpected()
        {
            Assert.Equal("Stationary Bike", _stationaryBike.ToString());
        }
    }
}