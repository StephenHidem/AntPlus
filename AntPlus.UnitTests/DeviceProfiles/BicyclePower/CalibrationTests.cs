using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;
using System.Threading.Tasks;
using static SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.Calibration;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePower
{
    [TestClass]
    public class CalibrationTests
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

        private Calibration CreateCalibration()
        {
            mockBicycle = CreateBicyclePower();
            return mockBicycle.Calibration;
        }

        [TestMethod]
        [DataRow(0x00, false, AutoZero.Off)]
        [DataRow(0x01, true, AutoZero.Off)]
        [DataRow(0x02, false, AutoZero.On)]
        [DataRow(0x03, true, AutoZero.On)]
        public void Parse_AutoZeroSupport_ExpectedAutoZeroSupport(int config, bool supported, AutoZero status)
        {
            // Arrange
            var calibration = CreateCalibration();
            byte[] dataPage = new byte[8] { 0x01, 0x12, (byte)config, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            mockBicycle?.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(supported, calibration.AutoZeroSupported);
            Assert.AreEqual(status, calibration.AutoZeroStatus);
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
            var calibration = CreateCalibration();

            // Act
            mockBicycle?.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(response, calibration.CalibrationStatus);
            Assert.AreEqual(autoZero, calibration.AutoZeroStatus);
            Assert.IsTrue(calibration.CalibrationData == 0x1122);
        }

        [TestMethod]
        [DataRow(0xBB)]
        [DataRow(0xBD)]
        public void Parse_CustomCalibration_ExpectedCustomParameters(int calID)
        {
            // Arrange
            var calibration = CreateCalibration();
            byte[] dataPage = new byte[8] { 1, (byte)calID, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };

            // Act
            mockBicycle?.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(calibration.CustomCalibrationParameters.SequenceEqual(dataPage.Skip(2).ToArray()));
        }

        [TestMethod]
        [DataRow(0, MeasurementOutputData.DataType.ProgressCountdown)]
        [DataRow(1, MeasurementOutputData.DataType.TimeCountdown)]
        [DataRow(8, MeasurementOutputData.DataType.WholeSensorTorque)]
        [DataRow(42, MeasurementOutputData.DataType.Reserved)]
        [DataRow(254, MeasurementOutputData.DataType.Reserved)]
        public void Parse_MeasurementOutputData_ExpectedDataType(int val, MeasurementOutputData.DataType expDataType)
        {
            // Arrange
            var calibration = CreateCalibration();
            byte[] dataPage = new byte[8] { 3, 0x00, (byte)val, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Act
            mockBicycle?.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expDataType, calibration.Measurements[0].MeasurementType);
        }

        [TestMethod]
        [DataRow(sbyte.MinValue, short.MaxValue)]
        [DataRow(sbyte.MaxValue, short.MaxValue)]
        [DataRow(sbyte.MinValue, short.MinValue)]
        [DataRow(sbyte.MaxValue, short.MinValue)]
        public void Parse_MeasurementOutputData_ExpectedScaledMeasurement(int scale, int meas)
        {
            // Arrange
            var calibration = CreateCalibration();
            byte[] dataPage = new byte[8] { 3, 0x00, 0x00, (byte)scale, 0x00, 0x00, 0x00, 0x00 };
            dataPage[6] = BitConverter.GetBytes((short)meas)[0];
            dataPage[7] = BitConverter.GetBytes((short)meas)[1];
            double expMeas = meas * Math.Pow(2, scale);

            // Act
            mockBicycle?.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expMeas, calibration.Measurements[0].Measurement);
        }

        [TestMethod]
        [DataRow(ushort.MinValue, 0)]
        [DataRow(ushort.MaxValue, 31.99951171875)]
        [DataRow(32768, 16)]
        public void Parse_MeasurementOutputData_ExpectedTimestamp(int timeStamp, double timeSpan)
        {
            // Arrange
            var calibration = CreateCalibration();
            byte[] dataPage = new byte[8] { 3, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            dataPage[4] = BitConverter.GetBytes((ushort)timeStamp)[0];
            dataPage[5] = BitConverter.GetBytes((ushort)timeStamp)[1];

            // Act
            mockBicycle?.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(timeSpan, calibration.Measurements[0].Timestamp);
        }

        [TestMethod]
        public async Task RequestManualCalibration_Request_ExpectedMessage()
        {
            // Arrange
            var calibration = CreateCalibration();
            mockAntChannel?.Setup(ac => ac.SendExtAcknowledgedData(mockChannelId, new byte[] { 0x01, 0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await calibration.RequestManualCalibration();

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            Assert.AreEqual(CalibrationResponse.InProgress, calibration.CalibrationStatus);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        [DataRow(AutoZero.Off)]
        [DataRow(AutoZero.On)]
        [DataRow(AutoZero.NotSupported)]
        public async Task SetAutoZeroConfiguration_StateUnderTest_ExpectedBehavior(AutoZero autoZero)
        {
            // Arrange
            var calibration = CreateCalibration();
            mockAntChannel?.Setup(ac => ac.SendExtAcknowledgedData(mockChannelId, new byte[] { 0x01, 0xAB, (byte)autoZero, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await calibration.SetAutoZeroConfiguration(
                autoZero);

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            Assert.AreEqual(CalibrationResponse.Unknown, calibration.CalibrationStatus);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public async Task RequestCustomParameters_Request_ExpectedMessage()
        {
            // Arrange
            var calibration = CreateCalibration();
            mockAntChannel?.Setup(ac => ac.SendExtAcknowledgedData(mockChannelId, new byte[] { 0x01, 0xBA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await calibration.RequestCustomParameters();

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            Assert.AreEqual(CalibrationResponse.InProgress, calibration.CalibrationStatus);
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public async Task SetCustomParameters_Parameters_ExpectedMessage()
        {
            // Arrange
            var calibration = CreateCalibration();
            byte[] customParameters = new byte[] { 0x01, 0xBC, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 };
            mockAntChannel?.Setup(ac => ac.SendExtAcknowledgedData(mockChannelId, customParameters,
                It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);

            // Act
            var result = await calibration.SetCustomParameters(
                customParameters.Skip(2).ToArray());

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            Assert.AreEqual(CalibrationResponse.InProgress, calibration.CalibrationStatus);
            mockRepository?.VerifyAll();
        }
    }
}
