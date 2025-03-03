using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Rower;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    public class RowerTests
    {
        private readonly Rower _rower;

        public RowerTests()
        {
            _rower = new(new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<ILogger<Rower>>(), It.IsAny<int>());
        }

        [Fact]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            byte[] dataPage = [22, 0xFF, 0xFF, 0, 128, 0, 0x80, 0];

            // Act
            _rower.Parse(dataPage);

            // Assert
            Assert.Equal(128, _rower.Cadence);
            Assert.Equal(32768, _rower.InstantaneousPower);
        }

        [Fact]
        public void Parse_StrokeCount_Matches()
        {
            // Arrange
            byte[] dataPage = [22, 0xFF, 0xFF, 255, 0, 0, 0, 0];
            _rower.Parse(dataPage);
            dataPage[3] = 19;

            // Act
            _rower.Parse(dataPage);

            // Assert
            Assert.Equal(20, _rower.StrokeCount);
        }

        [Theory]
        [InlineData(new byte[] { 22, 0xFF, 0xFF, 0, 0, 0, 0, 0x00 }, CapabilityFlags.None)]
        [InlineData(new byte[] { 22, 0xFF, 0xFF, 0, 0, 0, 0, 0x01 }, CapabilityFlags.TxStrokeCount)]
        public void Parse_Capabilities_MatchesExpectedValue(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange

            // Act
            _rower.Parse(dataPage);

            // Assert
            Assert.Equal(capabilities, _rower.Capabilities);
        }
    }
}
