using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace AntPlus.UnitTests
{
    public class CommonDataPagesTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly CommonDataPages _commonDataPages;

        public CommonDataPagesTests()
        {
            _mockLogger = new Mock<ILogger>();
            _commonDataPages = new(_mockLogger.Object);
        }

        [Fact]
        public void ParseCommandStatusPage_PropertiesCorrect()
        {
            // Arrange
            byte[] dataPage = [0x47, 0xFF, 0xFE, 0x04, 0x11, 0x22, 0x33, 0x44];
            // Act
            _commonDataPages.ParseCommonDataPage(dataPage);
            // Assert
            Assert.Equal(0xFF, _commonDataPages.CommandStatus.LastCommandReceived);
            Assert.Equal(254, _commonDataPages.CommandStatus.SequenceNumber);
            Assert.Equal(CommonDataPages.CommandResult.Pending, _commonDataPages.CommandStatus.Status);
            Assert.Equal<uint>(0x44332211, _commonDataPages.CommandStatus.ResponseData);
        }

        [Fact]
        public void ParseManufacturerInfoPage_PropertiesCorrect()
        {
            // Arrange
            byte[] dataPage = [0x50, 0xFF, 0xFF, 0x01, 0x0F, 0x00, 0x85, 0x83];
            // Act
            _commonDataPages.ParseCommonDataPage(dataPage);
            // Assert
            Assert.Equal(1, _commonDataPages.ManufacturerInfo.HardwareRevision);
            Assert.Equal(15, _commonDataPages.ManufacturerInfo.ManufacturerId);
            Assert.Equal(0x8385, _commonDataPages.ManufacturerInfo.ModelNumber);
        }

        [Theory]
        [InlineData(new byte[] { 0x51, 0xFF, 0xFF, 12, 5, 6, 7, 8 }, "1.200", (uint)0x08070605)]
        [InlineData(new byte[] { 0x51, 0xFF, 34, 12, 8, 7, 6, 5 }, "1.234", (uint)0x05060708)]
        public void ParseProductInfoPage_PropertiesCorrect(byte[] dataPage, string version, uint serialNumber)
        {
            // Act
            var currentCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            _commonDataPages.ParseCommonDataPage(dataPage);
            CultureInfo.CurrentCulture = currentCulture;

            // Assert
            Assert.Equal(serialNumber, _commonDataPages.ProductInfo.SerialNumber);
            Assert.True(_commonDataPages.ProductInfo.SoftwareRevision == Version.Parse(version));
        }

        [Theory]
        // invalid battery status, cumulative operating time resolution is 2 seconds
        [InlineData(new byte[] { 0x52, 0xFF, 0xFF, 50, 0x00, 0x00, 0xFF, 0xFF },
            BatteryStatus.Invalid, 15.996, 1, 0, 100)]
        // new battery status, 3.5 volt, 2 batteries, battery 0, cumulative operating time resolution is 16 seconds
        [InlineData(new byte[] { 0x52, 0xFF, 0x02, 13, 0x00, 0x00, 128, 0x13 },
            BatteryStatus.New, 3.5, 2, 0, 208)]
        // low battery status, 2.5 volt, 2 batteries, battery 1, cumulative operating time resolution is 16 seconds
        [InlineData(new byte[] { 0x52, 0xFF, 0x12, 0x00, 0x01, 0x00, 128, 0x42 },
            BatteryStatus.Low, 2.5, 2, 1, 4096)]
        public void ParseBatteryStatusPage_PropertiesCorrect(byte[] dataPage,
            BatteryStatus batteryStatus, double voltage, byte count, byte id, uint operatingTimeInSeconds)
        {
            // Act
            _commonDataPages.ParseCommonDataPage(dataPage);
            // Assert
            Assert.Equal(batteryStatus, _commonDataPages.BatteryStatus.Status);
            Assert.Equal(voltage, _commonDataPages.BatteryStatus.BatteryVoltage, 0.001);
            Assert.Equal(count, _commonDataPages.BatteryStatus.NumberOfBatteries);
            Assert.Equal(id, _commonDataPages.BatteryStatus.Identifier);
            Assert.Equal(TimeSpan.FromSeconds(operatingTimeInSeconds), _commonDataPages.BatteryStatus.CumulativeOperatingTime);
        }

        [Fact]
        public void ParseTimeAndDatePage_PropertiesCorrect()
        {
            // Arrange
            byte[] dataPage = [0x53, 0xFF, 0x0D, 0x1B, 0x11, 0x92, 0x06, 0x09];
            // Act
            _commonDataPages.ParseCommonDataPage(dataPage);
            // Assert
            Assert.Equal(DateTime.Parse("06/18/2009 17:27:13", DateTimeFormatInfo.InvariantInfo), _commonDataPages.TimeAndDate);
        }

        [Theory]
        [InlineData(new byte[] { 0x54, 0xFF, 0x01, 0x02, 0x6B, 0x0A, 0xEA, 0x19 },
            CommonDataPages.SubfieldDataPage.SubPage.Temperature,
            CommonDataPages.SubfieldDataPage.SubPage.BarometricPressure,
            26.67, 66.34)]
        [InlineData(new byte[] { 0x54, 0xFF, 0x03, 0x04, 0x6B, 0x0A, 0xEA, 0x19 },
            CommonDataPages.SubfieldDataPage.SubPage.Humidity,
            CommonDataPages.SubfieldDataPage.SubPage.WindSpeed,
            26.67, 66.34)]
        [InlineData(new byte[] { 0x54, 0xFF, 0x05, 0x06, 0x6B, 0x0A, 0xEA, 0x19 },
            CommonDataPages.SubfieldDataPage.SubPage.WindDirection,
            CommonDataPages.SubfieldDataPage.SubPage.ChargingCycles,
            133.35, 6634.0)]
        [InlineData(new byte[] { 0x54, 0xFF, 0x07, 0x08, 0x6B, 0x0A, 0xEA, 0x19 },
            CommonDataPages.SubfieldDataPage.SubPage.MinimumOperatingTemperature,
            CommonDataPages.SubfieldDataPage.SubPage.MaximumOperatingTemperature,
            26.67, 66.34)]
        [InlineData(new byte[] { 0x54, 0xFF, 0x01, 0x07, 0x00, 0x80, 0x00, 0x80 },
            CommonDataPages.SubfieldDataPage.SubPage.Temperature,
            CommonDataPages.SubfieldDataPage.SubPage.MinimumOperatingTemperature,
            -327.68, -327.68)]
        [InlineData(new byte[] { 0x54, 0xFF, 0x07, 0x08, 0x6B, 0x0A, 0x00, 0x80 },
            CommonDataPages.SubfieldDataPage.SubPage.MinimumOperatingTemperature,
            CommonDataPages.SubfieldDataPage.SubPage.MaximumOperatingTemperature,
            26.67, -327.68)]
        public void ParseSubfieldDataPage_PropertiesCorrect(byte[] dataPage,
            CommonDataPages.SubfieldDataPage.SubPage subPage1,
            CommonDataPages.SubfieldDataPage.SubPage subPage2,
            double subPage1Value, double subPage2Value)
        {
            // Act
            _commonDataPages.ParseCommonDataPage(dataPage);
            // Assert
            Assert.Equal(subPage1, _commonDataPages.SubfieldData.Subpage1);
            Assert.Equal(subPage1Value, _commonDataPages.SubfieldData.ComputedDataField1, 0.01);
            Assert.Equal(subPage2, _commonDataPages.SubfieldData.Subpage2);
            Assert.Equal(subPage2Value, _commonDataPages.SubfieldData.ComputedDataField2, 0.01);
        }

        [Theory]
        [InlineData(new byte[] { 0x54, 0xFF, 0x01, 0x09, 0x6B, 0x0A, 0xEA, 0x19 })]
        [InlineData(new byte[] { 0x54, 0xFF, 0x09, 0x01, 0x6B, 0x0A, 0xEA, 0x19 })]
        public void ParseSubfieldDataPage_InvalidSubPage_LogsError(byte[] dataPage)
        {
            // Act
            _commonDataPages.ParseCommonDataPage(dataPage);
            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid subfield data page")),
                    It.Is<ArgumentOutOfRangeException>(ex => ex.Message.Contains("Invalid subpage")),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(new byte[] { 0x55, 0xFF, 0xFF, 0xFF, 110, 0x00, 0x80, 0x01 }, CommonDataPages.MemorySizeUnit.KiloBits)]
        [InlineData(new byte[] { 0x55, 0xFF, 0xFF, 0xFF, 110, 0x00, 0x80, 0x02 }, CommonDataPages.MemorySizeUnit.MegaBits)]
        [InlineData(new byte[] { 0x55, 0xFF, 0xFF, 0xFF, 110, 0x00, 0x80, 0x03 }, CommonDataPages.MemorySizeUnit.TeraBits)]
        [InlineData(new byte[] { 0x55, 0xFF, 0xFF, 0xFF, 110, 0x00, 0x80, 0x81 }, CommonDataPages.MemorySizeUnit.KiloBytes)]
        [InlineData(new byte[] { 0x55, 0xFF, 0xFF, 0xFF, 110, 0x00, 0x80, 0x82 }, CommonDataPages.MemorySizeUnit.MegaBytes)]
        [InlineData(new byte[] { 0x55, 0xFF, 0xFF, 0xFF, 110, 0x00, 0x80, 0x83 }, CommonDataPages.MemorySizeUnit.TeraBytes)]
        public void ParseMemoryLevelPage_PropertiesCorrect(byte[] dataPage, CommonDataPages.MemorySizeUnit sizeUnit)
        {
            // Act
            _commonDataPages.ParseCommonDataPage(dataPage);
            // Assert
            Assert.Equal(55.0, _commonDataPages.MemoryLevel.PercentUsed, 0.5);
            Assert.Equal(3276.8, _commonDataPages.MemoryLevel.TotalSize, 0.1);
            Assert.Equal(sizeUnit, _commonDataPages.MemoryLevel.TotalSizeUnit);
        }

        [Theory]
        [InlineData(new byte[] { 0x57, 0xFF, 0x4F, 0xFF, 0x44, 0x33, 0x22, 0x11 }, CommonDataPages.ErrorLevel.Warning)]
        [InlineData(new byte[] { 0x57, 0xFF, 0x8F, 0xFF, 0x44, 0x33, 0x22, 0x11 }, CommonDataPages.ErrorLevel.Critical)]
        public void ParseErrorDescriptionPage_PropertiesCorrect(byte[] dataPage, CommonDataPages.ErrorLevel errorLevel)
        {
            // Act
            _commonDataPages.ParseCommonDataPage(dataPage);
            // Assert
            Assert.Equal(errorLevel, _commonDataPages.ErrorDescription.ErrorLevel);
            Assert.Equal(0xF, _commonDataPages.ErrorDescription.SystemComponentIndex);
            Assert.Equal(0xFF, _commonDataPages.ErrorDescription.ProfileSpecificErrorCode);
            Assert.Equal<uint>(0x11223344, _commonDataPages.ErrorDescription.ManufacturerSpecificErrorCode);
        }

        [Fact]
        public void FormatGenericCommandPage_CheckPageReturned_ReturnPageEqualsExpectedValue()
        {
            // Arrange
            ushort slaveSerialNumber = 0x1122;
            ushort slaveManufacturerId = 0x3344;
            byte sequenceNumber = 5;
            ushort command = 0x6677;

            // Act
            var result = CommonDataPages.FormatGenericCommandPage(
                slaveSerialNumber,
                slaveManufacturerId,
                sequenceNumber,
                command);

            // Assert
            Assert.True(result.SequenceEqual(new byte[] { (byte)CommonDataPage.GenericCommandPage, 0x22, 0x11, 0x44, 0x33, 5, 0x77, 0x66 }));
        }

        [Fact]
        public void FormatOpenChannelCommandPage_CheckPageReturned_ReturnPageEqualsExpectedValue()
        {
            // Arrange
            uint slaveSerialNumber = 0x11223344;
            byte deviceType = 120;
            byte frequency = 57;
            ushort channelPeriod = 8192;

            // Act
            var result = CommonDataPages.FormatOpenChannelCommandPage(
                slaveSerialNumber,
                deviceType,
                frequency,
                channelPeriod);

            // Assert
            Assert.True(result.SequenceEqual(new byte[] { (byte)CommonDataPage.OpenChannelCommand, 0x44, 0x33, 0x22, 120, 57, BitConverter.GetBytes(channelPeriod)[0], BitConverter.GetBytes(channelPeriod)[1] }));
        }

        [Fact]
        public void FormatModeSettingsPage_CheckPageReturned_ReturnPageEqualsExpectedValue()
        {
            // Arrange
            SportMode sportMode = default;
            SubSportMode subSportMode = default;

            // Act
            var result = CommonDataPages.FormatModeSettingsPage(
                sportMode,
                subSportMode);

            // Assert
            Assert.True(result.SequenceEqual(new byte[] { (byte)CommonDataPage.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)subSportMode, (byte)sportMode }));
        }
    }
}
