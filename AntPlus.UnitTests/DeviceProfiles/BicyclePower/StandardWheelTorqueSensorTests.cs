using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePower
{
    [TestClass]
    public class StandardWheelTorqueSensorTests
    {
        private MockRepository? mockRepository;

        private Bicycle? mockBicycle;
        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel>? mockAntChannel;
        private Mock<ILogger<Bicycle>>? mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<ILogger<Bicycle>>(MockBehavior.Loose);
        }

        private Bicycle CreateBicyclePower()
        {
            return new Bicycle(
                mockChannelId,
                mockAntChannel?.Object,
                mockLogger?.Object);
        }

        private StandardWheelTorqueSensor CreateStandardWheelTorqueSensor()
        {
            mockBicycle = CreateBicyclePower();
            mockBicycle.Parse(new byte[8] { (byte)DataPage.WheelTorque, 0, 0, 0, 0, 0, 0, 0 });
            return mockBicycle.WheelTorqueSensor;
        }

        [TestMethod]
        public void Parse_WheelTorque_ExpectedTorqueValues()
        {
            // Arrange
            var standardWheelTorqueSensor = CreateStandardWheelTorqueSensor();
            byte expInstCad = 60;
            double expAvgSpeed = 15;
            double expAvgPower = 178;
            double expAvgAngVel = 2 * Math.PI / (0x439 / 2048.0);
            double expAvgTorq = 14.9375;
            double expDist = 2.2;
            byte[] dataPage = new byte[8] { (byte)DataPage.WheelTorque, 1, 1, 60, 0x39, 0x04, 0xDE, 0x01 };

            // Act
            mockBicycle?.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expDist, standardWheelTorqueSensor.AccumulatedDistance);
            Assert.AreEqual(expAvgSpeed, standardWheelTorqueSensor.AverageSpeed, 0.01);
            Assert.AreEqual(expInstCad, standardWheelTorqueSensor.InstantaneousCadence);
            Assert.AreEqual(expAvgAngVel, standardWheelTorqueSensor.AverageAngularVelocity);
            Assert.AreEqual(expAvgTorq, standardWheelTorqueSensor.AverageTorque);
            Assert.AreEqual(expAvgPower, standardWheelTorqueSensor.AveragePower, 0.5);
            mockRepository.VerifyAll();
        }
    }
}
