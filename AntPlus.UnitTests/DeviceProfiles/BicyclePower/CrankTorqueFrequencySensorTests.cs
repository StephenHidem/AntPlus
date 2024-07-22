using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading.Tasks;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    [TestClass]
    public class CrankTorqueFrequencySensorTests
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

        private CrankTorqueFrequencySensor CreateCrankTorqueFrequencySensor()
        {
            byte[] page = new byte[8] { (byte)DataPage.CrankTorqueFrequency, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, missedMessages: 8) as CrankTorqueFrequencySensor;
        }

        [TestMethod]
        public void ParseCalibrationMessage_ZeroOffset_ExpectedOffset()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            byte[] dataPage = new byte[8] { 0x01, 0x10, 0x01, 0xFF, 0xFF, 0xFF, 0x11, 0x22 };

            // Act
            crankTorqueFrequencySensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(4386, crankTorqueFrequencySensor.Offset);
        }

        [TestMethod]
        public void Parse_Cadence_ExpectedCadence()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            byte[] dataPage = new byte[8] { 0x20, 0x01, 0, 0, 0x07, 0xD0, 0, 0x01 };

            // Act
            crankTorqueFrequencySensor.Parse(
                new byte[8] { 0x20, 0, 0, 0, 0, 0, 0, 0 });
            crankTorqueFrequencySensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(60.0, crankTorqueFrequencySensor.Cadence, 0.0005);
        }

        [TestMethod]
        public void Parse_TorqueAndPower_ExpectedTorqueAndPower()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            byte[] dataPage = new byte[8] { 0x20, 0x01, 0x00, 0x64, 0x07, 0xD0, 0, 0x01 };

            // Act
            crankTorqueFrequencySensor.Parse(
                new byte[8] { 0x20, 0, 0, 0, 0, 0, 0, 0 });
            crankTorqueFrequencySensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(0.1, crankTorqueFrequencySensor.Torque, 0.0005);
            Assert.AreEqual(0.628, crankTorqueFrequencySensor.Power, 0.0005);
        }

        [TestMethod]
        public async Task SaveSlopeToFlash_Message_MessageFormatCorrect()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            double slope = 34.5;
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(mockChannelId, new byte[] { 0x01, 0x10, 0x02, 0xFF, 0xFF, 0xFF, 0x01, 0x59 },
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await crankTorqueFrequencySensor.SaveSlopeToFlash(
                slope);

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(9.9)]
        [DataRow(50.1)]
        public async Task SaveSlopeToFlash_Message_SlopeOutOfRangeException(double slope)
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();

            // Act and Assert
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () => await crankTorqueFrequencySensor.SaveSlopeToFlash(slope));
        }

        [TestMethod]
        public async Task SaveSerialNumberToFlash_Message_MessageFormatCorrect()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            ushort serialNumber = 12345;
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(mockChannelId, new byte[] { 0x01, 0x10, 0x03, 0xFF, 0xFF, 0xFF, 0x30, 0x39 },
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await crankTorqueFrequencySensor.SaveSerialNumberToFlash(
                serialNumber);

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }
    }
}
