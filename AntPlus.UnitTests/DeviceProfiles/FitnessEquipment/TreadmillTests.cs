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
        private readonly Treadmill _treadmill;

        public TreadmillTests()
        {
            _treadmill = new Treadmill(
                new ChannelId(0),
                Mock.Of<IAntChannel>(),
                Mock.Of<ILogger<Treadmill>>(), It.IsAny<int>());
        }

        [Fact]
        public void Parse_InstantaneousCadence_Matches()
        {
            // Arrange
            byte[] dataPage = [19, 0, 0, 0, 128, 0, 0, 0];

            // Act
            _treadmill.Parse(dataPage);

            // Assert
            Assert.Equal(128, _treadmill.Cadence);
        }

        [Fact]
        public void Parse_PositiveVerticalDistance_Updated()
        {
            // Arrange
            byte[] dataPage = [19, 0, 0, 0, 0, 0, 255, 0];
            _treadmill.Parse(dataPage);
            dataPage[6] = 19;

            // Act
            _treadmill.Parse(dataPage);

            // Assert
            Assert.Equal(2.0, _treadmill.PosVerticalDistance);
        }

        [Fact]
        public void Parse_NegativeVerticalDistance_Updated()
        {
            // Arrange
            byte[] dataPage = [19, 0, 0, 0, 0, 255, 0, 0];
            _treadmill.Parse(dataPage);
            dataPage[5] = 19;

            // Act
            _treadmill.Parse(dataPage);

            // Assert
            Assert.True(_treadmill.NegVerticalDistance == -2.0);
        }

        [Theory]
        [InlineData(new byte[] { 19, 0, 0, 0, 0, 0, 0, 0xFD }, CapabilityFlags.TxPosVertDistance)]
        [InlineData(new byte[] { 19, 0, 0, 0, 0, 0, 0, 0xFE }, CapabilityFlags.TxNegVertDistance)]
        public void Parse_Capabilities_Matches(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange

            // Act
            _treadmill.Parse(dataPage);

            // Assert
            Assert.Equal(capabilities, _treadmill.Capabilities);
        }
    }
}
