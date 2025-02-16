using SmallEarthTech.AntPlus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace AntPlus.UnitTests
{
    public class CommonDataPagesTests
    {
        [Fact]
        public void ParseCommonDataPage_SupportedPages_PropertiesCorrect()
        {
            // Arrange
            CommonDataPages commonDataPage = new(null);
            List<byte[]> commonDataPages =
                [
                    [0x47, 0xFF, 0xFE, 0x04, 0x11, 0x22, 0x33, 0x44],   // command status
                    [0x50, 0xFF, 0xFF, 0x01, 0x0F, 0x00, 0x85, 0x83],   // manufacturer ID
                    [0x52, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x55, 0x93],   // battery status
                    [0x53, 0xFF, 0x0D, 0x1B, 0x11, 0x92, 0x06, 0x09],   // time and date
                    [0x54, 0xFF, 0x01, 0x03, 0x6B, 0x0A, 0xEA, 0x19],   // subfield data
                    [0x55, 0xFF, 0xFF, 0xFF, 110, 0x00, 0x80, 0x81],    // memory level
                    [0x57, 0xFF, 0x4F, 0xFF, 0x44, 0x33, 0x22, 0x11],   // error description
                ];

            // Act
            foreach (var dataPage in commonDataPages)
            {
                commonDataPage.ParseCommonDataPage(
                        dataPage);
            }

            // Assert
            Assert.Equal(0xFF, commonDataPage.CommandStatus.LastCommandReceived);
            Assert.Equal(254, commonDataPage.CommandStatus.SequenceNumber);
            Assert.Equal(CommonDataPages.CommandResult.Pending, commonDataPage.CommandStatus.Status);
            Assert.Equal<uint>(0x44332211, commonDataPage.CommandStatus.ResponseData);
            Assert.Equal(15, commonDataPage.ManufacturerInfo.ManufacturerId);
            Assert.Equal(1, commonDataPage.ManufacturerInfo.HardwareRevision);
            Assert.Equal(0x8385, commonDataPage.ManufacturerInfo.ModelNumber);
            Assert.Equal(BatteryStatus.New, commonDataPage.BatteryStatus.Status);
            Assert.Equal(3.33203125, commonDataPage.BatteryStatus.BatteryVoltage);
            Assert.Equal(1, commonDataPage.BatteryStatus.NumberOfBatteries);
            Assert.Equal(0, commonDataPage.BatteryStatus.Identifier);
            Assert.True(commonDataPage.BatteryStatus.CumulativeOperatingTime == TimeSpan.Zero);
            Assert.True(commonDataPage.TimeAndDate == DateTime.Parse("06/18/2009 17:27:13", DateTimeFormatInfo.InvariantInfo));
            Assert.Equal(CommonDataPages.SubfieldDataPage.SubPage.Temperature, commonDataPage.SubfieldData.Subpage1);
            Assert.Equal(26.67, commonDataPage.SubfieldData.ComputedDataField1);
            Assert.Equal(CommonDataPages.SubfieldDataPage.SubPage.Humidity, commonDataPage.SubfieldData.Subpage2);
            Assert.Equal(66.34, commonDataPage.SubfieldData.ComputedDataField2);
            Assert.Equal(55.0, commonDataPage.MemoryLevel.PercentUsed);
            Assert.Equal(3276.8, commonDataPage.MemoryLevel.TotalSize);
            Assert.Equal(CommonDataPages.MemorySizeUnit.KiloBytes, commonDataPage.MemoryLevel.TotalSizeUnit);
            Assert.Equal(CommonDataPages.ErrorLevel.Warning, commonDataPage.ErrorDescription.ErrorLevel);
            Assert.Equal(0xF, commonDataPage.ErrorDescription.SystemComponentIndex);
            Assert.Equal(0xFF, commonDataPage.ErrorDescription.ProfileSpecificErrorCode);
            Assert.Equal<uint>(0x11223344, commonDataPage.ErrorDescription.ManufacturerSpecificErrorCode);
        }

        [Theory]
        [InlineData(new byte[] { 0x51, 0xFF, 0xFF, 12, 5, 6, 7, 8 }, "1.200", (uint)0x08070605)]
        [InlineData(new byte[] { 0x51, 0xFF, 34, 12, 8, 7, 6, 5 }, "1.234", (uint)0x05060708)]
        public void ParseCommonDataPage_ProductInfo_PropertiesCorrect(byte[] dataPage, string version, uint serialNumber)
        {
            // Arrange
            CommonDataPages commonDataPage = new(null);

            // Act
            var currentCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            commonDataPage.ParseCommonDataPage(dataPage);
            CultureInfo.CurrentCulture = currentCulture;

            // Assert
            Assert.Equal(serialNumber, commonDataPage.ProductInfo.SerialNumber);
            Assert.True(commonDataPage.ProductInfo.SoftwareRevision == Version.Parse(version));
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
