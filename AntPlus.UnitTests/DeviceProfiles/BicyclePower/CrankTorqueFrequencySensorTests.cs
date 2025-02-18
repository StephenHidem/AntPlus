using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class CrankTorqueFrequencySensorTests
    {
        private readonly MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger> mockLogger;
        private readonly Mock<ILoggerFactory> mockLoggerFactory;

        public CrankTorqueFrequencySensorTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<ILogger>(MockBehavior.Loose);
            mockLoggerFactory = mockRepository.Create<ILoggerFactory>();
            mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
        }

        private CrankTorqueFrequencySensor CreateCrankTorqueFrequencySensor()
        {
            byte[] page = [(byte)BicyclePower.DataPage.CrankTorqueFrequency, 0, 0, 0, 0, 0, 0, 0];
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLoggerFactory.Object, 2000) as CrankTorqueFrequencySensor;
        }

        [Fact]
        public void ParseCalibrationMessage_ZeroOffset_ExpectedOffset()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            byte[] dataPage = [0x01, 0x10, 0x01, 0xFF, 0xFF, 0xFF, 0x11, 0x22];

            // Act
            crankTorqueFrequencySensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(4386, crankTorqueFrequencySensor.Offset);
        }

        [Fact]
        public void Parse_Cadence_ExpectedCadence()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            byte[] dataPage = [0x20, 0x01, 0, 0, 0x07, 0xD0, 0, 0x01];

            // Act
            crankTorqueFrequencySensor.Parse(
                [0x20, 0, 0, 0, 0, 0, 0, 0]);
            crankTorqueFrequencySensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(60.0, crankTorqueFrequencySensor.Cadence, 0.0005);
        }

        [Fact]
        public void Parse_TorqueAndPower_ExpectedTorqueAndPower()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            byte[] dataPage = [0x20, 0x01, 0x00, 0x64, 0x07, 0xD0, 0, 0x01];

            // Act
            crankTorqueFrequencySensor.Parse(
                [0x20, 0, 0, 0, 0, 0, 0, 0]);
            crankTorqueFrequencySensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(0.1, crankTorqueFrequencySensor.Torque, 0.0005);
            Assert.Equal(0.628, crankTorqueFrequencySensor.Power, 0.0005);
        }

        [Fact]
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
            Assert.Equal(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(9.9)]
        [InlineData(50.1)]
        public async Task SaveSlopeToFlash_Message_SlopeOutOfRangeException(double slope)
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await crankTorqueFrequencySensor.SaveSlopeToFlash(slope));
        }

        [Fact]
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
            Assert.Equal(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }

        [Fact]
        public void Parse_UnknownDataPage_LogsWarning()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            byte[] dataPage = [0xFF, 0, 0, 0, 0, 0, 0, 0];

            // Act
            crankTorqueFrequencySensor.Parse(dataPage);

            // Assert
            mockLogger.Verify(
                m => m.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unknown data page")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void ParseCTFMessage_SameUpdateEventCountAndTorqueTicks_NoCalculations()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            byte[] dataPage = [0x20, 0x01, 0x00, 0x64, 0x07, 0xD0, 0, 0x01];

            // Act
            crankTorqueFrequencySensor.Parse(dataPage);
            crankTorqueFrequencySensor.Parse(dataPage);

            // Assert
            Assert.Equal(60, crankTorqueFrequencySensor.Cadence);
            Assert.Equal(0.1, crankTorqueFrequencySensor.Torque);
            Assert.Equal(0.628, crankTorqueFrequencySensor.Power, 0.001);
        }

        [Fact]
        public void ParseCalibrationMessage_UnknownCTFDefinedId_NoAction()
        {
            // Arrange
            var crankTorqueFrequencySensor = CreateCrankTorqueFrequencySensor();
            byte[] dataPage = [0x01, 0x10, 0xFF, 0xFF, 0xFF, 0xFF, 0x11, 0x22];

            // Act
            crankTorqueFrequencySensor.Parse(dataPage);

            // Assert
            // No exception should be thrown
        }
    }
}
