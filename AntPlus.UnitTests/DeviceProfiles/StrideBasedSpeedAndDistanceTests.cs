using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.StrideBasedSpeedAndDistance;

namespace AntPlus.UnitTests.DeviceProfiles
{
    public class StrideBasedSpeedAndDistanceTests
    {
        private readonly StrideBasedSpeedAndDistance _strideBasedSpeedAndDistance;

        public StrideBasedSpeedAndDistanceTests()
        {
            _strideBasedSpeedAndDistance = new(new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<ILogger<StrideBasedSpeedAndDistance>>(), It.IsAny<int>());
        }

        [Fact]
        public void Parse_FirstDefaultPage_AccumulatedValueIsZero()
        {
            // Arrange
            byte[] dataPage = [1, 100, 200, 50, 0x81, 128, 254, 48];

            // Act
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(0, _strideBasedSpeedAndDistance.AccumulatedTime);
            Assert.Equal(0, _strideBasedSpeedAndDistance.AccumulatedDistance);
            Assert.Equal(1.5, _strideBasedSpeedAndDistance.InstantaneousSpeed);
            Assert.Equal(0, _strideBasedSpeedAndDistance.AccumulatedStrideCount);
            Assert.Equal(1.5, _strideBasedSpeedAndDistance.UpdateLatency);
        }

        [Fact]
        public void Parse_CheckRolloverDefaultPage_AccumulatedValueUpdated()
        {
            // Arrange
            byte[] dataPage = [1, 100, 200, 50, 0x81, 128, 254, 48];
            _strideBasedSpeedAndDistance.Parse(dataPage); // initialize with first default page

            // Act
            dataPage = [1, 100, 100, 25, 0x81, 128, 4, 48];
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(156.5, _strideBasedSpeedAndDistance.AccumulatedTime);
            Assert.Equal(231.5, _strideBasedSpeedAndDistance.AccumulatedDistance);
            Assert.Equal(1.5, _strideBasedSpeedAndDistance.InstantaneousSpeed);
            Assert.Equal(6, _strideBasedSpeedAndDistance.AccumulatedStrideCount);
            Assert.Equal(1.5, _strideBasedSpeedAndDistance.UpdateLatency);
        }

        [Fact]
        public void Parse_BaseSupplementaryPage_Matches()
        {
            // Arrange

            // Act
            byte[] dataPage = [2, 0xFF, 0xFF, 60, 0x81, 128, 0xFF, 0x00];
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(1.5, _strideBasedSpeedAndDistance.InstantaneousSpeed);
            Assert.Equal(60.5, _strideBasedSpeedAndDistance.InstantaneousCadence);
        }

        [Fact]
        public void Parse_CaloriesSupplementaryPage_Matches()
        {
            // Arrange

            // Act
            byte[] dataPage = [3, 0xFF, 0xFF, 0, 0, 0, 128, 0];
            _strideBasedSpeedAndDistance.Parse(dataPage);
            dataPage[6] += 20;  // increment accumulated calories
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(20, _strideBasedSpeedAndDistance.AccumulatedCalories);
        }

        [Theory]
        [InlineData(new byte[] { 22, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.Unknown)]
        [InlineData(new byte[] { 22, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.TimeValid)]
        [InlineData(new byte[] { 22, 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.DistanceValid)]
        [InlineData(new byte[] { 22, 0x04, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.SpeedValid)]
        [InlineData(new byte[] { 22, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.LatencyValid)]
        [InlineData(new byte[] { 22, 0x10, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.CadenceValid)]
        [InlineData(new byte[] { 22, 0x20, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.CaloriesValid)]
        public void Parse_Capabilities_Matches(byte[] dataPage, CapabilitiesFlags capabilities)
        {
            // Arrange

            // Act
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(capabilities, _strideBasedSpeedAndDistance.Capabilities);
        }

        [Fact]
        public void Parse_DistanceAndStridesSummary_Matches()
        {
            // Arrange
            byte[] dataPage = [0x10, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77];

            // Act
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(7824981.265625, _strideBasedSpeedAndDistance.DistanceSummary);
            Assert.Equal((double)3351057, _strideBasedSpeedAndDistance.StrideCountSummary);
        }

        [Theory]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, SDMLocation.Laces)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x40 }, SDMLocation.Midsole)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x80 }, SDMLocation.Other)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0xC0 }, SDMLocation.Ankle)]
        public void Parse_SupplementaryPages_SDMLocationMatches(byte[] dataPage, SDMLocation location)
        {
            // Arrange

            // Act
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(location, _strideBasedSpeedAndDistance.Status.Location);
        }

        [Theory]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, BatteryStatus.New)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x10 }, BatteryStatus.Good)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x20 }, BatteryStatus.Ok)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x30 }, BatteryStatus.Low)]
        public void Parse_SupplementaryPages_BatteryStatusMatches(byte[] dataPage, BatteryStatus batteryStatus)
        {
            // Arrange

            // Act
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(batteryStatus, _strideBasedSpeedAndDistance.Status.Battery);
        }

        [Theory]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, HealthStatus.Ok)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x04 }, HealthStatus.Error)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x08 }, HealthStatus.Warning)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x0C }, HealthStatus.Reserved)]
        public void Parse_SupplementaryPages_HealthMatches(byte[] dataPage, HealthStatus healthStatus)
        {
            // Arrange

            // Act
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(healthStatus, _strideBasedSpeedAndDistance.Status.Health);
        }

        [Theory]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, UseState.Inactive)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x01 }, UseState.Active)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x02 }, UseState.Reserved)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x03 }, UseState.Reserved)]
        public void Parse_SupplementaryPages_UseStateMatches(byte[] dataPage, UseState useState)
        {
            // Arrange

            // Act
            _strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(useState, _strideBasedSpeedAndDistance.Status.State);
        }
    }
}
