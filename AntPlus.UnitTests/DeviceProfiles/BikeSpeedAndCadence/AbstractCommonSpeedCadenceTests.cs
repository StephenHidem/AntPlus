using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    internal class AbstractCommonSpeedCadence : CommonSpeedCadence
    {
        /// <inheritdoc/>
        public override int ChannelCount => 8118;

        public AbstractCommonSpeedCadence(ChannelId channelId, IAntChannel antChannel, ILogger logger)
            : base(channelId, antChannel, logger, 2000)
        {
        }

        public override Stream DeviceImageStream => throw new NotImplementedException();

        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
        }
    }

    public class AbstractCommonSpeedCadenceTests
    {
        private readonly AbstractCommonSpeedCadence _sensor;

        public AbstractCommonSpeedCadenceTests()
        {
            _sensor = new(new ChannelId(1), Mock.Of<IAntChannel>(), Mock.Of<ILogger>());
            byte[] dataPage = new byte[8];
            dataPage[0] = 0x80;         // initial page change toggle
            _sensor.Parse(dataPage);
        }

        [Fact]
        public void Parse_CumulativeOperatingTime_Expected()
        {
            // Arrange
            TimeSpan cot = TimeSpan.FromSeconds(10000);
            byte[] dataPage = [1, 0x88, 0x13, 0x00, 4, 5, 6, 7];

            // Act
            _sensor.Parse(dataPage);

            // Assert
            Assert.Equal(cot, _sensor.CumulativeOperatingTime);
        }

        [Fact]
        public void Parse_ManufacturerInfo_Expected()
        {
            // Arrange
            byte manId = 15;
            uint sn = 2147483649;
            byte[] dataPage = [2, manId, 0x00, 0x80, 4, 5, 6, 7];

            // Act
            _sensor.Parse(dataPage);

            // Assert
            Assert.Equal(manId, _sensor.ManufacturerInfo.ManufacturingId);
            Assert.Equal(sn, _sensor.ManufacturerInfo.SerialNumber);
        }

        [Fact]
        public void Parse_ProductInfo_Expected()
        {
            // Arrange
            byte hw = 1, sw = 2, model = 3;
            byte[] dataPage = [3, hw, sw, model, 4, 5, 6, 7];

            // Act
            _sensor.Parse(dataPage);

            // Assert
            Assert.Equal(hw, _sensor.ProductInfo.HardwareVersion);
            Assert.Equal(sw, _sensor.ProductInfo.SoftwareVersion);
            Assert.Equal(model, _sensor.ProductInfo.ModelNumber);
        }

        [Fact]
        public void Parse_BatteryStatus_Expected()
        {
            // Arrange
            double battVolt = 3.5;
            BatteryStatus battStat = BatteryStatus.Ok;
            byte[] dataPage = [4, 0xFF, 128, 0x33, 4, 5, 6, 7];

            // Act
            _sensor.Parse(dataPage);

            // Assert
            Assert.Equal(battVolt, _sensor.BatteryStatus.BatteryVoltage);
            Assert.Equal(battStat, _sensor.BatteryStatus.BatteryStatus);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void Parse_Motion_Expected(int flag, bool expState)
        {
            // Arrange
            byte[] dataPage = [5, (byte)flag, 0xFF, 0xFF, 4, 5, 6, 7];

            // Act
            _sensor.Parse(dataPage);

            // Assert
            Assert.Equal(expState, _sensor.Stopped);
        }
    }
}
