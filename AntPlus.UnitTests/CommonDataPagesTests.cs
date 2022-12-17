using System;

namespace AntPlus.UnitTests
{
    [TestClass]
    public class CommonDataPageTests
    {
        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.CommandStatus, 0xAA, 0x55, (byte)CommandStatus.Pass, 0x11, 0x22, 0x33, 0x44 }, (byte)0xAA, (byte)0x55, CommandStatus.Pass, (uint)0x44332211)]
        public void ParseCommonDataPage_CommandStatus_ExpectedBehavior(byte[] payload, byte commandId, byte sequenceNumber, CommandStatus commandStatus, uint responseData)
        {
            // Arrange
            CommonDataPages.CommandStatusPage cs = new();
            CommonDataPages cdp = new();
            cdp.CommandStatusPageChanged += (s, e) => cs = e;

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(commandId, cs.LastCommandReceived, "Command ID");
            Assert.AreEqual(sequenceNumber, cs.SequenceNumber, "Sequence number");
            Assert.AreEqual(commandStatus, cs.Status, "Command status");
            Assert.AreEqual(responseData, cs.ResponseData, "Response data");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.MultiComponentManufacturerInfo, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00 }, 15, 0)]
        [DataRow(new byte[] { (byte)CommonDataPage.MultiComponentManufacturerInfo, 0x00, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 15)]
        public void ParseCommonDataPage_MultiComponentManufactureInfo_ExpectedBehavior(byte[] payload, int numberOfComponents, int componentId)
        {
            // Arrange
            CommonDataPages cdp = new();

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(numberOfComponents, cdp.NumberOfComponents, "Multi-Component number of components");
            Assert.AreEqual(componentId, cdp.ComponentId, "Multi-Component ID");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.ManufacturerInfo, 0xFF, 0xFF, 0x11, 0x22, 0x33, 0x44, 0x55 }, (byte)0x11, (ushort)0x3322, (ushort)0x5544)]
        public void ParseCommonDataPage_ManufacturerInfo_ExpectedBehavior(byte[] payload, byte hwRev, ushort manId, ushort modelNumber)
        {
            // Arrange
            CommonDataPages.ManufacturerInfoPage mi = new();
            CommonDataPages cdp = new();
            cdp.ManufacturerInfoPageChanged += (s, e) => mi = e;

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(hwRev, mi.HardwareRevision, "HW revision");
            Assert.AreEqual(manId, mi.ManufacturerId, "Manufacturer ID");
            Assert.AreEqual(modelNumber, mi.ModelNumber, "Model number");
        }
        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.MultiComponentProductInfo, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 15, 0)]
        [DataRow(new byte[] { (byte)CommonDataPage.MultiComponentProductInfo, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 15)]
        public void ParseCommonDataPage_MultiComponentProductInfo_ExpectedBehavior(byte[] payload, int numberOfComponents, int componentId)
        {
            // Arrange
            CommonDataPages cdp = new();

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(numberOfComponents, cdp.NumberOfComponents, "Multi-Component number of components");
            Assert.AreEqual(componentId, cdp.ComponentId, "Multi-Component ID");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.ProductInfo, 0xFF, 0x50, 0x0D, 0x11, 0x22, 0x33, 0x44 }, "1.380", (uint)0x44332211)]
        [DataRow(new byte[] { (byte)CommonDataPage.ProductInfo, 0xFF, 0x00, 0x00, 0x11, 0x22, 0x33, 0x44 }, "0.0", (uint)0x44332211)]
        [DataRow(new byte[] { (byte)CommonDataPage.ProductInfo, 0xFF, 0xFE, 0xFF, 0x11, 0x22, 0x33, 0x44 }, "25.754", (uint)0x44332211)]
        [DataRow(new byte[] { (byte)CommonDataPage.ProductInfo, 0xFF, 0xFF, 0x00, 0x11, 0x22, 0x33, 0x44 }, "0.0", (uint)0x44332211)]
        [DataRow(new byte[] { (byte)CommonDataPage.ProductInfo, 0xFF, 0xFF, 0xFF, 0x11, 0x22, 0x33, 0x44 }, "25.500", (uint)0x44332211)]
        public void ParseCommonDataPage_ProductInfo_ExpectedBehavior(byte[] payload, string swVersion, uint serialNumber)
        {
            // Arrange
            CommonDataPages.ProductInfoPage pi = new();
            CommonDataPages cdp = new();
            cdp.ProductInfoPageChanged += (s, e) => pi = e;

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(Version.Parse(swVersion), pi.SoftwareRevision, "Software version");
            Assert.AreEqual(serialNumber, pi.SerialNumber, "Serial number");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xEE, 0x00, 0x00, 0x00, 0xFF, 0x0F }, 14, 14, 0, 15.99609375, BatteryStatus.Unknown)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x80, 0x96 }, 0, 0, 510, 6.5, BatteryStatus.New)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x80, 0x25 }, 1, 0, 4080, 5.5, BatteryStatus.Good)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x80, 0xB4 }, 1, 0, 131070, 4.5, BatteryStatus.Ok)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x80, 0x43 }, 1, 0, 1048560, 3.5, BatteryStatus.Low)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x80, 0xD2 }, 1, 0, 33554430, 2.5, BatteryStatus.Critical)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x80, 0x61 }, 1, 0, 268435440, 1.5, BatteryStatus.Reserved)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 1, 0, 33554430, 15.99609375, BatteryStatus.Invalid)]
        public void ParseCommonDataPage_BatteryStatus_ExpectedBehavior(byte[] payload, int numberOfBatt, int battId, int seconds, double voltage, BatteryStatus batteryStatus)
        {
            // Arrange
            CommonDataPages.BatteryStatusPage bsp = new();
            CommonDataPages cdp = new();
            cdp.BatteryStatusPageChanged += (s, e) => bsp = e;

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(numberOfBatt, bsp.NumberOfBatteries, "Number of batteries");
            Assert.AreEqual(battId, bsp.Identifier, "Battery ID");
            Assert.AreEqual(batteryStatus, bsp.BatteryStatus, "Battery status");
            Assert.AreEqual(voltage, bsp.BatteryVoltage, "Battery voltage");
            Assert.AreEqual(TimeSpan.FromSeconds(seconds), bsp.CumulativeOperatingTime, "Cumulative operating time");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.TimeAndDate, 0xFF, 0, 0, 0, 1, 1, 0 }, "1/1/2000 0:0:0 AM")]
        [DataRow(new byte[] { (byte)CommonDataPage.TimeAndDate, 0xFF, 59, 59, 23, 31, 12, 255 }, "12/31/2255 11:59:59 PM")]
        public void ParseCommonDataPage_TimeAndDate_ExpectedBehavior(byte[] payload, string dateTime)
        {
            // Arrange
            DateTime dt = DateTime.Now;
            CommonDataPages cdp = new();
            cdp.DateTimeChanged += (s, e) => dt = e;

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(DateTime.Parse(dateTime), dt);
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.SubfieldData, 0xFF, 0x01, 0x05, 0x00, 0x80, 0x01F, 0x1C }, -327.68, 359.95)]
        [DataRow(new byte[] { (byte)CommonDataPage.SubfieldData, 0xFF, 0x02, 0x06, 0xFF, 0xFF, 0xFF, 0x10 }, 655.35, 4351)]
        [DataRow(new byte[] { (byte)CommonDataPage.SubfieldData, 0xFF, 0x03, 0x07, 0x0F, 0x27, 0x00, 0x80 }, 99.99, -327.68)]
        [DataRow(new byte[] { (byte)CommonDataPage.SubfieldData, 0xFF, 0x04, 0x08, 0xFF, 0xFF, 0xFF, 0x7F }, 655.35, 327.67)]
        public void ParseCommonDataPage_SubfieldData_ExpectedBehavior(byte[] payload, double field1, double field2)
        {
            // Arrange
            CommonDataPages cdp = new();

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(field1, cdp.SubfieldData.ComputedDataField1, cdp.SubfieldData.Subpage1.ToString());
            Assert.AreEqual(field2, cdp.SubfieldData.ComputedDataField2, cdp.SubfieldData.Subpage2.ToString());
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.MemoryLevel, 0xFF, 0xFF, 0xFF, 1, 0x00, 0x01, 0x00 }, 0.5, 25.6, MemorySizeUnit.Bits)]
        [DataRow(new byte[] { (byte)CommonDataPage.MemoryLevel, 0xFF, 0xFF, 0xFF, 199, 0xFF, 0xFF, 0xFF }, 99.5, 6553.5, MemorySizeUnit.TeraBytes)]
        public void ParseCommonDataPage_MemoryLevel_ExpectedBehavior(byte[] payload, double used, double total, MemorySizeUnit memorySizeUnit)
        {
            // Arrange
            CommonDataPages cdp = new();

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(used, cdp.MemoryLevel.PercentUsed, "Percent used");
            Assert.AreEqual(total, cdp.MemoryLevel.TotalSize, "Total size");
            Assert.AreEqual(memorySizeUnit, cdp.MemoryLevel.TotalSizeUnit, "Memory size unit");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.PairedDevices, 0xFF, 0x00, 0x00, 0x00, 0x11, 0x22, 0x33 }, 255, 0, false, ConnectionState.Closed, NetworkKey.Public, (uint)0x33221100)]
        [DataRow(new byte[] { (byte)CommonDataPage.PairedDevices, 0x00, 0xFF, 0x80, 0x33, 0x22, 0x11, 0x00 }, 0, 255, true, ConnectionState.Closed, NetworkKey.Public, (uint)0x00112233)]
        [DataRow(new byte[] { (byte)CommonDataPage.PairedDevices, 0x00, 0x00, 0x89, 0x00, 0x00, 0x00, 0x00 }, 0, 0, true, ConnectionState.Searching, NetworkKey.Private, (uint)0)]
        [DataRow(new byte[] { (byte)CommonDataPage.PairedDevices, 0x00, 0x00, 0x92, 0x00, 0x00, 0x00, 0x00 }, 0, 0, true, ConnectionState.Synchronized, NetworkKey.AntPlusManaged, (uint)0)]
        [DataRow(new byte[] { (byte)CommonDataPage.PairedDevices, 0x00, 0x00, 0x83, 0x00, 0x00, 0x00, 0x00 }, 0, 0, true, ConnectionState.Closed, NetworkKey.AntFS, (uint)0)]
        public void ParseCommonDataPage_PairedDevices_ExpectedBehavior(byte[] payload, int index, int total, bool paired, ConnectionState connectionState, NetworkKey networkKey, uint deviceId)
        {
            // Arrange
            CommonDataPages cdp = new();

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(index, cdp.PairedDevices[0].Index, "Paired device index");
            Assert.AreEqual(total, cdp.NumberOfConnectedDevices, "Connected devices");
            Assert.AreEqual(paired, cdp.IsPaired, "Pairing state");
            Assert.AreEqual(connectionState, cdp.ConnectionState, "Connection state");
            Assert.AreEqual(networkKey, cdp.NetworkKey, "Network key");
            Assert.AreEqual(deviceId, cdp.PairedDevices[0].PeripheralDeviceId, "Peripheral device ID");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.ErrorDescription, 0xFF, 0xCF, 0x00, 0x00, 0x00, 0x00, 0x00 }, 15, ErrorLevel.Reserved, 0, (uint)0)]
        [DataRow(new byte[] { (byte)CommonDataPage.ErrorDescription, 0xFF, 0x4F, 0x80, 0xFF, 0xFF, 0x00, 0x00 }, 15, ErrorLevel.Warning, 128, (uint)ushort.MaxValue)]
        [DataRow(new byte[] { (byte)CommonDataPage.ErrorDescription, 0xFF, 0x8F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 15, ErrorLevel.Critical, 255, uint.MaxValue)]
        public void ParseCommonDataPage_ErrorDescription_ExpectedBehavior(byte[] payload, int componentIndex, ErrorLevel errorLevel, int profileErrorCode, uint manufacturerErrorCode)
        {
            // Arrange
            CommonDataPages cdp = new();

            // Act
            cdp.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(componentIndex, cdp.ErrorDescription.SystemComponentIndex, "Component index");
            Assert.AreEqual(errorLevel, cdp.ErrorDescription.ErrorLevel, "Error level");
            Assert.AreEqual(profileErrorCode, cdp.ErrorDescription.ProfileSpecificErrorCode, "Profile error code");
            Assert.AreEqual(manufacturerErrorCode, cdp.ErrorDescription.ManufacturerSpecificErrorCode, "Manufacturer error code");
        }
    }
}
