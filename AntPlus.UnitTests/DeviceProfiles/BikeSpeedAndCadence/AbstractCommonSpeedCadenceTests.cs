using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    internal class AbstractCommonSpeedCadence : CommonSpeedCadence
    {
        public AbstractCommonSpeedCadence(ChannelId channelId, IAntChannel antChannel, ILogger logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
        }

        public override Stream DeviceImageStream => throw new NotImplementedException();

        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
        }
    }

    [TestClass]
    public class AbstractCommonSpeedCadenceTests
    {
        private AbstractCommonSpeedCadence _sensor;
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger> mockLogger;

        [TestInitialize]
        public void Initialize()
        {
            mockAntChannel = new Mock<IAntChannel>();
            mockLogger = new Mock<ILogger>();
            _sensor = new(new ChannelId(1), mockAntChannel.Object, mockLogger.Object);
            byte[] dataPage = new byte[8];
            dataPage[0] = 0x80;         // initial page change toggle
            _sensor.Parse(dataPage);
        }

        [TestMethod]
        public void Parse_CumulativeOperartingTime_Expected()
        {
            // Arrange
            TimeSpan cot = TimeSpan.FromSeconds(10000);
            byte[] dataPage = new byte[8] { 1, 0x88, 0x13, 0x00, 4, 5, 6, 7 };

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(cot, _sensor.CumulativeOperatingTime);
        }

        [TestMethod]
        public void Parse_ManufacturerInfo_Expected()
        {
            // Arrange
            byte manId = 15;
            uint sn = 2147483649;
            byte[] dataPage = new byte[8] { 2, manId, 0x00, 0x80, 4, 5, 6, 7 };

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(manId, _sensor.ManufacturerInfo.ManufacturingId);
            Assert.AreEqual(sn, _sensor.ManufacturerInfo.SerialNumber);
        }

        [TestMethod]
        public void Parse_ProductInfo_Expected()
        {
            // Arrange
            byte hw = 1, sw = 2, model = 3;
            byte[] dataPage = new byte[8] { 3, hw, sw, model, 4, 5, 6, 7 };

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(hw, _sensor.ProductInfo.HardwareVersion);
            Assert.AreEqual(sw, _sensor.ProductInfo.SoftwareVersion);
            Assert.AreEqual(model, _sensor.ProductInfo.ModelNumber);
        }

        [TestMethod]
        public void Parse_BatteryStatus_Expected()
        {
            // Arrange
            double battVolt = 3.5;
            BatteryStatus battStat = BatteryStatus.Ok;
            byte[] dataPage = new byte[8] { 4, 0xFF, 128, 0x33, 4, 5, 6, 7 };

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(battVolt, _sensor.BatteryStatus.BatteryVoltage);
            Assert.AreEqual(battStat, _sensor.BatteryStatus.BatteryStatus);
        }

        [TestMethod]
        [DataRow(0, false)]
        [DataRow(1, true)]
        public void Parse_Motion_Expected(int flag, bool expState)
        {
            // Arrange
            byte[] dataPage = new byte[8] { 5, (byte)flag, 0xFF, 0xFF, 4, 5, 6, 7 };

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expState, _sensor.Stopped);
        }
    }
}
