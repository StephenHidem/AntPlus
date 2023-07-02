using SmallEarthTech.AntPlus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntPlus.UnitTests
{
    [TestClass]
    public class CommonDataPagesTests
    {
        [TestMethod]
        public void ParseCommonDataPage_SupportedPages_PropertiesCorrect()
        {
            // Arrange
            CommonDataPages commonDataPage = new(null);
            List<byte[]> commonDataPages = new()
            {
                new byte[8] { 0x47, 0xFF, 0xFE, 0x04, 0x11, 0x22, 0x33, 0x44 },   // command status
                new byte[8] { 0x50, 0xFF, 0xFF, 0x01, 0x0F, 0x00, 0x85, 0x83 },   // manufacturer ID
                new byte[8] { 0x51, 0xFF, 0xFF, 0x01, 0x01, 0x00, 0x00, 0x00 },   // product info
                new byte[8] { 0x52, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x55, 0x93 },   // battery status
                new byte[8] { 0x53, 0xFF, 0x0D, 0x1B, 0x11, 0x92, 0x06, 0x09 },   // time and date
                new byte[8] { 0x54, 0xFF, 0x01, 0x03, 0x6B, 0x0A, 0xEA, 0x19 },   // subfield data
                new byte[8] { 0x55, 0xFF, 0xFF, 0xFF, 110, 0x00, 0x80, 0x81 },    // memory level
                new byte[8] { 0x57, 0xFF, 0x4F, 0xFF, 0x44, 0x33, 0x22, 0x11 },   // error description
            };

            // Act
            foreach (var dataPage in commonDataPages)
            {
                commonDataPage.ParseCommonDataPage(
                        dataPage);
            }

            // Assert
            Assert.IsTrue(commonDataPage.CommandStatus.LastCommandReceived == 0xFF);
            Assert.IsTrue(commonDataPage.CommandStatus.SequenceNumber == 254);
            Assert.IsTrue(commonDataPage.CommandStatus.Status == CommonDataPages.CommandResult.Pending);
            Assert.IsTrue(commonDataPage.CommandStatus.ResponseData == 0x44332211);
            Assert.IsTrue(commonDataPage.ManufacturerInfo.ManufacturerId == 15);
            Assert.IsTrue(commonDataPage.ManufacturerInfo.HardwareRevision == 1);
            Assert.IsTrue(commonDataPage.ManufacturerInfo.ModelNumber == 0x8385);
            Assert.IsTrue(commonDataPage.ProductInfo.SoftwareRevision == Version.Parse("0.100"));
            Assert.IsTrue(commonDataPage.ProductInfo.SerialNumber == 1);
            Assert.IsTrue(commonDataPage.BatteryStatus.Status == BatteryStatus.New);
            Assert.IsTrue(commonDataPage.BatteryStatus.BatteryVoltage == 3.33203125);
            Assert.IsTrue(commonDataPage.BatteryStatus.NumberOfBatteries == 1);
            Assert.IsTrue(commonDataPage.BatteryStatus.Identifier == 0);
            Assert.IsTrue(commonDataPage.BatteryStatus.CumulativeOperatingTime == TimeSpan.Zero);
            Assert.IsTrue(commonDataPage.TimeAndDate == DateTime.Parse("06/18/2009 17:27:13"));
            Assert.IsTrue(commonDataPage.SubfieldData.Subpage1 == CommonDataPages.SubfieldDataPage.SubPage.Temperature);
            Assert.IsTrue(commonDataPage.SubfieldData.ComputedDataField1 == 26.67);
            Assert.IsTrue(commonDataPage.SubfieldData.Subpage2 == CommonDataPages.SubfieldDataPage.SubPage.Humidity);
            Assert.IsTrue(commonDataPage.SubfieldData.ComputedDataField2 == 66.34);
            Assert.IsTrue(commonDataPage.MemoryLevel.PercentUsed == 55.0);
            Assert.IsTrue(commonDataPage.MemoryLevel.TotalSize == 3276.8);
            Assert.IsTrue(commonDataPage.MemoryLevel.TotalSizeUnit == CommonDataPages.MemorySizeUnit.KiloBytes);
            Assert.IsTrue(commonDataPage.ErrorDescription.ErrorLevel == CommonDataPages.ErrorLevel.Warning);
            Assert.IsTrue(commonDataPage.ErrorDescription.SystemComponentIndex == 0xF);
            Assert.IsTrue(commonDataPage.ErrorDescription.ProfileSpecificErrorCode == 0xFF);
            Assert.IsTrue(commonDataPage.ErrorDescription.ManufacturerSpecificErrorCode == 0x11223344);
        }

        [TestMethod]
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
            Assert.IsTrue(result.SequenceEqual(new byte[] { (byte)CommonDataPage.GenericCommandPage, 0x22, 0x11, 0x44, 0x33, 5, 0x77, 0x66 }));
        }

        [TestMethod]
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
            Assert.IsTrue(result.SequenceEqual(new byte[] { (byte)CommonDataPage.OpenChannelCommand, 0x44, 0x33, 0x22, 120, 57, BitConverter.GetBytes(channelPeriod)[0], BitConverter.GetBytes(channelPeriod)[1] }));
        }

        [TestMethod]
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
            Assert.IsTrue(result.SequenceEqual(new byte[] { (byte)CommonDataPage.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)subSportMode, (byte)sportMode }));
        }
    }
}
