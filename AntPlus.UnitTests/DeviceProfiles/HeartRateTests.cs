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
        private readonly HeartRate _heartRate;

        private readonly ChannelId cid = new(0);
        private readonly Mock<IAntChannel> _mockAntChannel;

        public HeartRateTests()
        {
            _mockAntChannel = new();
            _heartRate = new(cid, _mockAntChannel.Object, Mock.Of<ILogger<HeartRate>>(), It.IsAny<int>());
        }

        [Fact]
        public void Parse_FirstDataPage_CommonHRDataInitialized()
        {
            // Arrange
            byte hr = 70;
            byte[] dataPage = [0, 0, 0, 0, 0xAA, 0x55, 0x33, hr];

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(hr, _heartRate.HeartRateData.ComputedHeartRate);
            Assert.Equal(0, _heartRate.HeartRateData.RRInterval);
            Assert.Equal(0, _heartRate.HeartRateData.AccumulatedHeartBeatEventTime);
        }

        [Fact]
        public void Parse_CommonHRData_ExpectedRRandBeatEventTime()
        {
            // Arrange
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0, 0, 0, 0, 0xAA, 0x59, 0x34, 70];

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(1000, _heartRate.HeartRateData.RRInterval);
            Assert.Equal(1000, _heartRate.HeartRateData.AccumulatedHeartBeatEventTime);
        }

        [Fact]
        public void Parse_CumulativeOperatingTime_ExpectedTime()
        {
            // Arrange
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x81, 1, 2, 3, 0xAA, 0x59, 0x34, 70];
            TimeSpan expTime = TimeSpan.FromSeconds(0x030201 * 2);

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(expTime, _heartRate.CumulativeOperatingTime);
        }

        [Fact]
        public void Parse_ManufacturerInfo_ExpectedManufacturerInfo()
        {
            // Arrange
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x82, 1, 2, 3, 0xAA, 0x59, 0x34, 70];
            byte manId = 1;
            uint serialNum = 0x03020000;

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(manId, _heartRate.ManufacturerInfo.ManufacturingIdLsb);
            Assert.Equal(serialNum, _heartRate.ManufacturerInfo.SerialNumber);
        }

        [Fact]
        public void Parse_ProductInfo_ExpectedProductInfo()
        {
            // Arrange
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x83, 1, 2, 3, 0xAA, 0x59, 0x34, 70];
            byte hwVer = 1;
            byte swVer = 2;
            byte model = 3;

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(hwVer, _heartRate.ProductInfo.HardwareVersion);
            Assert.Equal(swVer, _heartRate.ProductInfo.SoftwareVersion);
            Assert.Equal(model, _heartRate.ProductInfo.ModelNumber);
        }

        [Fact]
        public void Parse_PreviousHeartBeat_ExpectedRRInterval()
        {
            // Arrange
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x84, 1, 0xFF, 0xFF, 0xFF, 0x03, 0x34, 70];
            byte manSpecific = 1;
            int rrInterval = 1000;

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(manSpecific, _heartRate.PreviousHeartBeat.ManufacturerSpecific);
            Assert.Equal(rrInterval, _heartRate.PreviousHeartBeat.RRInterval);
        }

        [Fact]
        public void Parse_SwimInterval_ExpectedSummary()
        {
            // Arrange
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x85, 70, 120, 100, 0xAA, 0x59, 0x34, 70];
            byte intAvgHr = 70;
            byte intMaxHr = 120;
            byte sessionAvgHr = 100;

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(intAvgHr, _heartRate.SwimInterval.IntervalAverageHeartRate);
            Assert.Equal(intMaxHr, _heartRate.SwimInterval.IntervalMaximumHeartRate);
            Assert.Equal(sessionAvgHr, _heartRate.SwimInterval.SessionAverageHeartRate);
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
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x86, 0xFF, (byte)feature, (byte)feature, 0xAA, 0x59, 0x34, 70];

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(expFeature, _heartRate.Capabilities.Supported);
            Assert.Equal(expFeature, _heartRate.Capabilities.Enabled);
        }

        [Fact]
        public void Parse_BatteryStatus_ExpectedPctAndVoltage()
        {
            // Arrange
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x87, 70, 0x66, 0x03, 0xAA, 0x59, 0x34, 70];
            byte expPct = 70;
            double expVoltage = 3.4;

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(expPct, _heartRate.BatteryStatus.BatteryLevel);
            Assert.Equal(expVoltage, _heartRate.BatteryStatus.BatteryVoltage, 1 / 256.0);
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
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x87, 70, 120, (byte)val, 0xAA, 0x59, 0x34, 70];

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(expStatus, _heartRate.BatteryStatus.BatteryStatus);
        }

        [Theory]
        [InlineData(0xFC, HeartbeatEventType.MeasuredTimestamp)]
        [InlineData(0xFD, HeartbeatEventType.ComputedTimestamp)]
        public void Parse_DeviceInfo_ExpectedEventType(int eventType, HeartbeatEventType expEventType)
        {
            // Arrange
            _heartRate.Parse([0, 0, 0, 0, 0xAA, 0x55, 0x33, 70]);
            byte[] dataPage = [0x89, (byte)eventType, 0xFF, 0xFF, 0xAA, 0x59, 0x34, 70];

            // Act
            _heartRate.Parse(dataPage);

            // Assert
            Assert.Equal(expEventType, _heartRate.EventType);
        }

        [Fact]
        public async Task SetSportMode_SportMode_ExpectedSportModeRequest()
        {
            // Arrange
            SportMode sportMode = default;
            SubSportMode subSportMode = default;
            byte[] msg = [(byte)CommonDataPage.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)subSportMode, (byte)sportMode];
            _mockAntChannel.Setup(ac =>
            ac.SendExtAcknowledgedDataAsync(cid, msg, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            var result = await _heartRate.SetSportMode(
                sportMode,
                subSportMode);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
        }

        [Fact]
        public async Task SetHRFeature_Feature_ExpectedFeatureRequest()
        {
            // Arrange
            bool applyGymMode = false;
            bool gymMode = false;
            byte[] msg = [(byte)DataPage.HRFeature, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x00];
            _mockAntChannel.Setup(ac =>
            ac.SendExtAcknowledgedDataAsync(cid, msg, It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);

            // Act
            var result = await _heartRate.SetHRFeature(
                applyGymMode,
                gymMode);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
        }
    }
}
