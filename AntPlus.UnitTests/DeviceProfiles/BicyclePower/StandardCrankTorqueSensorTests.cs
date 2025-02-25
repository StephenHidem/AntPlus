using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.StandardCrankTorqueSensor.PedalPositionPage;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class StandardCrankTorqueSensorTests
    {
        private readonly StandardPowerSensor _standardPowerSensor;
        public StandardCrankTorqueSensorTests()
        {
            byte[] page = [(byte)BicyclePower.DataPage.CrankTorque, 0, 0, 0, 0, 0, 0, 0];
            _standardPowerSensor = BicyclePower.GetBicyclePowerSensor(page, new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<NullLoggerFactory>(), 2000) as StandardPowerSensor;
        }

        [Theory]
        [InlineData(64, 192, 90, 270)]
        [InlineData(0xC0, 0xC0, double.NaN, double.NaN)]
        public void Parse_CyclingDynamics_ExpectedAngle(int start, int end, double startAngle, double endAngle)
        {
            // Arrange
            var crankTorqueSensor = _standardPowerSensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = [(byte)BicyclePower.DataPage.RightForceAngle, 0xFF, (byte)start, (byte)end, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);
            dataPage[0] = (byte)BicyclePower.DataPage.LeftForceAngle;
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(startAngle, crankTorqueSensor.RightForceAngle.StartAngle);
            Assert.Equal(endAngle, crankTorqueSensor.RightForceAngle.EndAngle);
            Assert.Equal(2047.97, crankTorqueSensor.RightForceAngle.AvgTorque, 0.01);
            Assert.Equal(startAngle, crankTorqueSensor.LeftForceAngle.StartAngle);
            Assert.Equal(endAngle, crankTorqueSensor.LeftForceAngle.EndAngle);
            Assert.Equal(2047.97, crankTorqueSensor.LeftForceAngle.AvgTorque, 0.01);
        }

        [Theory]
        [InlineData(64, 192, 90, 270)]
        [InlineData(0xC0, 0xC0, double.NaN, double.NaN)]
        public void Parse_CyclingDynamics_ExpectedPeakAngle(int start, int end, double startAngle, double endAngle)
        {
            // Arrange
            var crankTorqueSensor = _standardPowerSensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = [(byte)BicyclePower.DataPage.RightForceAngle, 0xFF, 0xFF, 0xFF, (byte)start, (byte)end, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);
            dataPage[0] = (byte)BicyclePower.DataPage.LeftForceAngle;
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(startAngle, crankTorqueSensor.RightForceAngle.StartPeakAngle);
            Assert.Equal(endAngle, crankTorqueSensor.RightForceAngle.EndPeakAngle);
            Assert.Equal(startAngle, crankTorqueSensor.LeftForceAngle.StartPeakAngle);
            Assert.Equal(endAngle, crankTorqueSensor.LeftForceAngle.EndPeakAngle);
        }

        [Theory]
        [InlineData(0x00, Position.Seated)]
        [InlineData(0x40, Position.TransitionToSeated)]
        [InlineData(0x80, Position.Standing)]
        [InlineData(0xC0, Position.TransitionToStanding)]
        public void Parse_CyclingDynamicsPedalPosition_ExpectedRiderPosition(int pos, Position expPos)
        {
            // Arrange
            var crankTorqueSensor = _standardPowerSensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = [(byte)BicyclePower.DataPage.PedalPosition, 0xFF, (byte)pos, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expPos, crankTorqueSensor.PedalPosition.RiderPosition);
        }

        [Fact]
        public void Parse_CyclingDynamicsPedalPosition_ExpectedCadencePCO()
        {
            // Arrange
            var crankTorqueSensor = _standardPowerSensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = [(byte)BicyclePower.DataPage.PedalPosition, 0xFF, 0xFF, 128, 64, 0xE0, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(128, crankTorqueSensor.PedalPosition.AverageCadence);
            Assert.Equal(64, crankTorqueSensor.PedalPosition.RightPlatformCenterOffset);
            Assert.Equal(-32, crankTorqueSensor.PedalPosition.LeftPlatformCenterOffset);
        }

        [Theory]
        [InlineData(0, 30.0)]
        [InlineData(128, 94.0)]
        [InlineData(255, 157.5)]
        public void Parse_CyclingDynamicsTorqueBarycenter_Expected(int val, double expBarycenterTorque)
        {
            // Arrange
            var crankTorqueSensor = _standardPowerSensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = [(byte)BicyclePower.DataPage.TorqueBarycenter, (byte)val, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expBarycenterTorque, crankTorqueSensor.TorqueBarycenterAngle);
        }

        [Fact]
        public void Parse_CrankTorqueSensorMessage_ExpectedValues()
        {
            // Arrange
            var crankTorqueSensor = _standardPowerSensor.TorqueSensor as StandardCrankTorqueSensor;

            double expAvgCad = 60;
            double expAvgAngVel = 2 * Math.PI;
            double expAvgTorq = 44.875;
            double expAvgPow = 282;
            byte[] dataPage = [(byte)BicyclePower.DataPage.CrankTorque, 1, 1, 60, 0x00, 0x08, 0x9C, 0x05];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expAvgCad, crankTorqueSensor.AverageCadence, 0.1);
            Assert.Equal(expAvgAngVel, crankTorqueSensor.AverageAngularVelocity, 0.001);
            Assert.Equal(expAvgTorq, crankTorqueSensor.AverageTorque, 0.001);
            Assert.Equal(expAvgPow, crankTorqueSensor.AveragePower, 0.1);
        }
    }
}
