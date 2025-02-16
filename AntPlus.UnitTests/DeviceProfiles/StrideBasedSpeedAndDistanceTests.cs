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
        private readonly MockRepository mockRepository;

        readonly ChannelId cid = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<StrideBasedSpeedAndDistance>> mockLogger;

        public StrideBasedSpeedAndDistanceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<StrideBasedSpeedAndDistance>>(MockBehavior.Loose);
        }

        private StrideBasedSpeedAndDistance CreateStrideBasedSpeedAndDistance()
        {
            return new StrideBasedSpeedAndDistance(
                cid,
                mockAntChannel.Object,
                mockLogger.Object, null);
        }

        [Fact]
        public void Parse_FirstDefaultPage_AccumulatedValueIsZero()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();
            byte[] dataPage = [1, 100, 200, 50, 0x81, 128, 254, 48];

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.Equal(0, strideBasedSpeedAndDistance.AccumulatedTime);
            Assert.Equal(0, strideBasedSpeedAndDistance.AccumulatedDistance);
            Assert.Equal(1.5, strideBasedSpeedAndDistance.InstantaneousSpeed);
            Assert.Equal(0, strideBasedSpeedAndDistance.AccumulatedStrideCount);
            Assert.Equal(1.5, strideBasedSpeedAndDistance.UpdateLatency);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_CheckRolloverDefaultPage_AccumulatedValueUpdated()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();
            byte[] dataPage = [1, 100, 200, 50, 0x81, 128, 254, 48];
            strideBasedSpeedAndDistance.Parse(dataPage); // initialize with first default page

            // Act
            dataPage = [1, 100, 100, 25, 0x81, 128, 4, 48];
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.Equal(156.5, strideBasedSpeedAndDistance.AccumulatedTime);
            Assert.Equal(231.5, strideBasedSpeedAndDistance.AccumulatedDistance);
            Assert.Equal(1.5, strideBasedSpeedAndDistance.InstantaneousSpeed);
            Assert.Equal(6, strideBasedSpeedAndDistance.AccumulatedStrideCount);
            Assert.Equal(1.5, strideBasedSpeedAndDistance.UpdateLatency);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_BaseSupplementaryPage_Matches()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            byte[] dataPage = [2, 0xFF, 0xFF, 60, 0x81, 128, 0xFF, 0x00];
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.Equal(1.5, strideBasedSpeedAndDistance.InstantaneousSpeed);
            Assert.Equal(60.5, strideBasedSpeedAndDistance.InstantaneousCadence);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_CaloriesSupplementaryPage_Matches()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            byte[] dataPage = [3, 0xFF, 0xFF, 0, 0, 0, 128, 0];
            strideBasedSpeedAndDistance.Parse(dataPage);
            dataPage[6] += 20;  // increment accumulated calories
            strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(20, strideBasedSpeedAndDistance.AccumulatedCalories);
            mockRepository.VerifyAll();
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
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(capabilities, strideBasedSpeedAndDistance.Capabilities);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_DistanceAndStridesSummary_Matches()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();
            byte[] dataPage = [0x10, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77];

            // Act
            strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.Equal(7824981.265625, strideBasedSpeedAndDistance.DistanceSummary);
            Assert.Equal((double)3351057, strideBasedSpeedAndDistance.StrideCountSummary);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, SDMLocation.Laces)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x40 }, SDMLocation.Midsole)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x80 }, SDMLocation.Other)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0xC0 }, SDMLocation.Ankle)]
        public void Parse_SupplementaryPages_SDMLocationMatches(byte[] dataPage, SDMLocation location)
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.Equal(location, strideBasedSpeedAndDistance.Status.Location);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, BatteryStatus.New)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x10 }, BatteryStatus.Good)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x20 }, BatteryStatus.Ok)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x30 }, BatteryStatus.Low)]
        public void Parse_SupplementaryPages_BatteryStatusMatches(byte[] dataPage, BatteryStatus batteryStatus)
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.Equal(batteryStatus, strideBasedSpeedAndDistance.Status.Battery);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, HealthStatus.Ok)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x04 }, HealthStatus.Error)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x08 }, HealthStatus.Warning)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x0C }, HealthStatus.Reserved)]
        public void Parse_SupplementaryPages_HealthMatches(byte[] dataPage, HealthStatus healthStatus)
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.Equal(healthStatus, strideBasedSpeedAndDistance.Status.Health);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, UseState.Inactive)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x01 }, UseState.Active)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x02 }, UseState.Reserved)]
        [InlineData(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x03 }, UseState.Reserved)]
        public void Parse_SupplementaryPages_UseStateMatches(byte[] dataPage, UseState useState)
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.Equal(useState, strideBasedSpeedAndDistance.Status.State);
            mockRepository.VerifyAll();
        }

        //[Fact]
        //public void RequestSummaryPage_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

        //    // Act
        //    strideBasedSpeedAndDistance.RequestSummaryPage();

        //    // Assert
        //    Assert.Fail();
        //    mockRepository.VerifyAll();
        //}

        //[Fact]
        //public void RequestBroadcastCapabilities_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

        //    // Act
        //    strideBasedSpeedAndDistance.RequestBroadcastCapabilities();

        //    // Assert
        //    Assert.Fail();
        //    mockRepository.VerifyAll();
        //}
    }
}
