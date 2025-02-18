using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Treadmill;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    public class TreadmillTests
    {
        private readonly MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<Treadmill>> mockLogger;


        public TreadmillTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Treadmill>>(MockBehavior.Loose);
        }

        private Treadmill CreateTreadmill()
        {
            return new Treadmill(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null);
        }

        [Fact]
        public void Parse_InstantaneousCadence_Matches()
        {
            // Arrange
            var treadmill = CreateTreadmill();
            byte[] dataPage = [19, 0, 0, 0, 128, 0, 0, 0];

            // Act
            treadmill.Parse(dataPage);

            // Assert
            Assert.Equal(128, treadmill.Cadence);
        }

        [Fact]
        public void Parse_PositiveVerticalDistance_Updated()
        {
            // Arrange
            var treadmill = CreateTreadmill();
            byte[] dataPage = [19, 0, 0, 0, 0, 0, 255, 0];
            treadmill.Parse(dataPage);
            dataPage[6] = 19;

            // Act
            treadmill.Parse(dataPage);

            // Assert
            Assert.Equal(2.0, treadmill.PosVerticalDistance);
        }

        [Fact]
        public void Parse_NegativeVerticalDistance_Updated()
        {
            // Arrange
            var treadmill = CreateTreadmill();
            byte[] dataPage = [19, 0, 0, 0, 0, 255, 0, 0];
            treadmill.Parse(dataPage);
            dataPage[5] = 19;

            // Act
            treadmill.Parse(dataPage);

            // Assert
            Assert.True(treadmill.NegVerticalDistance == -2.0);
        }

        [Theory]
        [InlineData(new byte[] { 19, 0, 0, 0, 0, 0, 0, 0xFD }, CapabilityFlags.TxPosVertDistance)]
        [InlineData(new byte[] { 19, 0, 0, 0, 0, 0, 0, 0xFE }, CapabilityFlags.TxNegVertDistance)]
        public void Parse_Capabilities_Matches(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var treadmill = CreateTreadmill();

            // Act
            treadmill.Parse(
                dataPage);

            // Assert
            Assert.Equal(capabilities, treadmill.Capabilities);
        }
    }
}
