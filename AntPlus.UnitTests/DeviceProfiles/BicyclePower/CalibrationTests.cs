using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
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
            return new Calibration(
                mockBicycle,
                mockLogger?.Object);
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
