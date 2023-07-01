using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;

namespace DeviceProfile.UnitTests
{
    [TestClass]
    public class HeartRateTests
    {
        private readonly ChannelId hrmCid = new(0x01782211);

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Default, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF, 0x00 }, 0, 0, 0)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Default, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF }, 0, 0, 255)]
        public void Parse_CommonHRData_ExpectedBehavior(byte[] dataPage, int eventTime, int hrBeatCount, int computedHr)
        {
            // Arrange
            int et = 0, rr = 0;
            byte chr = 0;
            var heartRate = new HeartRate(hrmCid, null);
            heartRate.PropertyChanged += (sender, e) =>
            {
                et = heartRate.HeartRateData.AccumulatedHeartBeatEventTime;
                rr = heartRate.HeartRateData.RRInterval;
                chr = heartRate.HeartRateData.ComputedHeartRate;
            };

            // Act
            heartRate.Parse(dataPage);

            // Assert
            Assert.AreEqual(eventTime, et, "AccumulatedHeartBeatEventTime");
            Assert.AreEqual(hrBeatCount, rr, "RRInterval");
            Assert.AreEqual(computedHr, chr, "ComputedHeartRate");
        }

        [TestMethod]
        public void Parse_AccumulatedValueRollover_ExpectedBehavior()
        {
            // Arrange
            int et = 0, rr = 0;
            byte chr = 0;
            var heartRate = new HeartRate(hrmCid, null);
            heartRate.PropertyChanged += (sender, e) =>
            {
                et = heartRate.HeartRateData.AccumulatedHeartBeatEventTime;
                rr = heartRate.HeartRateData.RRInterval;
                chr = heartRate.HeartRateData.ComputedHeartRate;
            };

            // Act
            heartRate.Parse(new byte[] { (byte)HeartRate.DataPage.Default | 0x80, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x40 });
            heartRate.Parse(new byte[] { (byte)HeartRate.DataPage.Default, 0xFF, 0x02, 0x00, 0x03, 0x00, 0x00, 0x46 });

            // Assert
            Assert.AreEqual(3, et, "AccumulatedHeartBeatEventTime");
            Assert.AreEqual(3, rr, "RRInterval");
            Assert.AreEqual(70, chr, "Computed HR");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.CumulativeOperatingTime, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00 }, 0)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.CumulativeOperatingTime, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0xFF }, 33554430)]
        public void Parse_CumulativeOperatingTime_ExpectedBehavior(byte[] dataPage, int cumulativeOpTime)
        {
            // Arrange
            TimeSpan cot = TimeSpan.FromSeconds(0);
            var heartRate = new HeartRate(hrmCid, null);
            heartRate.PropertyChanged += (sender, e) =>
            {
                cot = heartRate.CumulativeOperatingTime;
            };
            heartRate.Parse(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 });

            // Act
            heartRate.Parse(dataPage);

            // Assert
            Assert.AreEqual(cumulativeOpTime, cot.TotalSeconds, "CumulativeOperatingTime");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.ManufacturerInfo, 0x80, 0x00, 0x00, 0x11, 0x22, 0x33, 0x44 }, (byte)128, (uint)8721)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.ManufacturerInfo, 0x08, 0xFF, 0xFF, 0x11, 0x22, 0x33, 0x44 }, (byte)8, 4294910481)]
        public void Parse_ManufacturerInformation_ExpectedBehavior(byte[] dataPage, byte manId, uint sn)
        {
            // Arrange
            var heartRate = new HeartRate(hrmCid, null);
            byte mid = 0;
            uint serNum = 0;
            heartRate.PropertyChanged += (sender, e) =>
            {
                mid = heartRate.ManufacturerInfo.ManufacturingIdLsb;
                serNum = heartRate.ManufacturerInfo.SerialNumber;
            };
            heartRate.Parse(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 });

            // Act
            heartRate.Parse(dataPage);

            // Assert
            Assert.AreEqual(manId, mid, "ManufacturingIdLsb");
            Assert.AreEqual(sn, serNum, "SerialNumber");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.ProductInfo, 0x11, 0x22, 0x33, 0, 0, 0, 0 }, (byte)17, (byte)34, (byte)51)]
        public void Parse_ProductInformation_ExpectedBehavior(byte[] dataPage, byte hwVersion, byte swVersion, byte modelNumber)
        {
            // Arrange
            byte hv = 0, sv = 0, mn = 0;
            var heartRate = new HeartRate(hrmCid, null);
            heartRate.PropertyChanged += (sender, e) =>
            {
                hv = heartRate.ProductInfo.HardwareVersion;
                sv = heartRate.ProductInfo.SoftwareVersion;
                mn = heartRate.ProductInfo.ModelNumber;
            };
            heartRate.Parse(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 });

            // Act
            heartRate.Parse(dataPage);

            // Assert
            Assert.AreEqual(hwVersion, hv, "HardwareVersion");
            Assert.AreEqual(swVersion, sv, "SoftwareVersion");
            Assert.AreEqual(modelNumber, mn, "ModelNumber");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.PreviousHeartBeat, 0xFF, 0x88, 0x06, 0xDD, 0x07, 0x83, 0xB4 }, 333)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.PreviousHeartBeat, 0xFF, 0x88, 0xFE, 0x15, 0x00, 0x83, 0xB4 }, 387)]
        public void Parse_PreviousHeartBeat_ExpectedBehavior(byte[] dataPage, int rrInterval)
        {
            // Arrange
            int rr = 0;
            var heartRate = new HeartRate(hrmCid, null);
            heartRate.PropertyChanged += (sender, e) =>
            {
                rr = heartRate.PreviousHeartBeat.RRInterval;
            };
            heartRate.Parse(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 });

            // Act
            heartRate.Parse(dataPage);

            // Assert
            Assert.AreEqual(rrInterval, rr, "RRInterval");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.SwimInterval, 0x11, 0x22, 0x33, 0, 0, 0, 0 }, (byte)17, (byte)34, (byte)51)]
        public void Parse_SwimInterval_ExpectedBehavior(byte[] dataPage, byte intervalAvgHr, byte intervalMaxHr, byte sessionAvgHr)
        {
            // Arrange
            byte iahr = 0, imhr = 0, sahr = 0;
            var heartRate = new HeartRate(hrmCid, null);
            heartRate.PropertyChanged += (sender, e) =>
            {
                iahr = heartRate.SwimInterval.IntervalAverageHeartRate;
                imhr = heartRate.SwimInterval.IntervalMaximumHeartRate;
                sahr = heartRate.SwimInterval.SessionAverageHeartRate;
            };
            heartRate.Parse(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 });

            // Act
            heartRate.Parse(dataPage);

            // Assert
            Assert.AreEqual(intervalAvgHr, iahr, "IntervalAverageHeartRate");
            Assert.AreEqual(intervalMaxHr, imhr, "IntervalMaximumHeartRate");
            Assert.AreEqual(sessionAvgHr, sahr, "SessionAverageHeartRate");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Capabilities, 0xFF, 0xCF, 0x01, 0, 0, 0, 0 }, HeartRate.Features.All, HeartRate.Features.Running)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Capabilities, 0xFF, 0xCF, 0x02, 0, 0, 0, 0 }, HeartRate.Features.All, HeartRate.Features.Cycling)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.Capabilities, 0xFF, 0xCF, 0x04, 0, 0, 0, 0 }, HeartRate.Features.All, HeartRate.Features.Swimming)]
        public void Parse_Capabilities_ExpectedBehavior(byte[] dataPage, HeartRate.Features supportedFeatures, HeartRate.Features enabledFeatures)
        {
            // Arrange
            HeartRate.Features enabled = HeartRate.Features.Generic, supported = HeartRate.Features.Generic;
            var heartRate = new HeartRate(hrmCid, null);
            heartRate.PropertyChanged += (sender, e) =>
            {
                enabled = heartRate.Capabilities.Enabled;
                supported = heartRate.Capabilities.Supported;
            };
            heartRate.Parse(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 });

            // Act
            heartRate.Parse(dataPage);

            // Assert
            Assert.AreEqual(supportedFeatures, supported, "Supported features");
            Assert.AreEqual(enabledFeatures, enabled, "Enabled features");
        }

        [TestMethod]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.BatteryStatus, 50, 0xFF, 0x3F, 0x00, 0x00, 0xFF, 0x0F }, 50, 15.99609375, BatteryStatus.Ok)]
        [DataRow(new byte[] { (byte)HeartRate.DataPage.BatteryStatus, 0, 0x00, 0x50, 0x00, 0x00, 0xFF, 0x0F }, 0, 0.0, BatteryStatus.Critical)]
        public void Parse_BatteryStatus_ExpectedBehavior(byte[] dataPage, int battPerCent, double voltage, BatteryStatus batteryStatus)
        {
            // Arrange
            byte bl = 0;
            double bv = 0;
            BatteryStatus bs = BatteryStatus.Unknown;
            var heartRate = new HeartRate(hrmCid, null);
            heartRate.PropertyChanged += (sender, e) =>
            {
                bl = heartRate.BatteryStatus.BatteryLevel;
                bv = heartRate.BatteryStatus.BatteryVoltage;
                bs = heartRate.BatteryStatus.BatteryStatus;
            };
            heartRate.Parse(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 });

            // Act
            heartRate.Parse(dataPage);

            // Assert
            Assert.AreEqual(battPerCent, bl, "Battery level");
            Assert.AreEqual(voltage, bv, "Battery voltage");
            Assert.AreEqual(batteryStatus, bs, "Battery status");
        }

        [TestMethod]
        public void Parse_ManufacturerDataPage_ExpectedBehavior()
        {
            // Arrange
            byte page = 0;
            byte[] data = { 0, 0, 0 };
            var heartRate = new HeartRate(hrmCid, null);
            heartRate.PropertyChanged += (sender, e) =>
            {
                page = heartRate.ManufacturerSpecific.Page;
                data = heartRate.ManufacturerSpecific.Data;
            };
            heartRate.Parse(new byte[] { 0x80, 0, 0, 0, 0, 0, 0, 0 });

            // Act
            heartRate.Parse(new byte[] { 112, 0x11, 0x22, 0x33, 0, 0, 0, 0 });

            // Assert
            Assert.AreEqual(112, page, "Page");
            Assert.IsTrue(data.SequenceEqual(new byte[] { 0x11, 0x22, 0x33 }, null), "Data");
        }

        //[TestMethod]
        //public void Parse_RequestCapabilies_ExpectedBehavior()
        //{
        //    // Arrange
        //    var heartRate = new HeartRate(hrmCid);

        //    // Act
        //    heartRate.RequestCapabilities();

        //    // Assert
        //    byte[] expected = new byte[] { 0x0D, 0x5E, 0, 0x11, 0x22, 0x78, 0x01, (byte)CommonDataPageType.RequestDataPage, 0xFF, 0xFF, 0xFF, 0xFF, 4, (byte)HeartRate.DataPage.Capabilities, (byte)CommandType.DataPage };
        //    Assert.IsTrue(expected.SequenceEqual(heartRate.Message), "RequestCapabilities expected = {0}, actual = {1}", BitConverter.ToString(expected), BitConverter.ToString(heartRate.Message));
        //}

        //[TestMethod]
        //public void Parse_SetSportMode_ExpectedBehavior()
        //{
        //    // Arrange
        //    var heartRate = new HeartRate(hrmCid);

        //    // Act
        //    heartRate.SetSportMode(HeartRate.SportMode.Swimming);

        //    // Assert
        //    byte[] expected = new byte[] { 0x0D, 0x5E, 0, 0x11, 0x22, 0x78, 0x01, (byte)CommonDataPageType.ModeSettingsPage, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)HeartRate.SportMode.Swimming };
        //    Assert.IsTrue(expected.SequenceEqual(heartRate.Message), "SetSportMode expected = {0}, actual = {1}", BitConverter.ToString(expected), BitConverter.ToString(heartRate.Message));
        //}
    }
}
