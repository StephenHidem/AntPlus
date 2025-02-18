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
        private readonly MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<Rower>> mockLogger;


        public RowerTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Rower>>(MockBehavior.Loose);
        }

        private Rower CreateRower()
        {
            return new Rower(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null);
        }

        [Fact]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var rower = CreateRower();
            byte[] dataPage = [22, 0xFF, 0xFF, 0, 128, 0, 0x80, 0];

            // Act
            rower.Parse(
                dataPage);

            // Assert
            Assert.Equal(128, rower.Cadence);
            Assert.Equal(32768, rower.InstantaneousPower);
        }

        [Fact]
        public void Parse_StrokeCount_Matches()
        {
            // Arrange
            var rower = CreateRower();
            byte[] dataPage = [22, 0xFF, 0xFF, 255, 0, 0, 0, 0];
            rower.Parse(
                dataPage);
            dataPage[3] = 19;

            // Act
            rower.Parse(
                dataPage);

            // Assert
            Assert.Equal(20, rower.StrokeCount);
        }

        [Theory]
        [InlineData(new byte[] { 22, 0xFF, 0xFF, 0, 0, 0, 0, 0x00 }, CapabilityFlags.None)]
        [InlineData(new byte[] { 22, 0xFF, 0xFF, 0, 0, 0, 0, 0x01 }, CapabilityFlags.TxStrokeCount)]
        public void Parse_Capabilities_MatchesExpectedValue(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var rower = CreateRower();

            // Act
            rower.Parse(
                dataPage);

            // Assert
            Assert.Equal(capabilities, rower.Capabilities);
        }
    }
}
