using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading.Tasks;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.HeartRate;

namespace AntPlus.UnitTests.DeviceProfiles
{
    public class HeartRateTests
    {
        private readonly MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<HeartRate>> mockLogger;

        public HeartRateTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<HeartRate>>(MockBehavior.Loose);
        }

        private HeartRate CreateHeartRate()
        {
            return new HeartRate(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null);
        }

        [Fact]
        public void Parse_FirstDataPage_CommonHRDataInitialized()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            byte hr = 70;
            byte[] dataPage = [0, 0, 0, 0, 0xAA, 0x55, 0x33, hr];

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(hr, heartRate.HeartRateData.ComputedHeartRate);
            Assert.Equal(0, heartRate.HeartRateData.RRInterval);
            Assert.Equal(0, heartRate.HeartRateData.AccumulatedHeartBeatEventTime);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_CommonHRData_ExpectedRRandBeatEventTime()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0, 0, 0, 0, 0xAA, 0x59, 0x34, 70];

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(1000, heartRate.HeartRateData.RRInterval);
            Assert.Equal(1000, heartRate.HeartRateData.AccumulatedHeartBeatEventTime);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_CumulativeOperatingTime_ExpectedTime()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x81, 1, 2, 3, 0xAA, 0x59, 0x34, 70];
            TimeSpan expTime = TimeSpan.FromSeconds(0x030201 * 2);

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(expTime, heartRate.CumulativeOperatingTime);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_ManufacturerInfo_ExpectedManufacturerInfo()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x82, 1, 2, 3, 0xAA, 0x59, 0x34, 70];
            byte manId = 1;
            uint serialNum = 0x03020000;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(manId, heartRate.ManufacturerInfo.ManufacturingIdLsb);
            Assert.Equal(serialNum, heartRate.ManufacturerInfo.SerialNumber);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_ProductInfo_ExpectedProductInfo()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x83, 1, 2, 3, 0xAA, 0x59, 0x34, 70];
            byte hwVer = 1;
            byte swVer = 2;
            byte model = 3;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(hwVer, heartRate.ProductInfo.HardwareVersion);
            Assert.Equal(swVer, heartRate.ProductInfo.SoftwareVersion);
            Assert.Equal(model, heartRate.ProductInfo.ModelNumber);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_PreviousHeartBeat_ExpectedRRInterval()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x84, 1, 0xFF, 0xFF, 0xFF, 0x03, 0x34, 70];
            byte manSpecific = 1;
            int rrInterval = 1000;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(manSpecific, heartRate.PreviousHeartBeat.ManufacturerSpecific);
            Assert.Equal(rrInterval, heartRate.PreviousHeartBeat.RRInterval);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_SwimInterval_ExpectedSummary()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x85, 70, 120, 100, 0xAA, 0x59, 0x34, 70];
            byte intAvgHr = 70;
            byte intMaxHr = 120;
            byte sessionAvgHr = 100;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(intAvgHr, heartRate.SwimInterval.IntervalAverageHeartRate);
            Assert.Equal(intMaxHr, heartRate.SwimInterval.IntervalMaximumHeartRate);
            Assert.Equal(sessionAvgHr, heartRate.SwimInterval.SessionAverageHeartRate);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(0, HeartRate.Features.Generic)]
        [InlineData(1, HeartRate.Features.Running)]
        [InlineData(2, HeartRate.Features.Cycling)]
        [InlineData(4, HeartRate.Features.Swimming)]
        [InlineData(8, HeartRate.Features.GymMode)]
        [InlineData(0x40, HeartRate.Features.ManufacturerFeature1)]
        [InlineData(0x80, HeartRate.Features.ManufacturerFeature2)]
        public void Parse_Capabilities_ExpectedFeature(int feature, HeartRate.Features expFeature)
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x86, 0xFF, (byte)feature, (byte)feature, 0xAA, 0x59, 0x34, 70];

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(expFeature, heartRate.Capabilities.Supported);
            Assert.Equal(expFeature, heartRate.Capabilities.Enabled);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_BatteryStatus_ExpectedPctAndVoltage()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x87, 70, 0x66, 0x03, 0xAA, 0x59, 0x34, 70];
            byte expPct = 70;
            double expVoltage = 3.4;

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(expPct, heartRate.BatteryStatus.BatteryLevel);
            Assert.Equal(expVoltage, heartRate.BatteryStatus.BatteryVoltage, 1 / 256.0);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(0x00, BatteryStatus.Unknown)]
        [InlineData(0x10, BatteryStatus.New)]
        [InlineData(0x20, BatteryStatus.Good)]
        [InlineData(0x30, BatteryStatus.Ok)]
        [InlineData(0x40, BatteryStatus.Low)]
        [InlineData(0x50, BatteryStatus.Critical)]
        [InlineData(0x60, BatteryStatus.Reserved)]
        [InlineData(0x70, BatteryStatus.Invalid)]
        public void Parse_BatteryStatus_ExpectedStatus(int val, BatteryStatus expStatus)
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x87, 70, 120, (byte)val, 0xAA, 0x59, 0x34, 70];

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(expStatus, heartRate.BatteryStatus.BatteryStatus);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(0xFC, HeartbeatEventType.MeasuredTimestamp)]
        [InlineData(0xFD, HeartbeatEventType.ComputedTimestamp)]
        public void Parse_DeviceInfo_ExpectedEventType(int eventType, HeartbeatEventType expEventType)
        {
            // Arrange
            var heartRate = CreateHeartRate();
            heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x89, (byte)eventType, 0xFF, 0xFF, 0xAA, 0x59, 0x34, 70];

            // Act
            heartRate.Parse(
                dataPage);

            // Assert
            Assert.Equal(expEventType, heartRate.EventType);
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task SetSportMode_SportMode_ExpectedSportModeRequest()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            SportMode sportMode = default;
            SubSportMode subSportMode = default;
            byte[] msg = [(byte)CommonDataPage.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)subSportMode, (byte)sportMode];
            mockAntChannel.Setup(ac =>
            ac.SendExtAcknowledgedDataAsync(mockChannelId, msg, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            var result = await heartRate.SetSportMode(
                sportMode,
                subSportMode);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task SetHRFeature_Feature_ExpectedFeatureRequest()
        {
            // Arrange
            var heartRate = CreateHeartRate();
            bool applyGymMode = false;
            bool gymMode = false;
            byte[] msg = [(byte)DataPage.HRFeature, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x00];
            mockAntChannel.Setup(ac =>
            ac.SendExtAcknowledgedDataAsync(mockChannelId, msg, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            var result = await heartRate.SetHRFeature(
                applyGymMode,
                gymMode);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }
    }
}
