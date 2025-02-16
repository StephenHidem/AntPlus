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
        private readonly MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<NullLoggerFactory> mockLogger;

        public CalibrationTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<NullLoggerFactory>(MockBehavior.Loose);
        }

        private StandardPowerSensor CreateStandardPowerSensor()
        {
            byte[] page = [(byte)BicyclePower.DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0];
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, 2000) as StandardPowerSensor;
        }

        [Theory]
        [InlineData(0x00, false, AutoZero.Off)]
        [InlineData(0x01, true, AutoZero.Off)]
        [InlineData(0x02, false, AutoZero.On)]
        [InlineData(0x03, true, AutoZero.On)]
        public void Parse_AutoZeroSupport_ExpectedAutoZeroSupport(int config, bool supported, AutoZero status)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [0x01, 0x12, (byte)config, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(supported, sensor.AutoZeroSupported);
            Assert.Equal(status, sensor.AutoZeroStatus);
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
            var sensor = CreateStandardPowerSensor();

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(response, sensor.CalibrationStatus);
            Assert.Equal(autoZero, sensor.AutoZeroStatus);
            Assert.Equal(0x1122, sensor.CalibrationData);
        }

        [Theory]
        [InlineData(0xBB)]
        [InlineData(0xBD)]
        public void Parse_CustomCalibration_ExpectedCustomParameters(int calID)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [1, (byte)calID, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.True(sensor.CustomCalibrationParameters.SequenceEqual(dataPage.Skip(2).ToArray()));
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
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [3, 0x01, (byte)val, 0x00, 0x00, 0x00, 0x00, 0x00];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expDataType, sensor.Measurements[0].MeasurementType);
        }

        [Theory]
        [InlineData(sbyte.MinValue, short.MaxValue)]
        [InlineData(sbyte.MaxValue, short.MaxValue)]
        [InlineData(sbyte.MinValue, short.MinValue)]
        [InlineData(sbyte.MaxValue, short.MinValue)]
        public void Parse_MeasurementOutputData_ExpectedScaledMeasurement(int scale, int measurement)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [3, 0x01, 0x00, (byte)scale, 0x00, 0x00, 0x00, 0x00];
            dataPage[6] = BitConverter.GetBytes((short)measurement)[0];
            dataPage[7] = BitConverter.GetBytes((short)measurement)[1];
            double expMeasurement = measurement * Math.Pow(2, scale);

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(expMeasurement, sensor.Measurements[0].Measurement);
        }

        [Theory]
        [InlineData(ushort.MinValue, 0)]
        [InlineData(ushort.MaxValue, 31.99951171875)]
        [InlineData(32768, 16)]
        public void Parse_MeasurementOutputData_ExpectedTimestamp(int timeStamp, double timeSpan)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = [3, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
            dataPage[4] = BitConverter.GetBytes((ushort)timeStamp)[0];
            dataPage[5] = BitConverter.GetBytes((ushort)timeStamp)[1];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.Equal(timeSpan, sensor.Measurements[0].Timestamp);
        }

        [Fact]
        public async Task RequestManualCalibration_Request_ExpectedMessage()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(mockChannelId, new byte[] { 0x01, 0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await sensor.RequestManualCalibration();

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.Equal(CalibrationResponse.InProgress, sensor.CalibrationStatus);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(AutoZero.Off)]
        [InlineData(AutoZero.On)]
        [InlineData(AutoZero.NotSupported)]
        public async Task SetAutoZeroConfiguration_StateUnderTest_ExpectedBehavior(AutoZero autoZero)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(mockChannelId, new byte[] { 0x01, 0xAB, (byte)autoZero, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await sensor.SetAutoZeroConfiguration(
                autoZero);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.Equal(CalibrationResponse.Unknown, sensor.CalibrationStatus);
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task RequestCustomParameters_Request_ExpectedMessage()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(mockChannelId, new byte[] { 0x01, 0xBA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await sensor.RequestCustomParameters();

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.Equal(CalibrationResponse.InProgress, sensor.CalibrationStatus);
            mockRepository.VerifyAll();
        }

        [Fact]
        public async Task SetCustomParameters_Parameters_ExpectedMessage()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] customParameters = [0x01, 0xBC, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66];
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(mockChannelId, customParameters,
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await sensor.SetCustomParameters(
                customParameters.Skip(2).ToArray());

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.Equal(CalibrationResponse.InProgress, sensor.CalibrationStatus);
            mockRepository.VerifyAll();
        }
    }
}
