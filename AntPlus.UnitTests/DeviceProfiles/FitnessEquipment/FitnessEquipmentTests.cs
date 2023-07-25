using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using System;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Equipment;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class FitnessEquipmentTests
    {
        private MockRepository? mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel>? mockAntChannel;
        private Mock<ILogger<Equipment>>? mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Equipment>>(MockBehavior.Loose);
        }

        private Equipment CreateFitnessEquipment()
        {
            return new Equipment(
                mockChannelId,
                mockAntChannel?.Object,
                mockLogger?.Object);
        }

        [TestMethod]
        [DataRow(0x00, FEState.Unknown)]
        [DataRow(0x10, FEState.AsleepOrOff)]
        [DataRow(0x20, FEState.Ready)]
        [DataRow(0x30, FEState.InUse)]
        [DataRow(0x40, FEState.FinishedOrPaused)]
        public void Parse_FEState_ExpectedFEState(int state, FEState expState)
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            byte[] dataPage = { 16, 0, 0, 0, 0, 0, 0, (byte)state };

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expState, fitnessEquipment.State);
        }

        [TestMethod]
        [DataRow(19, FitnessEquipmentType.Treadmill)]
        [DataRow(20, FitnessEquipmentType.Elliptical)]
        [DataRow(22, FitnessEquipmentType.Rower)]
        [DataRow(23, FitnessEquipmentType.Climber)]
        [DataRow(24, FitnessEquipmentType.NordicSkier)]
        [DataRow(25, FitnessEquipmentType.TrainerStationaryBike)]
        public void Parse_GeneralDataPage_ExpectedEquipmentCreated(int equip, FitnessEquipmentType equipmentType)
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            byte[] dataPage = { 16, (byte)equip, 0, 0, 0, 0, 0, 0 };

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(equipmentType, fitnessEquipment.GeneralData.EquipmentType);
            switch (equipmentType)
            {
                case FitnessEquipmentType.Treadmill:
                    Assert.IsNotNull(fitnessEquipment.Treadmill);
                    break;
                case FitnessEquipmentType.Elliptical:
                    Assert.IsNotNull(fitnessEquipment.Elliptical);
                    break;
                case FitnessEquipmentType.Rower:
                    Assert.IsNotNull(fitnessEquipment.Rower);
                    break;
                case FitnessEquipmentType.Climber:
                    Assert.IsNotNull(fitnessEquipment.Climber);
                    break;
                case FitnessEquipmentType.NordicSkier:
                    Assert.IsNotNull(fitnessEquipment.NordicSkier);
                    break;
                case FitnessEquipmentType.TrainerStationaryBike:
                    Assert.IsNotNull(fitnessEquipment.TrainerStationaryBike);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        [TestMethod]
        public void Parse_GeneralDataPage_DataValid()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            fitnessEquipment.Parse(new byte[] { 16, 0, 0, 0, 0, 0, 0, 0x34 });
            byte[] dataPage = { 16, 0, 128, 64, 0x00, 0x80, 70, 0x34 };

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(fitnessEquipment.GeneralData.DistanceTraveled == 64);
            Assert.IsTrue(fitnessEquipment.GeneralData.ElapsedTime == TimeSpan.FromSeconds(128 / 4));
            Assert.IsTrue(fitnessEquipment.GeneralData.InstantaneousSpeed == 32.768);
            Assert.IsTrue(fitnessEquipment.GeneralData.InstantaneousHeartRate == 70);
        }

        [TestMethod]
        [DataRow(0x00, HRDataSource.Invalid, false, false)]
        [DataRow(0x01, HRDataSource.HeartRateMonitor, false, false)]
        [DataRow(0x02, HRDataSource.EMHeartRateMonitor, false, false)]
        [DataRow(0x03, HRDataSource.HandContactSensors, false, false)]
        [DataRow(0x04, HRDataSource.Invalid, true, false)]
        [DataRow(0x08, HRDataSource.Invalid, false, true)]
        public void Parse_GeneralDataPage_ExpectedCapabilities(int caps, HRDataSource hrSrc, bool expDistTravelEn, bool expVirtSpeedFlag)
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            byte[] dataPage = { 16, 0, 0, 0, 0, 0, 0, (byte)caps };

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(hrSrc, fitnessEquipment.GeneralData.HeartRateSource);
            Assert.AreEqual(expDistTravelEn, fitnessEquipment.GeneralData.DistanceTraveledEnabled);
            Assert.AreEqual(expVirtSpeedFlag, fitnessEquipment.GeneralData.VirtualSpeedFlag);
        }

        [TestMethod]
        public void Parse_GeneralSettings_Expected()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double cycleLen = 1.28;
            double incline = 50.0;
            double resistance = 25.5;
            byte[] inclineVal = BitConverter.GetBytes((ushort)(incline / 0.01));
            byte[] dataPage = new byte[8] { 17, 0xFF, 0xFF, (byte)(cycleLen / 0.01), inclineVal[0], inclineVal[1], (byte)(resistance / 0.5), 0 };

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(fitnessEquipment.GeneralSettings.CycleLength == cycleLen);
            Assert.IsTrue(fitnessEquipment.GeneralSettings.Incline == incline);
            Assert.IsTrue(fitnessEquipment.GeneralSettings.ResistanceLevel == resistance);
        }

        [TestMethod]
        public void Parse_GeneralMetabolicData_Expected()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double mets = 50.01;
            double calBurnRate = 3000.0;
            byte cals = 128;
            byte[] metsVal = BitConverter.GetBytes((ushort)(mets / 0.01));
            byte[] cbrVal = BitConverter.GetBytes((ushort)(calBurnRate / 0.1));
            byte[] dataPage = new byte[8] { 18, 0xFF, metsVal[0], metsVal[1], cbrVal[0], cbrVal[1], cals, 0 };

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(fitnessEquipment.GeneralMetabolic.InstantaneousMET == mets);
            Assert.IsTrue(fitnessEquipment.GeneralMetabolic.CaloricBurnRate == calBurnRate);
            Assert.IsTrue(fitnessEquipment.GeneralMetabolic.AccumulatedCalories == 0);
        }

        [TestMethod]
        [DataRow(SupportedTrainingModes.Simulation)]
        [DataRow(SupportedTrainingModes.BasicResistance)]
        [DataRow(SupportedTrainingModes.TargetPower)]
        public void Parse_FECapabilites_Expected(SupportedTrainingModes trainingModes)
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            ushort maxResistance = 32768;
            byte[] maxResVal = BitConverter.GetBytes(maxResistance);
            byte[] dataPage = new byte[8] { 54, 0xFF, 0xFF, 0xFF, 0xFF, maxResVal[0], maxResVal[1], (byte)trainingModes };

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(fitnessEquipment.MaxTrainerResistance == maxResistance);
            Assert.IsTrue(fitnessEquipment.Capabilities == trainingModes);
        }

        [TestMethod]
        public void SetBasicResistance_Message_Matches()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double resistance = 50;
            mockAntChannel?.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 48, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(resistance / 0.5) },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetBasicResistance(
                resistance);

            // Assert
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void SetTargetPower_Message_Matches()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double power = 4000;
            byte[] expPow = BitConverter.GetBytes((ushort)(power / 0.25));
            mockAntChannel?.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 49, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, expPow[0], expPow[1] },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetTargetPower(
                power);

            // Assert
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void SetWindResistance_Message_Matches()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double windResistanceCoefficient = 1.86;
            sbyte windSpeed = 0;
            double draftingFactor = 0.5;
            mockAntChannel?.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 50, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(windResistanceCoefficient / 0.01), (byte)(windSpeed + 127), (byte)(draftingFactor / 0.01) },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetWindResistance(
                windResistanceCoefficient,
                windSpeed,
                draftingFactor);

            // Assert
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void SetTrackResistance_Message_Matches()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double grade = 0;
            double rollingResistanceCoefficient = 0.004;
            byte[] expGrade = BitConverter.GetBytes((ushort)((grade + 200) / 0.01));
            mockAntChannel?.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 51, 0xFF, 0xFF, 0xFF, 0xFF, expGrade[0], expGrade[1], (byte)(rollingResistanceCoefficient / 0.00005) },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetTrackResistance(
                grade,
                rollingResistanceCoefficient);

            // Assert
            mockRepository?.VerifyAll();
        }

        [TestMethod]
        public void SetUserConfiguration_Message_Matches()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double userWeight = 355;
            byte wheelDiameterOffset = 0;
            double bikeWeight = 24;
            double wheelDiameter = 1;
            double gearRatio = 3.2;

            byte[] expWeight = BitConverter.GetBytes((ushort)(userWeight / 0.01));
            byte[] expBikeWeight = BitConverter.GetBytes((ushort)(bikeWeight / 0.05) << 4);
            mockAntChannel?.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 55, expWeight[0], expWeight[1], 0xFF, (byte)((wheelDiameterOffset & 0x0F) | (expBikeWeight[0] & 0xF0)), expBikeWeight[1], (byte)(wheelDiameter / 0.01), (byte)(gearRatio / 0.03) },
                500).Result).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetUserConfiguration(
                userWeight,
                wheelDiameterOffset,
                bikeWeight,
                wheelDiameter,
                gearRatio);

            // Assert
            mockRepository?.VerifyAll();
        }
    }
}
