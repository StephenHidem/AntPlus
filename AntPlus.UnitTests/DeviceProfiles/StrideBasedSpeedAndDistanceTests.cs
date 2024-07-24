using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using static SmallEarthTech.AntPlus.DeviceProfiles.StrideBasedSpeedAndDistance;

namespace AntPlus.UnitTests.DeviceProfiles
{
    [TestClass]
    public class StrideBasedSpeedAndDistanceTests
    {
        private MockRepository mockRepository;

        readonly ChannelId cid = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<StrideBasedSpeedAndDistance>> mockLogger;

        [TestInitialize]
        public void TestInitialize()
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
                mockLogger.Object, null, 8);
        }

        [TestMethod]
        public void Parse_FirstDefaultPage_AccumulatedValueIsZero()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();
            byte[] dataPage = new byte[] { 1, 100, 200, 50, 0x81, 128, 254, 48 };

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(strideBasedSpeedAndDistance.AccumulatedTime == 0);
            Assert.IsTrue(strideBasedSpeedAndDistance.AccumulatedDistance == 0);
            Assert.IsTrue(strideBasedSpeedAndDistance.InstantaneousSpeed == 1.5);
            Assert.IsTrue(strideBasedSpeedAndDistance.AccumulatedStrideCount == 0);
            Assert.IsTrue(strideBasedSpeedAndDistance.UpdateLatency == 1.5);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Parse_CheckRolloverDefaultPage_AccumulatedValueUpdated()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();
            byte[] dataPage = new byte[] { 1, 100, 200, 50, 0x81, 128, 254, 48 };
            strideBasedSpeedAndDistance.Parse(dataPage); // initialize with first default page

            // Act
            dataPage = new byte[] { 1, 100, 100, 25, 0x81, 128, 4, 48 };
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(strideBasedSpeedAndDistance.AccumulatedTime == 156.5);
            Assert.IsTrue(strideBasedSpeedAndDistance.AccumulatedDistance == 231.5);
            Assert.IsTrue(strideBasedSpeedAndDistance.InstantaneousSpeed == 1.5);
            Assert.IsTrue(strideBasedSpeedAndDistance.AccumulatedStrideCount == 6);
            Assert.IsTrue(strideBasedSpeedAndDistance.UpdateLatency == 1.5);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Parse_BaseSupplementaryPage_Matches()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            byte[] dataPage = new byte[] { 2, 0xFF, 0xFF, 60, 0x81, 128, 0xFF, 0x00 };
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(strideBasedSpeedAndDistance.InstantaneousSpeed == 1.5);
            Assert.IsTrue(strideBasedSpeedAndDistance.InstantaneousCadence == 60.5);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Parse_CaloriesSupplementaryPage_Matches()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            byte[] dataPage = new byte[] { 3, 0xFF, 0xFF, 0, 0, 0, 128, 0 };
            strideBasedSpeedAndDistance.Parse(dataPage);
            dataPage[6] += 20;  // increment accumulated calories
            strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.IsTrue(strideBasedSpeedAndDistance.AccumulatedCalories == 20);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(new byte[] { 22, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.Unknown)]
        [DataRow(new byte[] { 22, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.TimeValid)]
        [DataRow(new byte[] { 22, 0x02, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.DistanceValid)]
        [DataRow(new byte[] { 22, 0x04, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.SpeedValid)]
        [DataRow(new byte[] { 22, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.LatencyValid)]
        [DataRow(new byte[] { 22, 0x10, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.CadenceValid)]
        [DataRow(new byte[] { 22, 0x20, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, CapabilitiesFlags.CaloriesValid)]
        public void Parse_Capabilities_Matches(byte[] dataPage, CapabilitiesFlags capabilities)
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.AreEqual(capabilities, strideBasedSpeedAndDistance.Capabilities);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Parse_DistanceAndStridesSummary_Matches()
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();
            byte[] dataPage = new byte[] { 0x10, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 };

            // Act
            strideBasedSpeedAndDistance.Parse(dataPage);

            // Assert
            Assert.IsTrue(strideBasedSpeedAndDistance.DistanceSummary == 7824981.265625);
            Assert.IsTrue(strideBasedSpeedAndDistance.StrideCountSummary == 3351057);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, SDMLocation.Laces)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x40 }, SDMLocation.Midsole)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x80 }, SDMLocation.Other)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0xC0 }, SDMLocation.Ankle)]
        public void Parse_SupplementaryPages_SDMLocationMatches(byte[] dataPage, SDMLocation location)
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(location, strideBasedSpeedAndDistance.Status.Location);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, BatteryStatus.New)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x10 }, BatteryStatus.Good)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x20 }, BatteryStatus.Ok)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x30 }, BatteryStatus.Low)]
        public void Parse_SupplementaryPages_BatteryStatusMatches(byte[] dataPage, BatteryStatus batteryStatus)
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(batteryStatus, strideBasedSpeedAndDistance.Status.Battery);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, HealthStatus.Ok)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x04 }, HealthStatus.Error)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x08 }, HealthStatus.Warning)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x0C }, HealthStatus.Reserved)]
        public void Parse_SupplementaryPages_HealthMatches(byte[] dataPage, HealthStatus healthStatus)
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(healthStatus, strideBasedSpeedAndDistance.Status.Health);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x00 }, UseState.Inactive)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x01 }, UseState.Active)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x02 }, UseState.Reserved)]
        [DataRow(new byte[] { 2, 0, 0, 0, 0, 0, 0, 0x03 }, UseState.Reserved)]
        public void Parse_SupplementaryPages_UseStateMatches(byte[] dataPage, UseState useState)
        {
            // Arrange
            var strideBasedSpeedAndDistance = CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(useState, strideBasedSpeedAndDistance.Status.State);
            mockRepository.VerifyAll();
        }

        //[TestMethod]
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

        //[TestMethod]
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
