using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    public class CalibrationTests
    {
        private readonly ChannelId _channelId = new(0);
        private readonly Mock<IAntChannel> _mockAntChannel;
        private readonly Mock<NullLoggerFactory> _mockLogger;
        private readonly StandardPowerSensor _standardPowerSensor;

        public CalibrationTests()
        {
            _mockAntChannel = new Mock<IAntChannel> { CallBase = true };
            _mockLogger = new Mock<NullLoggerFactory> { CallBase = true };
            byte[] page = [(byte)BicyclePower.DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0];
            _standardPowerSensor = BicyclePower.GetBicyclePowerSensor(page, _channelId, _mockAntChannel.Object, _mockLogger.Object, 2000) as StandardPowerSensor;
        }

        [Theory]
        [InlineData(0x00, false, AutoZero.Off)]
        [InlineData(0x01, true, AutoZero.Off)]
        [InlineData(0x02, false, AutoZero.On)]
        [InlineData(0x03, true, AutoZero.On)]
        public void Parse_AutoZeroSupport_ExpectedAutoZeroSupport(int config, bool supported, AutoZero status)
        {
            // Arrange
            byte[] dataPage = [0x01, 0x12, (byte)config, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(supported, _standardPowerSensor.AutoZeroSupported);
            Assert.Equal(status, _standardPowerSensor.AutoZeroStatus);
        }

        [Theory]
        [InlineData(new byte[] { 1, 0xAC, 0x00, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Succeeded, AutoZero.Off)]
        [InlineData(new byte[] { 1, 0xAC, 0x01, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Succeeded, AutoZero.On)]
        [InlineData(new byte[] { 1, 0xAC, 0xFF, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Succeeded, AutoZero.NotSupported)]
        [InlineData(new byte[] { 1, 0xAF, 0x00, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Failed, AutoZero.Off)]
        [InlineData(new byte[] { 1, 0xAF, 0x01, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Failed, AutoZero.On)]
        [InlineData(new byte[] { 1, 0xAF, 0xFF, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Failed, AutoZero.NotSupported)]
        public void Parse_GeneralCalibrationResponse_ExpectedResponse(byte[] dataPage, CalibrationResponse response, AutoZero autoZero)
        {
            // Arrange

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(response, _standardPowerSensor.CalibrationStatus);
            Assert.Equal(autoZero, _standardPowerSensor.AutoZeroStatus);
            Assert.Equal(0x1122, _standardPowerSensor.CalibrationData);
        }

        [Theory]
        [InlineData(0xBB)]
        [InlineData(0xBD)]
        public void Parse_CustomCalibration_ExpectedCustomParameters(int calID)
        {
            // Arrange
            byte[] dataPage = [1, (byte)calID, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.True(_standardPowerSensor.CustomCalibrationParameters.SequenceEqual([.. dataPage.Skip(2)]));
        }

        [Theory]
        [InlineData(0, MeasurementType.ProgressCountdown)]
        [InlineData(1, MeasurementType.TimeCountdown)]
        [InlineData(8, MeasurementType.WholeSensorTorque)]
        [InlineData(42, MeasurementType.Reserved)]
        [InlineData(254, MeasurementType.Reserved)]
        public void Parse_MeasurementOutputData_ExpectedDataType(int val, MeasurementType expDataType)
        {
            // Arrange
            byte[] dataPage = [3, 0x01, (byte)val, 0x00, 0x00, 0x00, 0x00, 0x00];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expDataType, _standardPowerSensor.Measurements[0].MeasurementType);
        }

        [Theory]
        [InlineData(sbyte.MinValue, short.MaxValue)]
        [InlineData(sbyte.MaxValue, short.MaxValue)]
        [InlineData(sbyte.MinValue, short.MinValue)]
        [InlineData(sbyte.MaxValue, short.MinValue)]
        public void Parse_MeasurementOutputData_ExpectedScaledMeasurement(int scale, int measurement)
        {
            // Arrange
            byte[] dataPage = [3, 0x01, 0x00, (byte)scale, 0x00, 0x00, 0x00, 0x00];
            dataPage[6] = BitConverter.GetBytes((short)measurement)[0];
            dataPage[7] = BitConverter.GetBytes((short)measurement)[1];
            double expMeasurement = measurement * Math.Pow(2, scale);

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(expMeasurement, _standardPowerSensor.Measurements[0].Measurement);
        }

        [Theory]
        [InlineData(ushort.MinValue, 0)]
        [InlineData(ushort.MaxValue, 31.99951171875)]
        [InlineData(32768, 16)]
        public void Parse_MeasurementOutputData_ExpectedTimestamp(int timeStamp, double timeSpan)
        {
            // Arrange
            byte[] dataPage = [3, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            dataPage[4] = BitConverter.GetBytes((ushort)timeStamp)[0];
            dataPage[5] = BitConverter.GetBytes((ushort)timeStamp)[1];

            // Act
            _standardPowerSensor.Parse(dataPage);

            // Assert
            Assert.Equal(timeSpan, _standardPowerSensor.Measurements[0].Timestamp);
        }

        [Fact]
        public async Task RequestManualCalibration_Request_ExpectedMessage()
        {
            // Arrange
            _mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(_channelId, new byte[] { 0x01, 0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                It.IsAny<uint>()))
                .ReturnsAsync(MessagingReturnCode.Pass);

            // Act
            var result = await _standardPowerSensor.RequestManualCalibration();

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.Equal(CalibrationResponse.InProgress, _standardPowerSensor.CalibrationStatus);
        }

        [Theory]
        [InlineData(AutoZero.Off)]
        [InlineData(AutoZero.On)]
        [InlineData(AutoZero.NotSupported)]
        public async Task SetAutoZeroConfiguration_StateUnderTest_ExpectedBehavior(AutoZero autoZero)
        {
            // Arrange
            _mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(_channelId, new byte[] { 0x01, 0xAB, (byte)autoZero, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                It.IsAny<uint>()))
                .ReturnsAsync(MessagingReturnCode.Pass);

            // Act
            var result = await _standardPowerSensor.SetAutoZeroConfiguration(autoZero);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.Equal(CalibrationResponse.Unknown, _standardPowerSensor.CalibrationStatus);
        }

        [Fact]
        public async Task RequestCustomParameters_Request_ExpectedMessage()
        {
            // Arrange
            _mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(_channelId, new byte[] { 0x01, 0xBA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                It.IsAny<uint>()))
                .ReturnsAsync(MessagingReturnCode.Pass);

            // Act
            var result = await _standardPowerSensor.RequestCustomParameters();

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.Equal(CalibrationResponse.InProgress, _standardPowerSensor.CalibrationStatus);
        }

        [Fact]
        public async Task SetCustomParameters_Parameters_ExpectedMessage()
        {
            // Arrange
            byte[] customParameters = [0x01, 0xBC, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66];
            _mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(_channelId, customParameters,
                It.IsAny<uint>()))
                .ReturnsAsync(MessagingReturnCode.Pass);

            // Act
            var result = await _standardPowerSensor.SetCustomParameters([.. customParameters.Skip(2)]);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.Equal(CalibrationResponse.InProgress, _standardPowerSensor.CalibrationStatus);
        }
    }
}
