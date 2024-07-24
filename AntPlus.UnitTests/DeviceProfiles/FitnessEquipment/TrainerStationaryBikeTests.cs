using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading.Tasks;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.TrainerStationaryBike;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class TrainerStationaryBikeTests
    {
        private MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment>> mockLogger;


        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment>>(MockBehavior.Loose);
        }

        private TrainerStationaryBike CreateTrainer()
        {
            return new TrainerStationaryBike(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null, 8);
        }

        [TestMethod]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var trainer = CreateTrainer();
            byte[] dataPage = new byte[] { 25, 1, 128, 0, 0, 0x55, 0x0A, 0 };

            // Act
            trainer.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(trainer.InstantaneousCadence == 128);
            Assert.IsTrue(trainer.InstantaneousPower == 2645);
        }

        [TestMethod]
        public void Parse_AveragePower_RolloverCorrect()
        {
            // Arrange
            var trainer = CreateTrainer();
            byte[] dataPage = new byte[] { 25, 1, 0, 0xFF, 0xFF, 0, 0, 0 };
            trainer.Parse(
                dataPage);
            dataPage[1] = 2; dataPage[3] = 19; dataPage[4] = 0;

            // Act
            trainer.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(trainer.AveragePower == 20.0);
        }

        [TestMethod]
        [DataRow(new byte[] { 25, 1, 0, 0, 0, 0, 0x00, 0 }, TrainerStatusField.None)]
        [DataRow(new byte[] { 25, 1, 0, 0, 0, 0, 0x10, 0 }, TrainerStatusField.BikePowerCalRequired)]
        [DataRow(new byte[] { 25, 1, 0, 0, 0, 0, 0x20, 0 }, TrainerStatusField.ResistanceCalRequired)]
        [DataRow(new byte[] { 25, 1, 0, 0, 0, 0, 0x40, 0 }, TrainerStatusField.UserConfigRequired)]
        public void Parse_TrainerStatus_Matches(byte[] dataPage, TrainerStatusField trainerStatus)
        {
            // Arrange
            var trainer = CreateTrainer();

            // Act
            trainer.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(trainerStatus, trainer.TrainerStatus);
        }

        [TestMethod]
        [DataRow(new byte[] { 25, 1, 0, 0, 0, 0, 0, 0x00 }, TargetPowerLimit.AtTargetPower)]
        [DataRow(new byte[] { 25, 1, 0, 0, 0, 0, 0, 0x01 }, TargetPowerLimit.TooLow)]
        [DataRow(new byte[] { 25, 1, 0, 0, 0, 0, 0, 0x02 }, TargetPowerLimit.TooHigh)]
        [DataRow(new byte[] { 25, 1, 0, 0, 0, 0, 0, 0x03 }, TargetPowerLimit.Undetermined)]
        public void Parse_Flags_Matches(byte[] dataPage, TargetPowerLimit targetPowerLimit)
        {
            // Arrange
            var trainer = CreateTrainer();

            // Act
            trainer.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(targetPowerLimit, trainer.TargetPower);
        }

        [TestMethod]
        [DataRow(CalibrationRequestResponse.SpinDown)]
        [DataRow(CalibrationRequestResponse.ZeroOffset)]
        [DataRow(CalibrationRequestResponse.SpinDown | CalibrationRequestResponse.ZeroOffset)]
        public async Task Parse_CalibrationResponse_Success(CalibrationRequestResponse request)
        {
            // Arrange
            var trainer = CreateTrainer();
            byte[] dataPage = new byte[] { 0x01, (byte)request, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(
                mockChannelId,
                dataPage,
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            var result = await trainer.CalibrationRequest(request);
            trainer.Parse(dataPage);

            // Assert
            Assert.IsTrue(trainer.CalibrationStatus.HasFlag(CalibrationResult.Success));
            Assert.IsFalse(trainer.CalibrationStatus.HasFlag(CalibrationResult.TemperatureValid));
            Assert.IsFalse(trainer.CalibrationStatus.HasFlag(CalibrationResult.ZeroOffsetValid));
            Assert.IsFalse(trainer.CalibrationStatus.HasFlag(CalibrationResult.SpinDownValid));
        }

        [TestMethod]
        [DataRow(-25.0, 0)]
        [DataRow(0.00, 50)]
        [DataRow(101.5, 253)]
        public void Parse_CalibrationResponse_Temperature(double expected, int data)
        {
            // Arrange
            var trainer = CreateTrainer();
            byte[] dataPage = new byte[] { 0x01, 0xC0, 0x00, (byte)data, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            trainer.Parse(dataPage);

            // Assert
            Assert.AreEqual(expected, trainer.Temperature);
            Assert.IsTrue(trainer.CalibrationStatus.HasFlag(CalibrationResult.TemperatureValid));
            Assert.IsFalse(trainer.CalibrationStatus.HasFlag(CalibrationResult.ZeroOffsetValid));
            Assert.IsFalse(trainer.CalibrationStatus.HasFlag(CalibrationResult.SpinDownValid));
        }

        [TestMethod]
        public void Parse_CalibrationResponse_ZeroOffset()
        {
            // Arrange
            var trainer = CreateTrainer();
            byte[] dataPage = new byte[] { 0x01, 0xC0, 0x00, 0xFF, 0x00, 0x80, 0xFF, 0xFF };

            // Act
            trainer.Parse(dataPage);

            // Assert
            Assert.AreEqual(32768, trainer.ZeroOffset);
            Assert.IsFalse(trainer.CalibrationStatus.HasFlag(CalibrationResult.TemperatureValid));
            Assert.IsTrue(trainer.CalibrationStatus.HasFlag(CalibrationResult.ZeroOffsetValid));
            Assert.IsFalse(trainer.CalibrationStatus.HasFlag(CalibrationResult.SpinDownValid));
        }

        [TestMethod]
        public void Parse_CalibrationResponse_SpinDownTime()
        {
            // Arrange
            var trainer = CreateTrainer();
            byte[] dataPage = new byte[] { 0x01, 0xC0, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x80 };

            // Act
            trainer.Parse(dataPage);

            // Assert
            Assert.AreEqual(32768, trainer.SpinDownTime);
            Assert.IsFalse(trainer.CalibrationStatus.HasFlag(CalibrationResult.TemperatureValid));
            Assert.IsFalse(trainer.CalibrationStatus.HasFlag(CalibrationResult.ZeroOffsetValid));
            Assert.IsTrue(trainer.CalibrationStatus.HasFlag(CalibrationResult.SpinDownValid));
        }

        [TestMethod]
        [DataRow(TemperatureCondition.NotApplicable, SpeedCondition.NotApplicable)]
        [DataRow(TemperatureCondition.Low, SpeedCondition.NotApplicable)]
        [DataRow(TemperatureCondition.Ok, SpeedCondition.NotApplicable)]
        [DataRow(TemperatureCondition.High, SpeedCondition.NotApplicable)]
        [DataRow(TemperatureCondition.NotApplicable, SpeedCondition.Low)]
        [DataRow(TemperatureCondition.NotApplicable, SpeedCondition.Ok)]
        public void Parse_CalibrationProgress_Conditions(TemperatureCondition tc, SpeedCondition sc)
        {
            // Arrange
            var trainer = CreateTrainer();
            byte conditions = (byte)((int)tc | (int)sc);
            byte[] dataPage = new byte[] { 0x02, 0xC0, conditions, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            trainer.Parse(dataPage);

            // Assert
            Assert.AreEqual(tc, trainer.TemperatureStatus);
            Assert.AreEqual(sc, trainer.SpeedStatus);
        }

        [TestMethod]
        [DataRow(-25.0, 0)]
        [DataRow(0.00, 50)]
        [DataRow(101.5, 253)]
        public void Parse_CalibrationProgress_CurrentTemperature(double expected, int data)
        {
            // Arrange
            var trainer = CreateTrainer();
            byte[] dataPage = new byte[] { 0x02, 0xC0, 0x00, (byte)data, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            trainer.Parse(dataPage);

            // Assert
            Assert.AreEqual(expected, trainer.CurrentTemperature);
        }

        [TestMethod]
        [DataRow(0.001, 1)]
        [DataRow(65.534, 65534)]
        public void Parse_CalibrationProgress_TargetSpeed(double expected, int data)
        {
            // Arrange
            var trainer = CreateTrainer();
            byte[] val = BitConverter.GetBytes(data);
            byte[] dataPage = new byte[] { 0x02, 0xC0, 0x00, 0xFF, val[0], val[1], 0xFF, 0xFF };

            // Act
            trainer.Parse(dataPage);

            // Assert
            Assert.AreEqual(expected, trainer.TargetSpeed);
        }

        [TestMethod]
        public void Parse_CalibrationProgress_TargetSpinDownTime()
        {
            // Arrange
            var trainer = CreateTrainer();
            byte[] dataPage = new byte[] { 0x02, 0xC0, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x80 };

            // Act
            trainer.Parse(dataPage);

            // Assert
            Assert.AreEqual(32768, trainer.TargetSpinDownTime);
        }

        [TestMethod]
        public async Task SetBasicResistance_Message_Matches()
        {
            // Arrange
            var trainer = CreateTrainer();
            double resistance = 50;
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(
                mockChannelId,
                new byte[8] { 48, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(resistance / 0.5) },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            var result = await trainer.SetBasicResistance(
                resistance);

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task SetTargetPower_Message_Matches()
        {
            // Arrange
            var trainer = CreateTrainer();
            double power = 4000;
            byte[] expPow = BitConverter.GetBytes((ushort)(power / 0.25));
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(
                mockChannelId,
                new byte[8] { 49, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, expPow[0], expPow[1] },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            var result = await trainer.SetTargetPower(
                power);

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task SetWindResistance_Message_Matches()
        {
            // Arrange
            var trainer = CreateTrainer();
            double windResistanceCoefficient = 1.86;
            sbyte windSpeed = 0;
            double draftingFactor = 0.5;
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(
                mockChannelId,
                new byte[8] { 50, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(windResistanceCoefficient / 0.01), (byte)(windSpeed + 127), (byte)(draftingFactor / 0.01) },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            var result = await trainer.SetWindResistance(
                windResistanceCoefficient,
                windSpeed,
                draftingFactor);

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task SetTrackResistance_Message_Matches()
        {
            // Arrange
            var trainer = CreateTrainer();
            double grade = 0;
            double rollingResistanceCoefficient = 0.004;
            byte[] expGrade = BitConverter.GetBytes((ushort)((grade + 200) / 0.01));
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(
                mockChannelId,
                new byte[8] { 51, 0xFF, 0xFF, 0xFF, 0xFF, expGrade[0], expGrade[1], (byte)(rollingResistanceCoefficient / 0.00005) },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            var result = await trainer.SetTrackResistance(
                grade,
                rollingResistanceCoefficient);

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task SetUserConfiguration_Message_Matches()
        {
            // Arrange
            var trainer = CreateTrainer();
            double userWeight = 355;
            byte wheelDiameterOffset = 0;
            double bikeWeight = 24;
            double wheelDiameter = 1;
            double gearRatio = 3.2;

            byte[] expWeight = BitConverter.GetBytes((ushort)(userWeight / 0.01));
            byte[] expBikeWeight = BitConverter.GetBytes((ushort)(bikeWeight / 0.05) << 4);
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(
                mockChannelId,
                new byte[8] { 55, expWeight[0], expWeight[1], 0xFF, (byte)((wheelDiameterOffset & 0x0F) | (expBikeWeight[0] & 0xF0)), expBikeWeight[1], (byte)(wheelDiameter / 0.01), (byte)(gearRatio / 0.03) },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            var result = await trainer.SetUserConfiguration(
                userWeight,
                wheelDiameterOffset,
                bikeWeight,
                wheelDiameter,
                gearRatio);

            // Assert
            Assert.AreEqual(MessagingReturnCode.Pass, result);
            mockRepository.VerifyAll();
        }
    }
}
