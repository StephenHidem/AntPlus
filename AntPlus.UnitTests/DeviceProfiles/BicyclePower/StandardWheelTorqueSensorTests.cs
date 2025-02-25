using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class StandardWheelTorqueSensorTests
    {
        private readonly StandardPowerSensor _standardPowerSensor;

        public StandardWheelTorqueSensorTests()
        {
            byte[] page = [(byte)BicyclePower.DataPage.WheelTorque, 0, 0, 0, 0, 0, 0, 0];
            _standardPowerSensor = BicyclePower.GetBicyclePowerSensor(page, new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<NullLoggerFactory>(), It.IsAny<int>()) as StandardPowerSensor;
        }

        [Fact]
        public void Parse_WheelTorque_ExpectedTorqueValues()
        {
            // Arrange
            var wheelTorqueSensor = _standardPowerSensor.TorqueSensor as StandardWheelTorqueSensor;

            double expAvgSpeed = 15;
            double expAvgPower = 178;
            double expAvgAngVel = 2 * Math.PI / (0x439 / 2048.0);
            double expAvgTorq = 14.9375;
            double expDistance = 2.2;
            byte[] dataPage = [(byte)BicyclePower.DataPage.WheelTorque, 1, 1, 60, 0x39, 0x04, 0xDE, 0x01];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expDistance, wheelTorqueSensor.AccumulatedDistance, 0.001);
            Assert.Equal(expAvgSpeed, wheelTorqueSensor.AverageSpeed, 0.01);
            Assert.Equal(expAvgAngVel, wheelTorqueSensor.AverageAngularVelocity, 0.001);
            Assert.Equal(expAvgTorq, wheelTorqueSensor.AverageTorque, 0.001);
            Assert.Equal(expAvgPower, wheelTorqueSensor.AveragePower, 0.5);
        }
    }
}
