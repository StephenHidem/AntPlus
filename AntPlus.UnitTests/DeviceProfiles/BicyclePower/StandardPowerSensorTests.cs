using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.StandardPowerSensor;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePower
{
    [TestClass]
    public class StandardPowerSensorTests
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

        private StandardPowerSensor CreateStandardPowerSensor()
        {
            mockBicycle = CreateBicyclePower();
            mockBicycle.Parse(new byte[8] { (byte)DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0 });
            return mockBicycle.PowerSensor;
        }

        [TestMethod]
        [DataRow(0x32, 50, PedalDifferentiation.Unknown)]
        [DataRow(0xB2, 50, PedalDifferentiation.RightPedal)]
        [DataRow(0xFF, 0x7F, PedalDifferentiation.Unused)]
        public void Parse_PedalPower_ExpectedPedalPower(int value, int percent, PedalDifferentiation pedalDifferentiation)
        {
            // Arrange
            var standardPowerSensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { 16, 1, (byte)value, 0, 0, 0, 0, 0 };

            // Act
            standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(percent, standardPowerSensor.PedalPower);
            Assert.AreEqual(pedalDifferentiation, standardPowerSensor.PedalContribution);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Parse_InstantaneousCadence_ExpectedCadence()
        {
            // Arrange
            var standardPowerSensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { 16, 1, 0, 128, 0, 0, 0, 0 };

            // Act
            standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(128, standardPowerSensor.InstantaneousCadence);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Parse_InstantaneousPower_ExpectedPower()
        {
            // Arrange
            var standardPowerSensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { 16, 1, 0, 0, 0, 0, 0x11, 0x22 };

            // Act
            standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(8721, standardPowerSensor.InstantaneousPower);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Parse_AveragePower_ExpectedAvgPower()
        {
            // Arrange
            var standardPowerSensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { 16, 1, 0, 0, 0x11, 0x22, 0, 0 };

            // Act
            standardPowerSensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(8721, standardPowerSensor.AveragePower);
            mockRepository.VerifyAll();
        }
    }
}
