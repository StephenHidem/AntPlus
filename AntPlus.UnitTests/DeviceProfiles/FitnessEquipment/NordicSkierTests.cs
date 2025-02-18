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
        private readonly MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<NordicSkier>> mockLogger;


        public NordicSkierTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<NordicSkier>>(MockBehavior.Loose);
        }

        private NordicSkier CreateNordicSkier()
        {
            return new NordicSkier(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null);
        }

        [Fact]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var skier = CreateNordicSkier();
            byte[] dataPage = [24, 0xFF, 0xFF, 0, 128, 0, 0x80, 0];

            // Act
            skier.Parse(
                dataPage);

            // Assert
            Assert.Equal(128, skier.Cadence);
            Assert.Equal(32768, skier.InstantaneousPower);
        }

        [Fact]
        public void Parse_StrideCount_Matches()
        {
            // Arrange
            var skier = CreateNordicSkier();
            byte[] dataPage = [24, 0xFF, 0xFF, 255, 0, 0, 0, 0];
            skier.Parse(
                dataPage);
            dataPage[3] = 19;

            // Act
            skier.Parse(
                dataPage);

            // Assert
            Assert.Equal(20, skier.StrideCount);
        }

        [Theory]
        [InlineData(new byte[] { 24, 0xFF, 0xFF, 0, 0, 0, 0, 0x00 }, CapabilityFlags.None)]
        [InlineData(new byte[] { 24, 0xFF, 0xFF, 0, 0, 0, 0, 0x01 }, CapabilityFlags.TxStrideCount)]
        public void Parse_Capabilities_MatchesExpectedValue(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var skier = CreateNordicSkier();

            // Act
            skier.Parse(
                dataPage);

            // Assert
            Assert.Equal(capabilities, skier.Capabilities);
        }
    }
}
