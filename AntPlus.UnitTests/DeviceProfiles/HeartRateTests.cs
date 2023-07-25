using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using static SmallEarthTech.AntPlus.DeviceProfiles.HeartRate;

namespace AntPlus.UnitTests.DeviceProfiles
{
    [TestClass]
    public class HeartRateTests
    {
        private MockRepository? mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel>? mockAntChannel;
        private Mock<ILogger<HeartRate>>? mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<HeartRate>>(MockBehavior.Loose);
        }

        private HeartRate CreateHeartRate()
        {
            return new HeartRate(
                mockChannelId,
                mockAntChannel?.Object,
                mockLogger?.Object);
        }

        [TestMethod]
        public void Parse_FirstDataPage_CommonHRDataInitialized()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            byte hr = 70;
            byte[] dataPage = new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, hr };

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(hr, heartRate.HeartRateData.ComputedHeartRate);
            Assert.AreEqual(0, heartRate.HeartRateData.RRInterval);
            Assert.AreEqual(0, heartRate.HeartRateData.AccumulatedHeartBeatEventTime);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void Parse_CommonHRData_ExpectedRRandBeatEventTime()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0, 0, 0, 0, 0xAA, 0x59, 0x34, 70 };

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(1000, heartRate.HeartRateData.RRInterval);
            Assert.AreEqual(1000, heartRate.HeartRateData.AccumulatedHeartBeatEventTime);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void Parse_CumulativeOperatingTime_ExpectedTime()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0x81, 1, 2, 3, 0xAA, 0x59, 0x34, 70 };
            TimeSpan expTime = TimeSpan.FromSeconds(0x030201 * 2);

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expTime, heartRate.CumulativeOperatingTime);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void Parse_ManufacturerInfo_ExpectedManufacturerInfo()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0x82, 1, 2, 3, 0xAA, 0x59, 0x34, 70 };
            byte manId = 1;
            uint serialNum = 0x03020000;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(manId, heartRate.ManufacturerInfo.ManufacturingIdLsb);
            Assert.AreEqual(serialNum, heartRate.ManufacturerInfo.SerialNumber);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void Parse_ProductInfo_ExpectedProductInfo()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0x83, 1, 2, 3, 0xAA, 0x59, 0x34, 70 };
            byte hwVer = 1;
            byte swVer = 2;
            byte model = 3;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(hwVer, heartRate.ProductInfo.HardwareVersion);
            Assert.AreEqual(swVer, heartRate.ProductInfo.SoftwareVersion);
            Assert.AreEqual(model, heartRate.ProductInfo.ModelNumber);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void Parse_PreviousHeartBeat_ExpectedRRInterval()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0x84, 1, 0xFF, 0xFF, 0xFF, 0x03, 0x34, 70 };
            byte manSpecific = 1;
            int rrInterval = 1000;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(manSpecific, heartRate.PreviousHeartBeat.ManufacturerSpecific);
            Assert.AreEqual(rrInterval, heartRate.PreviousHeartBeat.RRInterval);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void Parse_SwimInterval_ExpectedSummary()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0x85, 70, 120, 100, 0xAA, 0x59, 0x34, 70 };
            byte intAvgHr = 70;
            byte intMaxHr = 120;
            byte sessAvgHr = 100;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(intAvgHr, heartRate.SwimInterval.IntervalAverageHeartRate);
            Assert.AreEqual(intMaxHr, heartRate.SwimInterval.IntervalMaximumHeartRate);
            Assert.AreEqual(sessAvgHr, heartRate.SwimInterval.SessionAverageHeartRate);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        [DataRow(0, HeartRate.Features.Generic)]
        [DataRow(1, HeartRate.Features.Running)]
        [DataRow(2, HeartRate.Features.Cycling)]
        [DataRow(4, HeartRate.Features.Swimming)]
        [DataRow(8, HeartRate.Features.GymMode)]
        [DataRow(0x40, HeartRate.Features.ManufacturerFeature1)]
        [DataRow(0x80, HeartRate.Features.ManufacturerFeature2)]
        public void Parse_Capabilities_ExpectedFeature(int feature, HeartRate.Features expFeature)
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0x86, 0xFF, (byte)feature, (byte)feature, 0xAA, 0x59, 0x34, 70 };

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expFeature, heartRate.Capabilities.Supported);
            Assert.AreEqual(expFeature, heartRate.Capabilities.Enabled);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void Parse_BatteryStatus_ExpectedPctAndVoltage()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0x87, 70, 0x66, 0x03, 0xAA, 0x59, 0x34, 70 };
            byte expPct = 70;
            double expVoltage = 3.4;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPct, heartRate.BatteryStatus.BatteryLevel);
            Assert.AreEqual(expVoltage, heartRate.BatteryStatus.BatteryVoltage, 1 / 256.0);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        [DataRow(0x00, BatteryStatus.Unknown)]
        [DataRow(0x10, BatteryStatus.New)]
        [DataRow(0x20, BatteryStatus.Good)]
        [DataRow(0x30, BatteryStatus.Ok)]
        [DataRow(0x40, BatteryStatus.Low)]
        [DataRow(0x50, BatteryStatus.Critical)]
        [DataRow(0x60, BatteryStatus.Reserved)]
        [DataRow(0x70, BatteryStatus.Invalid)]
        public void Parse_BatteryStatus_ExpectedStatus(int val, BatteryStatus expStatus)
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0x87, 70, 120, (byte)val, 0xAA, 0x59, 0x34, 70 };

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expStatus, heartRate.BatteryStatus.BatteryStatus);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        [DataRow(0xFC, HeartbeatEventType.MeasuredTimestamp)]
        [DataRow(0xFD, HeartbeatEventType.ComputedTimestamp)]
        public void Parse_DeviceInfo_ExpectedEventType(int eventType, HeartbeatEventType expEventType)
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse(new byte[8] { 0, 0, 0, 0, 0xAA, 0x55, 0x33, 70 });
            byte[] dataPage = new byte[8] { 0x89, (byte)eventType, 0xFF, 0xFF, 0xAA, 0x59, 0x34, 70 };

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expEventType, heartRate.EventType);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void SetSportMode_SportMode_ExpectedSportModeRequest()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            SportMode sportMode = default;
            SubSportMode subSportMode = default;
            byte[] msg = new byte[] { (byte)CommonDataPage.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)subSportMode, (byte)sportMode };
            mockAntChannel?.Setup(ac =>
            ac.SendExtAcknowledgedData(mockChannelId, msg, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            heartRate.SetSportMode(
                sportMode,
                subSportMode);

            // Assert
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void SetHRFeature_Feature_ExpectedFeatureRequest()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            bool applyGymMode = false;
            bool gymMode = false;
            byte[] msg = new byte[] { (byte)DataPage.HRFeature, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x00 };
            mockAntChannel?.Setup(ac =>
            ac.SendExtAcknowledgedData(mockChannelId, msg, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            heartRate.SetHRFeature(
                applyGymMode,
                gymMode);

            // Assert
            mockRepository?.VerifyAll();
        }
    }
}
