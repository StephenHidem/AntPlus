using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    [TestClass]
    public class CalibrationTests
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

        private StandardPowerSensor CreateStandardPowerSensor()
        {
            byte[] page = new byte[8] { (byte)BicyclePower.DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, null, 8) as StandardPowerSensor;
        }

        [TestMethod]
        [DataRow(0x00, false, AutoZero.Off)]
        [DataRow(0x01, true, AutoZero.Off)]
        [DataRow(0x02, false, AutoZero.On)]
        [DataRow(0x03, true, AutoZero.On)]
        public void Parse_AutoZeroSupport_ExpectedAutoZeroSupport(int config, bool supported, AutoZero status)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { 0x01, 0x12, (byte)config, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(supported, sensor.AutoZeroSupported);
            Assert.AreEqual(status, sensor.AutoZeroStatus);
        }

        [TestMethod]
        [DataRow(new byte[] { 1, 0xAC, 0x00, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Succeeded, AutoZero.Off)]
        [DataRow(new byte[] { 1, 0xAC, 0x01, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Succeeded, AutoZero.On)]
        [DataRow(new byte[] { 1, 0xAC, 0xFF, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Succeeded, AutoZero.NotSupported)]
        [DataRow(new byte[] { 1, 0xAF, 0x00, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Failed, AutoZero.Off)]
        [DataRow(new byte[] { 1, 0xAF, 0x01, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Failed, AutoZero.On)]
        [DataRow(new byte[] { 1, 0xAF, 0xFF, 0xFF, 0xFF, 0xFF, 0x22, 0x11 }, CalibrationResponse.Failed, AutoZero.NotSupported)]
        public void Parse_GeneralCalibrationResponse_ExpectedResponse(byte[] dataPage, CalibrationResponse response, AutoZero autoZero)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(response, sensor.CalibrationStatus);
            Assert.AreEqual(autoZero, sensor.AutoZeroStatus);
            Assert.IsTrue(sensor.CalibrationData == 0x1122);
        }

        [TestMethod]
        [DataRow(0xBB)]
        [DataRow(0xBD)]
        public void Parse_CustomCalibration_ExpectedCustomParameters(int calID)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { 1, (byte)calID, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(sensor.CustomCalibrationParameters.SequenceEqual(dataPage.Skip(2).ToArray()));
        }

        [TestMethod]
        [DataRow(0, MeasurementType.ProgressCountdown)]
        [DataRow(1, MeasurementType.TimeCountdown)]
        [DataRow(8, MeasurementType.WholeSensorTorque)]
        [DataRow(42, MeasurementType.Reserved)]
        [DataRow(254, MeasurementType.Reserved)]
        public void Parse_MeasurementOutputData_ExpectedDataType(int val, MeasurementType expDataType)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { 3, 0x01, (byte)val, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expDataType, sensor.Measurements[0].MeasurementType);
        }

        [TestMethod]
        [DataRow(sbyte.MinValue, short.MaxValue)]
        [DataRow(sbyte.MaxValue, short.MaxValue)]
        [DataRow(sbyte.MinValue, short.MinValue)]
        [DataRow(sbyte.MaxValue, short.MinValue)]
        public void Parse_MeasurementOutputData_ExpectedScaledMeasurement(int scale, int measurement)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { 3, 0x01, 0x00, (byte)scale, 0x00, 0x00, 0x00, 0x00 };
            dataPage[6] = BitConverter.GetBytes((short)measurement)[0];
            dataPage[7] = BitConverter.GetBytes((short)measurement)[1];
            double expMeasurement = measurement * Math.Pow(2, scale);

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expMeasurement, sensor.Measurements[0].Measurement);
        }

        [TestMethod]
        [DataRow(ushort.MinValue, 0)]
        [DataRow(ushort.MaxValue, 31.99951171875)]
        [DataRow(32768, 16)]
        public void Parse_MeasurementOutputData_ExpectedTimestamp(int timeStamp, double timeSpan)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { 3, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            dataPage[4] = BitConverter.GetBytes((ushort)timeStamp)[0];
            dataPage[5] = BitConverter.GetBytes((ushort)timeStamp)[1];

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(timeSpan, sensor.Measurements[0].Timestamp);
        }

        [TestMethod]
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
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            Assert.AreEqual(CalibrationResponse.InProgress, sensor.CalibrationStatus);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(AutoZero.Off)]
        [DataRow(AutoZero.On)]
        [DataRow(AutoZero.NotSupported)]
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
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            Assert.AreEqual(CalibrationResponse.Unknown, sensor.CalibrationStatus);
            mockRepository.VerifyAll();
        }

        [TestMethod]
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
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            Assert.AreEqual(CalibrationResponse.InProgress, sensor.CalibrationStatus);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task SetCustomParameters_Parameters_ExpectedMessage()
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] customParameters = new byte[] { 0x01, 0xBC, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 };
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(mockChannelId, customParameters,
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await sensor.SetCustomParameters(
                customParameters.Skip(2).ToArray());

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            Assert.AreEqual(CalibrationResponse.InProgress, sensor.CalibrationStatus);
            mockRepository.VerifyAll();
        }
    }
}
