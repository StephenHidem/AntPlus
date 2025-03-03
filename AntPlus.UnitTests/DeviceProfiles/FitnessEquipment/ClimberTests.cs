using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Climber;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    public class ClimberTests
    {
        private readonly Climber _climber;

        public ClimberTests()
        {
            _climber = new Climber(new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<ILogger<Climber>>(), It.IsAny<int>());
        }

        [Fact]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            byte[] dataPage = [23, 0, 0, 0, 128, 0, 0x80, 0];

            // Act
            _climber.Parse(dataPage);

            // Assert
            Assert.Equal(128, _climber.Cadence);
            Assert.Equal(32768, _climber.InstantaneousPower);
        }

        [Theory]
        [InlineData(new byte[] { 23, 0, 0, 0, 0, 0, 0, 0xFF }, CapabilityFlags.TxStrides)]
        [InlineData(new byte[] { 23, 0, 0, 0, 0, 0, 0, 0xFE }, CapabilityFlags.None)]
        public void Parse_Capabilities_Matches(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange

            // Act
            _climber.Parse(dataPage);

            // Assert
            Assert.Equal(capabilities, _climber.Capabilities);
        }

        [Fact]
        public void Parse_StrideCyclesRollover_MatchesExpectedValue()
        {
            // Arrange
            byte[] dataPage = [23, 0xFF, 0xFF, 255, 0, 0, 0, 0];
            _climber.Parse(dataPage);
            dataPage[3] = 19;

            // Act
            _climber.Parse(dataPage);

            // Assert
            Assert.Equal(20, _climber.StrideCycles);
        }
    }
}
