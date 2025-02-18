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
        private readonly MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<Climber>> mockLogger;


        public ClimberTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Climber>>(MockBehavior.Loose);
        }

        private Climber CreateClimber()
        {
            return new Climber(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null);
        }

        [Fact]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var climber = CreateClimber();
            byte[] dataPage = [23, 0, 0, 0, 128, 0, 0x80, 0];

            // Act
            climber.Parse(dataPage);

            // Assert
            Assert.Equal(128, climber.Cadence);
            Assert.Equal(32768, climber.InstantaneousPower);
        }

        [Theory]
        [InlineData(new byte[] { 23, 0, 0, 0, 0, 0, 0, 0xFF }, CapabilityFlags.TxStrides)]
        [InlineData(new byte[] { 23, 0, 0, 0, 0, 0, 0, 0xFE }, CapabilityFlags.None)]
        public void Parse_Capabilities_Matches(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var climber = CreateClimber();

            // Act
            climber.Parse(
                dataPage);

            // Assert
            Assert.Equal(capabilities, climber.Capabilities);
        }

        [Fact]
        public void Parse_StrideCyclesRollover_MatchesExpectedValue()
        {
            // Arrange
            var climber = CreateClimber();
            byte[] dataPage = [23, 0xFF, 0xFF, 255, 0, 0, 0, 0];
            climber.Parse(
                dataPage);
            dataPage[3] = 19;

            // Act
            climber.Parse(
                dataPage);

            // Assert
            Assert.Equal(20, climber.StrideCycles);
        }
    }
}
