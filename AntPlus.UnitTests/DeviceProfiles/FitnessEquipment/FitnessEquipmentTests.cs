using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using System;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class FitnessEquipmentTests
    {
        private MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<NullLoggerFactory> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<NullLoggerFactory>(MockBehavior.Loose);
        }

        private SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment CreateFitnessEquipment(FitnessEquipmentType equipmentType = FitnessEquipmentType.Treadmill)
        {
            byte[] dataPage = new byte[8] { (byte)DataPage.GeneralFEData, (byte)equipmentType, 0, 0, 0, 0, 0, 0 };
            return GetFitnessEquipment(dataPage, mockChannelId, mockAntChannel.Object, mockLogger.Object, missedMessages: 8);
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
        [DataRow(FitnessEquipmentType.Treadmill)]
        [DataRow(FitnessEquipmentType.Elliptical)]
        [DataRow(FitnessEquipmentType.Rower)]
        [DataRow(FitnessEquipmentType.Climber)]
        [DataRow(FitnessEquipmentType.NordicSkier)]
        [DataRow(FitnessEquipmentType.TrainerStationaryBike)]
        public void GetEquipment_GeneralDataPage_ExpectedEquipment(FitnessEquipmentType equipmentType)
        {
            // Arrange and Act
            var fitnessEquipment = CreateFitnessEquipment(equipmentType);

            // Assert
            switch (equipmentType)
            {
                case FitnessEquipmentType.Treadmill:
                    Assert.IsInstanceOfType<Treadmill>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Elliptical:
                    Assert.IsInstanceOfType<Elliptical>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Rower:
                    Assert.IsInstanceOfType<Rower>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Climber:
                    Assert.IsInstanceOfType<Climber>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.NordicSkier:
                    Assert.IsInstanceOfType<NordicSkier>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.TrainerStationaryBike:
                    Assert.IsInstanceOfType<TrainerStationaryBike>(fitnessEquipment);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        [TestMethod]
        [DataRow(DataPage.TreadmillData, FitnessEquipmentType.Treadmill)]
        [DataRow(DataPage.EllipticalData, FitnessEquipmentType.Elliptical)]
        [DataRow(DataPage.RowerData, FitnessEquipmentType.Rower)]
        [DataRow(DataPage.ClimberData, FitnessEquipmentType.Climber)]
        [DataRow(DataPage.NordicSkierData, FitnessEquipmentType.NordicSkier)]
        [DataRow(DataPage.TrainerStationaryBikeData, FitnessEquipmentType.TrainerStationaryBike)]
        [DataRow(DataPage.TrainerTorqueData, FitnessEquipmentType.TrainerStationaryBike)]
        public void GetEquipment_SpecificPage_ExpectedEquipment(DataPage pageNumber, FitnessEquipmentType equipmentType)
        {
            // Arrange
            byte[] dataPage = new byte[8] { (byte)pageNumber, 0, 0, 0, 0, 0, 0, 0 };

            // Act
            var fitnessEquipment = GetFitnessEquipment(dataPage, mockChannelId, mockAntChannel.Object, mockLogger.Object, missedMessages: 8);

            // Assert
            switch (equipmentType)
            {
                case FitnessEquipmentType.Treadmill:
                    Assert.IsInstanceOfType<Treadmill>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Elliptical:
                    Assert.IsInstanceOfType<Elliptical>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Rower:
                    Assert.IsInstanceOfType<Rower>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Climber:
                    Assert.IsInstanceOfType<Climber>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.NordicSkier:
                    Assert.IsInstanceOfType<NordicSkier>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.TrainerStationaryBike:
                    Assert.IsInstanceOfType<TrainerStationaryBike>(fitnessEquipment);
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
        public void Parse_GeneralDataPage_ExpectedCapabilities(int caps, HRDataSource hrSrc, bool expDistanceTravelEn, bool expVirtualSpeedFlag)
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            byte[] dataPage = { 16, 0, 0, 0, 0, 0, 0, (byte)caps };

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(hrSrc, fitnessEquipment.GeneralData.HeartRateSource);
            Assert.AreEqual(expDistanceTravelEn, fitnessEquipment.GeneralData.DistanceTraveledEnabled);
            Assert.AreEqual(expVirtualSpeedFlag, fitnessEquipment.GeneralData.VirtualSpeedFlag);
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
            double metabolicEquivalents = 50.01;
            double calBurnRate = 3000.0;
            byte calories = 128;
            byte[] metabolic = BitConverter.GetBytes((ushort)(metabolicEquivalents / 0.01));
            byte[] cbrVal = BitConverter.GetBytes((ushort)(calBurnRate / 0.1));
            byte[] dataPage = new byte[8] { 18, 0xFF, metabolic[0], metabolic[1], cbrVal[0], cbrVal[1], calories, 0 };

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(fitnessEquipment.GeneralMetabolic.InstantaneousMET == metabolicEquivalents);
            Assert.IsTrue(fitnessEquipment.GeneralMetabolic.CaloricBurnRate == calBurnRate);
            Assert.IsTrue(fitnessEquipment.GeneralMetabolic.AccumulatedCalories == 0);
        }

        [TestMethod]
        [DataRow(SupportedTrainingModes.Simulation)]
        [DataRow(SupportedTrainingModes.BasicResistance)]
        [DataRow(SupportedTrainingModes.TargetPower)]
        public void Parse_FECapabilities_Expected(SupportedTrainingModes trainingModes)
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
            Assert.IsTrue(fitnessEquipment.TrainingModes == trainingModes);
        }
    }
}
