using System;

namespace AntPlus.UnitTests
{
    [TestClass]
    public class CommonDataPagesTests
    {
        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.CommandStatus, 0xAA, 0x55, (byte)CommandStatus.Pass, 0x11, 0x22, 0x33, 0x44 }, (byte)0xAA, (byte)0x55, CommandStatus.Pass, (uint)0x44332211)]
        public void ParseCommonDataPage_CommandStatus_ExpectedBehavior(byte[] payload, byte commandId, byte sequenceNumber, CommandStatus commandStatus, uint responseData)
        {
            // Arrange
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(commandId, commonDataPages.LastCommandReceived, "Command ID");
            Assert.AreEqual(sequenceNumber, commonDataPages.SequenceNumber, "Sequence number");
            Assert.AreEqual(commandStatus, commonDataPages.CommandStatus, "Command status");
            Assert.AreEqual(responseData, commonDataPages.ResponseData, "Response data");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.MultiComponentManufacturerInfo, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00 }, 15, 0)]
        [DataRow(new byte[] { (byte)CommonDataPage.MultiComponentManufacturerInfo, 0x00, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 15)]
        public void ParseCommonDataPage_MultiComponentManufactureInfo_ExpectedBehavior(byte[] payload, int numberOfComponents, int componentId)
        {
            // Arrange
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(numberOfComponents, commonDataPages.NumberOfComponents, "Multi-Component number of components");
            Assert.AreEqual(componentId, commonDataPages.ComponentId, "Multi-Component ID");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.ManufacturerInfo, 0xFF, 0xFF, 0x11, 0x22, 0x33, 0x44, 0x55 }, (byte)0x11, (ushort)0x3322, (ushort)0x5544)]
        public void ParseCommonDataPage_ManufacturerInfo_ExpectedBehavior(byte[] payload, byte hwRev, ushort manId, ushort modelNumber)
        {
            // Arrange
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(hwRev, commonDataPages.Manufacturer.HardwareRevision, "HW revision");
            Assert.AreEqual(manId, commonDataPages.Manufacturer.ManufacturerId, "Manufacturer ID");
            Assert.AreEqual(modelNumber, commonDataPages.Manufacturer.ModelNumber, "Model number");
        }
        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.MultiComponentProductInfo, 0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 15, 0)]
        [DataRow(new byte[] { (byte)CommonDataPage.MultiComponentProductInfo, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0, 15)]
        public void ParseCommonDataPage_MultiComponentProductInfo_ExpectedBehavior(byte[] payload, int numberOfComponents, int componentId)
        {
            // Arrange
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(numberOfComponents, commonDataPages.NumberOfComponents, "Multi-Component number of components");
            Assert.AreEqual(componentId, commonDataPages.ComponentId, "Multi-Component ID");
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
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(Version.Parse(swVersion), commonDataPages.Product.SoftwareRevision, "Software version");
            Assert.AreEqual(serialNumber, commonDataPages.Product.SerialNumber, "Serial number");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xEE, 0x00, 0x00, 0x00, 0xFF, 0x0F }, 14, 14, 0, 15.99609375, BatteryStatus.Unknown)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x80, 0x96 }, 0, 0, 510, 6.5, BatteryStatus.New)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x80, 0x25 }, 0, 0, 4080, 5.5, BatteryStatus.Good)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x80, 0xB4 }, 0, 0, 131070, 4.5, BatteryStatus.Ok)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x80, 0x43 }, 0, 0, 1048560, 3.5, BatteryStatus.Low)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x80, 0xD2 }, 0, 0, 33554430, 2.5, BatteryStatus.Critical)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x80, 0x61 }, 0, 0, 268435440, 1.5, BatteryStatus.Reserved)]
        [DataRow(new byte[] { (byte)CommonDataPage.BatteryStatus, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 0, 0, 33554430, 15.99609375, BatteryStatus.Invalid)]
        public void ParseCommonDataPage_BatteryStatus_ExpectedBehavior(byte[] payload, int numberOfBatt, int battId, int seconds, double voltage, BatteryStatus batteryStatus)
        {
            // Arrange
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(numberOfBatt, commonDataPages.BatteryStatus.NumberOfBatteries, "Number of batteries");
            Assert.AreEqual(battId, commonDataPages.BatteryStatus.Identifier, "Battery ID");
            Assert.AreEqual(batteryStatus, commonDataPages.BatteryStatus.BatteryStatus, "Battery status");
            Assert.AreEqual(voltage, commonDataPages.BatteryStatus.BatteryVoltage, "Battery voltage");
            Assert.AreEqual(TimeSpan.FromSeconds(seconds), commonDataPages.BatteryStatus.CumulativeOperatingTime, "Cumulative operating time");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.TimeAndDate, 0xFF, 0, 0, 0, 1, 1, 0 }, "1/1/2000 0:0:0 AM")]
        [DataRow(new byte[] { (byte)CommonDataPage.TimeAndDate, 0xFF, 59, 59, 23, 31, 12, 255 }, "12/31/2255 11:59:59 PM")]
        public void ParseCommonDataPage_TimeAndDate_ExpectedBehavior(byte[] payload, string dateTime)
        {
            // Arrange
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(DateTime.Parse(dateTime), commonDataPages.TimeAndDate);
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.SubfieldData, 0xFF, 0x01, 0x05, 0x00, 0x80, 0x01F, 0x1C }, -327.68, 0.0, 0.0, 0.0, 359.95, 0, 0.0, 0.0)]
        [DataRow(new byte[] { (byte)CommonDataPage.SubfieldData, 0xFF, 0x02, 0x06, 0xFF, 0xFF, 0xFF, 0x10 }, 0.0, 655.35, 0.0, 0.0, 0.0, 4351, 0.0, 0.0)]
        [DataRow(new byte[] { (byte)CommonDataPage.SubfieldData, 0xFF, 0x03, 0x07, 0x0F, 0x27, 0x00, 0x80 }, 0.0, 0.0, 99.99, 0.0, 0.0, 0, -327.68, 0.0)]
        [DataRow(new byte[] { (byte)CommonDataPage.SubfieldData, 0xFF, 0x04, 0x08, 0xFF, 0xFF, 0xFF, 0x7F }, 0.0, 0.0, 0.0, 655.35, 0.0, 0, 0.0, 327.67)]
        public void ParseCommonDataPage_SubfieldData_ExpectedBehavior(byte[] payload, double temperature, double pressure, double humidity, double windSpeed, double windDirection, int chargingCycles, double minTemp, double maxTem)
        {
            // Arrange
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(temperature, commonDataPages.Temperature, "Temperature");
            Assert.AreEqual(pressure, commonDataPages.BarometricPressue, "Barometric Pressure");
            Assert.AreEqual(humidity, commonDataPages.Humidity, "Humidity");
            Assert.AreEqual(windSpeed, commonDataPages.WindSpeed, "Wind speed");
            Assert.AreEqual(windDirection, commonDataPages.WindDirection, "Wind direction");
            Assert.AreEqual(chargingCycles, commonDataPages.ChargingCycles, "Charging cycles");
            Assert.AreEqual(minTemp, commonDataPages.MinimumOperatingTemperature, "Min operating temp");
            Assert.AreEqual(maxTem, commonDataPages.MaximumOperatingTemperature, "Max operating temp");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.MemoryLevel, 0xFF, 0xFF, 0xFF, 1, 0x00, 0x01, 0x00 }, 0.5, 25.6, MemorySizeUnit.Bits)]
        [DataRow(new byte[] { (byte)CommonDataPage.MemoryLevel, 0xFF, 0xFF, 0xFF, 199, 0xFF, 0xFF, 0xFF }, 99.5, 6553.5, MemorySizeUnit.TeraBytes)]
        public void ParseCommonDataPage_MemoryLevel_ExpectedBehavior(byte[] payload, double used, double total, MemorySizeUnit memorySizeUnit)
        {
            // Arrange
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(used, commonDataPages.PercentUsed, "Percent used");
            Assert.AreEqual(total, commonDataPages.TotalSize, "Total size");
            Assert.AreEqual(memorySizeUnit, commonDataPages.TotalSizeUnit, "Memory size unit");
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
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(index, commonDataPages.PairedDevices[0].Index, "Paired device index");
            Assert.AreEqual(total, commonDataPages.NumberOfConnectedDevices, "Connected devices");
            Assert.AreEqual(paired, commonDataPages.IsPaired, "Pairing state");
            Assert.AreEqual(connectionState, commonDataPages.ConnectionState, "Connection state");
            Assert.AreEqual(networkKey, commonDataPages.NetworkKey, "Network key");
            Assert.AreEqual(deviceId, commonDataPages.PairedDevices[0].PeripheralDeviceId, "Peripheral device ID");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)CommonDataPage.ErrorDescription, 0xFF, 0xCF, 0x00, 0x00, 0x00, 0x00, 0x00 }, 15, ErrorLevel.Reserved, 0, (uint)0)]
        [DataRow(new byte[] { (byte)CommonDataPage.ErrorDescription, 0xFF, 0x4F, 0x80, 0xFF, 0xFF, 0x00, 0x00 }, 15, ErrorLevel.Warning, 128, (uint)ushort.MaxValue)]
        [DataRow(new byte[] { (byte)CommonDataPage.ErrorDescription, 0xFF, 0x8F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 15, ErrorLevel.Critical, 255, uint.MaxValue)]
        public void ParseCommonDataPage_ErrorDescription_ExpectedBehavior(byte[] payload, int componentIndex, ErrorLevel errorLevel, int profileErrorCode, uint manufacturerErrorCode)
        {
            // Arrange
            CommonDataPages commonDataPages = new();

            // Act
            commonDataPages.ParseCommonDataPage(
                payload);

            // Assert
            Assert.AreEqual(componentIndex, commonDataPages.SystemComponentIndex, "Component index");
            Assert.AreEqual(errorLevel, commonDataPages.ErrorLevel, "Error level");
            Assert.AreEqual(profileErrorCode, commonDataPages.ProfileSpecificErrorCode, "Profile error code");
            Assert.AreEqual(manufacturerErrorCode, commonDataPages.ManufacturerSpecificErrorCode, "Manufacturer error code");
        }
    }
}
