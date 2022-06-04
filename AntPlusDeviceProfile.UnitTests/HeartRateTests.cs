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
            int et = 0, ec = 0;
            byte hr = 0;
            var heartRate = new HeartRate(new byte[8], 0);
            heartRate.HeartRateChanged += (sender, e) =>
            {
                et = e.AccumulatedHeartBeatEventTime;
                ec = e.AccumulatedHeartBeatCount;
                hr = e.ComputedHeartRate;
            };

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(eventTime, et, "AccumulatedHeartBeatEventTime");
            Assert.AreEqual(hrBeatCount, ec, "AccumulatedHeartBeatCount");
            Assert.AreEqual(computedHr, hr, "ComputedHeartRate");
        }

        [TestMethod]
        public void Parse_AccumulatedValueRollover_ExpectedBehavior()
        {
            // Arrange
            int et = 0, ec = 0;
            var heartRate = new HeartRate(new byte[] { (byte)HeartRate.DataPage.Default | 0x80, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x40 }, 0);
            heartRate.HeartRateChanged += (sender, e) =>
            {
                et = e.AccumulatedHeartBeatEventTime;
                ec = e.AccumulatedHeartBeatCount;
            };

            // Act
            heartRate.Parse(
                new byte[] { (byte)HeartRate.DataPage.Default, 0xFF, 0x02, 0x00, 0x02, 0x00, 0x01, 0x46 });

            // Assert
            Assert.AreEqual(3, et, "AccumulatedHeartBeatEventTime");
            Assert.AreEqual(2, ec, "AccumulatedHeartBeatCount");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.CumulativeOperatingTime, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00 }, 0)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.CumulativeOperatingTime, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF }, 33554430)]
        public void Parse_CumulativeOperatingTime_ExpectedBehavior(byte[] payload, int cumulativeOpTime)
        {
            // Arrange
            TimeSpan cot = TimeSpan.FromSeconds(0);
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0);
            heartRate.CumulativeOperatingTimePageChanged += (sender, args) => cot = args;

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(cumulativeOpTime, cot.TotalSeconds, "CumulativeOperatingTime");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.ManufacturerInfo, 0x80, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00 }, (byte)128, (uint)21930)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.ManufacturerInfo, 0x08, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF }, (byte)8, 4294923690)]
        public void Parse_ManufacturerInformation_ExpectedBehavior(byte[] payload, byte manId, uint sn)
        {
            // Arrange
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0xF00055AA);
            byte mid = 0;
            uint serNum = 0;
            heartRate.ManufacturerInfoPageChanged += (sender, args) =>
            {
                mid = args.ManufacturingIdLsb;
                serNum = args.SerialNumber;
            };

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(manId, mid, "ManufacturingIdLsb");
            Assert.AreEqual(sn, serNum, "SerialNumber");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.ProductInfo, 0x11, 0x22, 0x33, 0, 0, 0, 0 }, (byte)17, (byte)34, (byte)51)]
        public void Parse_ProductInformation_ExpectedBehavior(byte[] payload, byte hwVersion, byte swVersion, byte modelNumber)
        {
            // Arrange
            byte hv = 0, sv = 0, mn = 0;
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0xF00055AA);
            heartRate.ProductInfoPageChanged += (sender, args) =>
            {
                hv = args.HardwareVersion;
                sv = args.SoftwareVersion;
                mn = args.ModelNumber;
            };

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(hwVersion, hv, "HardwareVersion");
            Assert.AreEqual(swVersion, sv, "SoftwareVersion");
            Assert.AreEqual(modelNumber, mn, "ModelNumber");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.PreviousHeartBeat, 0xFF, 0x88, 0x06, 0xDD, 0x07, 0x83, 0xB4 }, 333)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.PreviousHeartBeat, 0xFF, 0x88, 0xFE, 0x15, 0x00, 0x83, 0xB4 }, 387)]
        public void Parse_PreviousHeartBeat_ExpectedBehavior(byte[] payload, int rrInterval)
        {
            // Arrange
            int rr = 0;
            var heartRate = new HeartRate(new byte[] { (byte)HeartRate.DataPage.Default | 0x80, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF, 0x00 }, 0);
            heartRate.PreviousHeartBeatPageChanged += (sender, eventArgs) => rr = eventArgs.RRInterval;

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(rrInterval, rr, "RRInterval");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.SwimInterval, 0x11, 0x22, 0x33, 0, 0, 0, 0 }, (byte)17, (byte)34, (byte)51)]
        public void Parse_SwimInterval_ExpectedBehavior(byte[] payload, byte intervalAvgHr, byte intervalMaxHr, byte sessionAvgHr)
        {
            // Arrange
            byte iahr = 0, imhr = 0, sahr = 0;
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0xF00055AA);
            heartRate.SwimIntervalPageChanged += (sender, args) =>
            {
                iahr = args.IntervalAverageHeartRate;
                imhr = args.IntervalMaximumHeartRate;
                sahr = args.SessionAverageHeartRate;
            };

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(intervalAvgHr, iahr, "IntervalAverageHeartRate");
            Assert.AreEqual(intervalMaxHr, imhr, "IntervalMaximumHeartRate");
            Assert.AreEqual(sessionAvgHr, sahr, "SessionAverageHeartRate");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Capabilities, 0xFF, 0x07, 0x01, 0, 0, 0, 0 }, HeartRate.Features.All, HeartRate.Features.Running)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Capabilities, 0xFF, 0x07, 0x02, 0, 0, 0, 0 }, HeartRate.Features.All, HeartRate.Features.Cycling)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Capabilities, 0xFF, 0x07, 0x04, 0, 0, 0, 0 }, HeartRate.Features.All, HeartRate.Features.Swimming)]
        public void Parse_Capabilities_ExpectedBehavior(byte[] payload, HeartRate.Features supportedFeatures, HeartRate.Features enabledFeatures)
        {
            // Arrange
            HeartRate.Features enabled = HeartRate.Features.None, supported = HeartRate.Features.None;
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0xF00055AA);
            heartRate.CapabilitiesPageChanged += (sender, args) =>
            {
                enabled = args.Enabled;
                supported = args.Supported;
            };

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(supportedFeatures, supported, "Supported features");
            Assert.AreEqual(enabledFeatures, enabled, "Enabled features");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.BatteryStatus, 50, 0xFF, 0x3F, 0x00, 0x00, 0xFF, 0x0F }, 50, 15.99609375, BatteryStatus.Ok)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.BatteryStatus, 0, 0x00, 0x50, 0x00, 0x00, 0xFF, 0x0F }, 0, 0.0, BatteryStatus.Critical)]
        public void Parse_BatteryStatus_ExpectedBehavior(byte[] payload, int battPerCent, double voltage, BatteryStatus batteryStatus)
        {
            // Arrange
            byte bl = 0;
            double bv = 0;
            BatteryStatus bs = BatteryStatus.Unknown;
            var heartRate = new HeartRate(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 }, 0);
            heartRate.BatteryStatusPageChanged += (sender, e) =>
            {
                bl = e.BatteryLevel;
                bv = e.BatteryVoltage;
                bs = e.BatteryStatus;
            };

            // Act
            heartRate.Parse(
                payload);

            // Assert
            Assert.AreEqual(battPerCent, bl, "Battery level");
            Assert.AreEqual(voltage, bv, "Battery voltage");
            Assert.AreEqual(batteryStatus, bs, "Battery status");
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
