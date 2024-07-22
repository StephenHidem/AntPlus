using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    [TestClass]
    public class StandardWheelTorqueSensorTests
    {
        private MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<NullLoggerFactory> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<NullLoggerFactory>(MockBehavior.Loose);
        }

        private StandardPowerSensor CreateStandardWheelTorqueSensor()
        {
            byte[] page = new byte[8] { (byte)DataPage.WheelTorque, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, missedMessages: 8) as StandardPowerSensor;
        }

        [TestMethod]
        public void Parse_WheelTorque_ExpectedTorqueValues()
        {
            // Arrange
            var sensor = CreateStandardWheelTorqueSensor();
            var wheelTorqueSensor = sensor.TorqueSensor as StandardWheelTorqueSensor;

            double expAvgSpeed = 15;
            double expAvgPower = 178;
            double expAvgAngVel = 2 * Math.PI / (0x439 / 2048.0);
            double expAvgTorq = 14.9375;
            double expDistance = 2.2;
            byte[] dataPage = new byte[8] { (byte)DataPage.WheelTorque, 1, 1, 60, 0x39, 0x04, 0xDE, 0x01 };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expDistance, wheelTorqueSensor.AccumulatedDistance, 0.001);
            Assert.AreEqual(expAvgSpeed, wheelTorqueSensor.AverageSpeed, 0.01);
            Assert.AreEqual(expAvgAngVel, wheelTorqueSensor.AverageAngularVelocity, 0.001);
            Assert.AreEqual(expAvgTorq, wheelTorqueSensor.AverageTorque, 0.001);
            Assert.AreEqual(expAvgPower, wheelTorqueSensor.AveragePower, 0.5);
            mockRepository.VerifyAll();
        }
    }
}
