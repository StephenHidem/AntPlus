using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.StandardCrankTorqueSensor.PedalPositionPage;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    [TestClass]
    public class StandardCrankTorqueSensorTests
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

        private StandardPowerSensor CreateStandardCrankTorqueSensor()
        {
            byte[] page = new byte[8] { (byte)BicyclePower.DataPage.CrankTorque, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, null, 8) as StandardPowerSensor;
        }

        [TestMethod]
        [DataRow(64, 192, 90, 270)]
        [DataRow(0xC0, 0xC0, double.NaN, double.NaN)]
        public void Parse_CyclingDynamics_ExpectedAngle(int start, int end, double startAngle, double endAngle)
        {
            // Arrange
            var sensor = CreateStandardCrankTorqueSensor();
            var crankTorqueSensor = sensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = new byte[8] { (byte)BicyclePower.DataPage.RightForceAngle, 0xFF, (byte)start, (byte)end, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);
            dataPage[0] = (byte)BicyclePower.DataPage.LeftForceAngle;
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(startAngle, crankTorqueSensor.RightForceAngle.StartAngle);
            Assert.AreEqual(endAngle, crankTorqueSensor.RightForceAngle.EndAngle);
            Assert.AreEqual(2047.97, crankTorqueSensor.RightForceAngle.AvgTorque, 0.01);
            Assert.AreEqual(startAngle, crankTorqueSensor.LeftForceAngle.StartAngle);
            Assert.AreEqual(endAngle, crankTorqueSensor.LeftForceAngle.EndAngle);
            Assert.AreEqual(2047.97, crankTorqueSensor.LeftForceAngle.AvgTorque, 0.01);
        }

        [TestMethod]
        [DataRow(64, 192, 90, 270)]
        [DataRow(0xC0, 0xC0, double.NaN, double.NaN)]
        public void Parse_CyclingDynamics_ExpectedPeakAngle(int start, int end, double startAngle, double endAngle)
        {
            // Arrange
            var sensor = CreateStandardCrankTorqueSensor();
            var crankTorqueSensor = sensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = new byte[8] { (byte)BicyclePower.DataPage.RightForceAngle, 0xFF, 0xFF, 0xFF, (byte)start, (byte)end, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);
            dataPage[0] = (byte)BicyclePower.DataPage.LeftForceAngle;
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(startAngle, crankTorqueSensor.RightForceAngle.StartPeakAngle);
            Assert.AreEqual(endAngle, crankTorqueSensor.RightForceAngle.EndPeakAngle);
            Assert.AreEqual(startAngle, crankTorqueSensor.LeftForceAngle.StartPeakAngle);
            Assert.AreEqual(endAngle, crankTorqueSensor.LeftForceAngle.EndPeakAngle);
        }

        [TestMethod]
        [DataRow(0x00, Position.Seated)]
        [DataRow(0x40, Position.TransitionToSeated)]
        [DataRow(0x80, Position.Standing)]
        [DataRow(0xC0, Position.TransitionToStanding)]
        public void Parse_CyclingDynamicsPedalPosition_ExpectedRiderPosition(int pos, Position expPos)
        {
            // Arrange
            var sensor = CreateStandardCrankTorqueSensor();
            var crankTorqueSensor = sensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = new byte[8] { (byte)BicyclePower.DataPage.PedalPosition, 0xFF, (byte)pos, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPos, crankTorqueSensor.PedalPosition.RiderPosition);
        }

        [TestMethod]
        public void Parse_CyclingDynamicsPedalPosition_ExpectedCadencePCO()
        {
            // Arrange
            var sensor = CreateStandardCrankTorqueSensor();
            var crankTorqueSensor = sensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = new byte[8] { (byte)BicyclePower.DataPage.PedalPosition, 0xFF, 0xFF, 128, 64, 0xE0, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(128, crankTorqueSensor.PedalPosition.AverageCadence);
            Assert.AreEqual(64, crankTorqueSensor.PedalPosition.RightPlatformCenterOffset);
            Assert.AreEqual(-32, crankTorqueSensor.PedalPosition.LeftPlatformCenterOffset);
        }

        [TestMethod]
        [DataRow(0, 30.0)]
        [DataRow(128, 94.0)]
        [DataRow(255, 157.5)]
        public void Parse_CyclingDynamicsTorqueBarycenter_Expected(int val, double expBarycenterTorque)
        {
            // Arrange
            var sensor = CreateStandardCrankTorqueSensor();
            var crankTorqueSensor = sensor.TorqueSensor as StandardCrankTorqueSensor;
            byte[] dataPage = new byte[8] { (byte)BicyclePower.DataPage.TorqueBarycenter, (byte)val, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expBarycenterTorque, crankTorqueSensor.TorqueBarycenterAngle);
        }

        [TestMethod]
        public void Parse_CrankTorqueSensorMessage_ExpectedValues()
        {
            // Arrange
            var sensor = CreateStandardCrankTorqueSensor();
            var crankTorqueSensor = sensor.TorqueSensor as StandardCrankTorqueSensor;

            double expAvgCad = 60;
            double expAvgAngVel = 2 * Math.PI;
            double expAvgTorq = 44.875;
            double expAvgPow = 282;
            byte[] dataPage = new byte[8] { (byte)BicyclePower.DataPage.CrankTorque, 1, 1, 60, 0x00, 0x08, 0x9C, 0x05 };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expAvgCad, crankTorqueSensor.AverageCadence, 0.1);
            Assert.AreEqual(expAvgAngVel, crankTorqueSensor.AverageAngularVelocity, 0.001);
            Assert.AreEqual(expAvgTorq, crankTorqueSensor.AverageTorque, 0.001);
            Assert.AreEqual(expAvgPow, crankTorqueSensor.AveragePower, 0.1);
        }
    }
}
