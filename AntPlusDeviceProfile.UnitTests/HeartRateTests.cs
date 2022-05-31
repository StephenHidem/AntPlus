using AntPlus;
using AntPlusDeviceProfiles;

namespace AntPlusDeviceProfile.UnitTests
{
    [TestClass]
    public class HeartRateTests
    {
        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Default, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF, 0x00 }, 0, 0, 0)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Default, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF }, 0, 0, 255)]
        public void Parse_CommonHRData_ExpectedBehavior(byte[] payload, int eventTime, int hrBeatCount, int computedHr)
        {
            // Arrange
            var heartRate = new HeartRate(payload, 0);

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(eventTime, heartRate.AccumulatedHeartBeatEventTime, "AccumulatedHeartBeatEventTime");
            Assert.AreEqual(hrBeatCount, heartRate.AccumulatedHeartBeatCount, "AccumulatedHeartBeatCount");
            Assert.AreEqual(computedHr, heartRate.ComputedHeartRate, "ComputedHeartRate");
        }

        [TestMethod]
        public void Parse_AccumulatedValueRollover_ExpectedBehavior()
        {
            // Arrange
            var heartRate = new HeartRate(new byte[] { (byte)HeartRate.DataPage.Default | 0x80, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x40 }, 0);

            // Act
            heartRate.Parse(
                new byte[] { (byte)HeartRate.DataPage.Default, 0xFF, 0x02, 0x00, 0x02, 0x00, 0x01, 0x46 });

            // Assert
            Assert.AreEqual(3, heartRate.AccumulatedHeartBeatEventTime, "AccumulatedHeartBeatEventTime");
            Assert.AreEqual(2, heartRate.AccumulatedHeartBeatCount, "AccumulatedHeartBeatCount");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.CumulativeOperatingTime, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00 }, 0)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.CumulativeOperatingTime, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF }, 33554430)]
        public void Parse_CumulativeOperatingTime_ExpectedBehavior(byte[] payload, int cumulativeOpTime)
        {
            // Arrange
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0);

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(cumulativeOpTime, heartRate.CumulativeOperatingTime.TotalSeconds, "CumulativeOperatingTime");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.ManufacturerInfo, 0x80, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00 }, (byte)128, (uint)21930)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.ManufacturerInfo, 0x08, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF }, (byte)8, 4294923690)]
        public void Parse_ManufacturerInformation_ExpectedBehavior(byte[] payload, byte manId, uint sn)
        {
            // Arrange
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0xF00055AA);

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(manId, heartRate.ManufacturingIdLsb, "ManufacturingIdLsb");
            Assert.AreEqual(sn, heartRate.SerialNumber, "SerialNumber");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.ProductInfo, 0x11, 0x22, 0x33, 0, 0, 0, 0 }, (byte)17, (byte)34, (byte)51)]
        public void Parse_ProductInformation_ExpectedBehavior(byte[] payload, byte hwVersion, byte swVersion, byte modelNumber)
        {
            // Arrange
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0xF00055AA);

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(hwVersion, heartRate.HardwareVersion, "HardwareVersion");
            Assert.AreEqual(swVersion, heartRate.SoftwareVersion, "SoftwareVersion");
            Assert.AreEqual(modelNumber, heartRate.ModelNumber, "ModelNumber");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.PreviousHeartBeat, 0xFF, 0x88, 0x06, 0xDD, 0x07, 0x83, 0xB4 }, 333)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.PreviousHeartBeat, 0xFF, 0x88, 0xFE, 0x15, 0x00, 0x83, 0xB4 }, 387)]
        public void Parse_PreviousHeartBeat_ExpectedBehavior(byte[] payload, int rrInterval)
        {
            // Arrange
            var heartRate = new HeartRate(new byte[] { (byte)HeartRate.DataPage.Default | 0x80, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF, 0x00 }, 0);

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(rrInterval, heartRate.RRInterval, "RRInterval");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.SwimInterval, 0x11, 0x22, 0x33, 0, 0, 0, 0 }, (byte)17, (byte)34, (byte)51)]
        public void Parse_SwimInterval_ExpectedBehavior(byte[] payload, byte intervalAvgHr, byte intervalMaxHr, byte sessionAvgHr)
        {
            // Arrange
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0xF00055AA);

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(intervalAvgHr, heartRate.IntervalAverageHeartRate, "IntervalAverageHeartRate");
            Assert.AreEqual(intervalMaxHr, heartRate.IntervalMaximumHeartRate, "IntervalMaximumHeartRate");
            Assert.AreEqual(sessionAvgHr, heartRate.SessionAverageHeartRate, "SessionAverageHeartRate");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Capabilities, 0xFF, 0x01, 0x01, 0, 0, 0, 0 }, HeartRate.Features.Running, HeartRate.Features.Running)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Capabilities, 0xFF, 0x02, 0x02, 0, 0, 0, 0 }, HeartRate.Features.Cycling, HeartRate.Features.Cycling)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Capabilities, 0xFF, 0x04, 0x04, 0, 0, 0, 0 }, HeartRate.Features.Swimming, HeartRate.Features.Swimming)]
        public void Parse_Capabilities_ExpectedBehavior(byte[] payload, HeartRate.Features supportedFeatures, HeartRate.Features enabledFeatures)
        {
            // Arrange
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0xF00055AA);

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(supportedFeatures, heartRate.Supported, "Supported features");
            Assert.AreEqual(enabledFeatures, heartRate.Enabled, "Enabled features");
            //Assert.AreEqual(enabledFeatures, heartRate.ManufacturerSpecificFeatures, "Manufacturer specific features");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.BatteryStatus, 50, 0xFF, 0x3F, 0x00, 0x00, 0xFF, 0x0F }, 50, 15.99609375, BatteryStatus.Ok)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.BatteryStatus, 0, 0x00, 0x50, 0x00, 0x00, 0xFF, 0x0F }, 0, 0.0, BatteryStatus.Critical)]
        public void Parse_BatteryStatus_ExpectedBehavior(byte[] payload, int battPerCent, double voltage, BatteryStatus batteryStatus)
        {
            // Arrange
            HeartRate? heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0);

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(battPerCent, heartRate.BatteryLevel, "Battery level");
            Assert.AreEqual(voltage, heartRate.BatteryVoltage, "Battery voltage");
            Assert.AreEqual(batteryStatus, heartRate.BatteryStatus, "Battery status");
        }

        [TestMethod]
        public void Parse_RequestCapabilies_ExpectedBehavior()
        {
            // Arrange
            var heartRate = new HeartRate(new byte[8], 0x33221100);

            // Act
            heartRate.RequestCapabilities();

            // Assert
            byte[] expected = new byte[] { 0x0D, 0x5E, 0, 0x00, 0x11, 0x22, 0x33, (byte)CommonDataPageType.RequestDataPage, 0xFF, 0xFF, 0xFF, 0xFF, 4, (byte)HeartRate.DataPage.Capabilities, (byte)CommandType.DataPage };
            Assert.IsTrue(expected.SequenceEqual(heartRate.Message), "RequestCapabilities expected = {0}, actual = {1}", BitConverter.ToString(expected), BitConverter.ToString(heartRate.Message));
        }

        [TestMethod]
        public void Parse_SetSportMode_ExpectedBehavior()
        {
            // Arrange
            var heartRate = new HeartRate(new byte[8], 0x33221100);

            // Act
            heartRate.SetSportMode(HeartRate.SportMode.Swimming);

            // Assert
            byte[] expected = new byte[] { 0x0D, 0x5E, 0, 0x00, 0x11, 0x22, 0x33, (byte)CommonDataPageType.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)HeartRate.SportMode.Swimming };
            Assert.IsTrue(expected.SequenceEqual(heartRate.Message), "SetSportMode expected = {0}, actual = {1}", BitConverter.ToString(expected), BitConverter.ToString(heartRate.Message));
        }
    }
}
