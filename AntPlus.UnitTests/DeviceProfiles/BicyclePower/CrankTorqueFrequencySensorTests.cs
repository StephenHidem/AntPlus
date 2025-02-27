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
        private readonly CrankTorqueFrequencySensor _crankTorqueFrequencySensor;
        private readonly MockRepository _mockRepository;
        private readonly Mock<IAntChannel> _mockAntChannel;
        private readonly Mock<ILogger<CrankTorqueFrequencySensor>> _mockLogger;

        public CrankTorqueFrequencySensorTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _mockAntChannel = _mockRepository.Create<IAntChannel>();
            _mockLogger = _mockRepository.Create<ILogger<CrankTorqueFrequencySensor>>();
            _mockLogger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            _crankTorqueFrequencySensor = new CrankTorqueFrequencySensor(new ChannelId(0), _mockAntChannel.Object, _mockLogger.Object, 2000);
        }

        [Fact]
        public void ParseZeroOffsetPage_ExpectedOffset()
        {
            // Arrange

            // Act
            _crankTorqueFrequencySensor.Parse([0x01, 0x10, 0x01, 0xFF, 0xFF, 0xFF, 0x11, 0x22]);

            // Assert
            Assert.Equal(4386, _crankTorqueFrequencySensor.Offset);
        }

        [Fact]
        public void ParseMainDataPage_ExpectedValues()
        {
            // Arrange
            byte[] dataPage = [0x20, 0x01, 0x00, 0x64, 0x07, 0xD0, 0, 0x01];
            _crankTorqueFrequencySensor.Parse([0x20, 0, 0, 0, 0, 0, 0, 0]);
            _crankTorqueFrequencySensor.CalibrationStatus = CalibrationResponse.InProgress;
            // Act
            _crankTorqueFrequencySensor.Parse(dataPage);
            // Assert
            Assert.Equal(CalibrationResponse.Succeeded, _crankTorqueFrequencySensor.CalibrationStatus);
            Assert.Equal(10.0, _crankTorqueFrequencySensor.Slope);
            Assert.Equal(60.0, _crankTorqueFrequencySensor.Cadence, 0.0005);
            Assert.Equal(0.1, _crankTorqueFrequencySensor.Torque, 0.0005);
            Assert.Equal(0.628, _crankTorqueFrequencySensor.Power, 0.0005);
        }

        [Fact]
        public async Task SaveSlopeToFlash_MessageFormatCorrect()
        {
            // Arrange
            double slope = 34.5;
            _mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(It.IsAny<ChannelId>(), new byte[] { 0x01, 0x10, 0x02, 0xFF, 0xFF, 0xFF, 0x01, 0x59 },
                It.IsAny<uint>()))
                .ReturnsAsync(MessagingReturnCode.Pass);

            // Act
            var result = await _crankTorqueFrequencySensor.SaveSlopeToFlash(slope);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            _mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(9.9)]
        [InlineData(50.1)]
        public async Task SaveSlopeToFlash_SlopeOutOfRangeException(double slope)
        {
            // Arrange
            // Act and Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await _crankTorqueFrequencySensor.SaveSlopeToFlash(slope));
        }

        [Fact]
        public async Task SaveSerialNumberToFlash_MessageFormatCorrect()
        {
            // Arrange
            ushort serialNumber = 12345;
            _mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(It.IsAny<ChannelId>(), new byte[] { 0x01, 0x10, 0x03, 0xFF, 0xFF, 0xFF, 0x30, 0x39 },
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await _crankTorqueFrequencySensor.SaveSerialNumberToFlash(serialNumber);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void ParseUnknownDataPage_LogsWarning()
        {
            // Arrange

            // Act
            _crankTorqueFrequencySensor.Parse([0xFF, 0, 0, 0, 0, 0, 0, 0]);

            // Assert
            _mockLogger.Verify(
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
            byte[] dataPage = [0x20, 0x01, 0x00, 0x64, 0x07, 0xD0, 0, 0x01];
            _crankTorqueFrequencySensor.Parse([0x20, 0, 0, 0, 0, 0, 0, 0]);

            // Act
            _crankTorqueFrequencySensor.Parse(dataPage);
            _crankTorqueFrequencySensor.Parse(dataPage);

            // Assert
            Assert.Equal(60, _crankTorqueFrequencySensor.Cadence);
            Assert.Equal(0.1, _crankTorqueFrequencySensor.Torque);
            Assert.Equal(0.628, _crankTorqueFrequencySensor.Power, 0.001);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x11, 0x22 }, CrankTorqueFrequencySensor.CalEventId)]
        [InlineData(new byte[] { 0x01, 0x10, 0xFF, 0xFF, 0xFF, 0xFF, 0x11, 0x22 }, CrankTorqueFrequencySensor.CtfEventId)]
        [InlineData(new byte[] { 0x01, 0x10, 0xAC, 0xFF, 0xFF, 0xFF, 0x11, 0x22 }, CrankTorqueFrequencySensor.AckCtfIdEventId)]
        [InlineData(new byte[] { 0x02, 0x10, 0xAC, 0xFF, 0xFF, 0xFF, 0x11, 0x22 }, 3000)]
        public void ParseUnknownCTFDefinedId_LogsWarning(byte[] dataPage, int eventId)
        {
            // Arrange

            // Act
            _crankTorqueFrequencySensor.Parse(dataPage);

            // Assert
            _mockLogger.Verify(
                m => m.Log(
                    LogLevel.Warning,
                    It.Is<EventId>(v => v.Id == eventId),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unknown data page")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x10, 0xAC, 0x02, 0xFF, 0xFF, 0xFF, 0xFF }, "Slope saved to flash.")]
        [InlineData(new byte[] { 0x01, 0x10, 0xAC, 0x03, 0xFF, 0xFF, 0xFF, 0xFF }, "Serial number saved to flash.")]
        public void ParseAcknowledgeMessage_LogsInformation(byte[] dataPage, string message)
        {
            // Arrange
            // Act
            _crankTorqueFrequencySensor.Parse(dataPage);
            // Assert
            _mockLogger.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
