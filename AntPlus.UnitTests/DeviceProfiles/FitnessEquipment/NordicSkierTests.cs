using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.NordicSkier;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    public class NordicSkierTests
    {
        private readonly NordicSkier _skier;

        public NordicSkierTests()
        {
            _skier = new(new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<ILogger<NordicSkier>>(), It.IsAny<int>());
        }

        [Fact]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            byte[] dataPage = [24, 0xFF, 0xFF, 0, 128, 0, 0x80, 0];

            // Act
            _skier.Parse(dataPage);

            // Assert
            Assert.Equal(128, _skier.Cadence);
            Assert.Equal(32768, _skier.InstantaneousPower);
        }

        [Fact]
        public void Parse_StrideCount_Matches()
        {
            // Arrange
            byte[] dataPage = [24, 0xFF, 0xFF, 255, 0, 0, 0, 0];
            _skier.Parse(dataPage);
            dataPage[3] = 19;

            // Act
            _skier.Parse(dataPage);

            // Assert
            Assert.Equal(20, _skier.StrideCount);
        }

        [Theory]
        [InlineData(new byte[] { 24, 0xFF, 0xFF, 0, 0, 0, 0, 0x00 }, CapabilityFlags.None)]
        [InlineData(new byte[] { 24, 0xFF, 0xFF, 0, 0, 0, 0, 0x01 }, CapabilityFlags.TxStrideCount)]
        public void Parse_Capabilities_MatchesExpectedValue(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange

            // Act
            _skier.Parse(dataPage);

            // Assert
            Assert.Equal(capabilities, _skier.Capabilities);
        }
    }
}
