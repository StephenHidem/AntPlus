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
        private readonly Elliptical _elliptical;

        public EllipticalTests()
        {
            _elliptical = new(new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<ILogger<Elliptical>>(), It.IsAny<int>());
        }

        [Fact]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            byte[] dataPage = [20, 0xFF, 0, 0, 128, 0, 0x80, 0];

            // Act
            _elliptical.Parse(dataPage);

            // Assert
            Assert.Equal(128, _elliptical.Cadence);
            Assert.Equal(32768, _elliptical.InstantaneousPower);
        }

        [Fact]
        public void Parse_PositiveVerticalDistance_Matches()
        {
            // Arrange
            byte[] dataPage = [20, 0xFF, 255, 0, 0, 0, 0, 0];
            _elliptical.Parse(dataPage);
            dataPage[2] = 19;

            // Act
            _elliptical.Parse(dataPage);

            // Assert
            Assert.Equal(2.0, _elliptical.PosVerticalDistance);
        }

        [Fact]
        public void Parse_StrideCount_Matches()
        {
            // Arrange
            byte[] dataPage = [20, 0xFF, 0, 255, 0, 0, 0, 0];
            _elliptical.Parse(dataPage);
            dataPage[3] = 19;

            // Act
            _elliptical.Parse(dataPage);

            // Assert
            Assert.Equal(20, _elliptical.StrideCount);
        }

        [Theory]
        [InlineData(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x00 }, CapabilityFlags.None)]
        [InlineData(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x01 }, CapabilityFlags.TxStrideCount)]
        [InlineData(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x02 }, CapabilityFlags.TxPosVertDistance)]
        [InlineData(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x03 }, CapabilityFlags.TxPosVertDistance | CapabilityFlags.TxStrideCount)]
        public void Parse_Capabilities_MatchesExpectedValue(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange

            // Act
            _elliptical.Parse(dataPage);

            // Assert
            Assert.Equal(capabilities, _elliptical.Capabilities);
        }
    }
}
