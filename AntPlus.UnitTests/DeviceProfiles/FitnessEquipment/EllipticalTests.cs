using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Elliptical;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    public class EllipticalTests
    {
        private readonly MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<Elliptical>> mockLogger;


        public EllipticalTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Elliptical>>(MockBehavior.Loose);
        }

        private Elliptical CreateElliptical()
        {
            return new Elliptical(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null);
        }

        [Fact]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var elliptical = CreateElliptical();
            byte[] dataPage = [20, 0xFF, 0, 0, 128, 0, 0x80, 0];

            // Act
            elliptical.Parse(
                dataPage);

            // Assert
            Assert.Equal(128, elliptical.Cadence);
            Assert.Equal(32768, elliptical.InstantaneousPower);
        }

        [Fact]
        public void Parse_PositiveVerticalDistance_Matches()
        {
            // Arrange
            var elliptical = CreateElliptical();
            byte[] dataPage = [20, 0xFF, 255, 0, 0, 0, 0, 0];
            elliptical.Parse(
                dataPage);
            dataPage[2] = 19;

            // Act
            elliptical.Parse(
                dataPage);

            // Assert
            Assert.Equal(2.0, elliptical.PosVerticalDistance);
        }

        [Fact]
        public void Parse_StrideCount_Matches()
        {
            // Arrange
            var elliptical = CreateElliptical();
            byte[] dataPage = [20, 0xFF, 0, 255, 0, 0, 0, 0];
            elliptical.Parse(
                dataPage);
            dataPage[3] = 19;

            // Act
            elliptical.Parse(
                dataPage);

            // Assert
            Assert.Equal(20, elliptical.StrideCount);
        }

        [Theory]
        [InlineData(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x00 }, CapabilityFlags.None)]
        [InlineData(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x01 }, CapabilityFlags.TxStrideCount)]
        [InlineData(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x02 }, CapabilityFlags.TxPosVertDistance)]
        [InlineData(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x03 }, CapabilityFlags.TxPosVertDistance | CapabilityFlags.TxStrideCount)]
        public void Parse_Capabilities_MatchesExpectedValue(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var elliptical = CreateElliptical();

            // Act
            elliptical.Parse(
                dataPage);

            // Assert
            Assert.Equal(capabilities, elliptical.Capabilities);
        }
    }
}
