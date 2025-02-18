using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using System;
using Xunit;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    public class FitnessEquipmentTests
    {
        private readonly MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<NullLoggerFactory> mockLogger;

        public FitnessEquipmentTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<NullLoggerFactory>(MockBehavior.Loose);
        }

        private SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment CreateFitnessEquipment(FitnessEquipmentType equipmentType = FitnessEquipmentType.Treadmill)
        {
            byte[] dataPage = [(byte)DataPage.GeneralFEData, (byte)equipmentType, 0, 0, 0, 0, 0, 0];
            return GetFitnessEquipment(dataPage, mockChannelId, mockAntChannel.Object, mockLogger.Object, 2000);
        }

        [Theory]
        [InlineData(0x00, FEState.Unknown)]
        [InlineData(0x10, FEState.AsleepOrOff)]
        [InlineData(0x20, FEState.Ready)]
        [InlineData(0x30, FEState.InUse)]
        [InlineData(0x40, FEState.FinishedOrPaused)]
        public void Parse_FEState_ExpectedFEState(int state, FEState expState)
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            byte[] dataPage = [16, 0, 0, 0, 0, 0, 0, (byte)state];

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.Equal(expState, fitnessEquipment.State);
        }

        [Theory]
        [InlineData(FitnessEquipmentType.Treadmill)]
        [InlineData(FitnessEquipmentType.Elliptical)]
        [InlineData(FitnessEquipmentType.Rower)]
        [InlineData(FitnessEquipmentType.Climber)]
        [InlineData(FitnessEquipmentType.NordicSkier)]
        [InlineData(FitnessEquipmentType.TrainerStationaryBike)]
        public void GetEquipment_GeneralDataPage_ExpectedEquipment(FitnessEquipmentType equipmentType)
        {
            // Arrange and Act
            var fitnessEquipment = CreateFitnessEquipment(equipmentType);

            // Assert
            switch (equipmentType)
            {
                case FitnessEquipmentType.Treadmill:
                    Assert.IsType<Treadmill>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Elliptical:
                    Assert.IsType<Elliptical>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Rower:
                    Assert.IsType<Rower>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Climber:
                    Assert.IsType<Climber>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.NordicSkier:
                    Assert.IsType<NordicSkier>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.TrainerStationaryBike:
                    Assert.IsType<TrainerStationaryBike>(fitnessEquipment);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        [Theory]
        [InlineData(DataPage.TreadmillData, FitnessEquipmentType.Treadmill)]
        [InlineData(DataPage.EllipticalData, FitnessEquipmentType.Elliptical)]
        [InlineData(DataPage.RowerData, FitnessEquipmentType.Rower)]
        [InlineData(DataPage.ClimberData, FitnessEquipmentType.Climber)]
        [InlineData(DataPage.NordicSkierData, FitnessEquipmentType.NordicSkier)]
        [InlineData(DataPage.TrainerStationaryBikeData, FitnessEquipmentType.TrainerStationaryBike)]
        [InlineData(DataPage.TrainerTorqueData, FitnessEquipmentType.TrainerStationaryBike)]
        public void GetEquipment_SpecificPage_ExpectedEquipment(DataPage pageNumber, FitnessEquipmentType equipmentType)
        {
            // Arrange
            byte[] dataPage = [(byte)pageNumber, 0, 0, 0, 0, 0, 0, 0];

            // Act
            var fitnessEquipment = GetFitnessEquipment(dataPage, mockChannelId, mockAntChannel.Object, mockLogger.Object, 2000);

            // Assert
            switch (equipmentType)
            {
                case FitnessEquipmentType.Treadmill:
                    Assert.IsType<Treadmill>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Elliptical:
                    Assert.IsType<Elliptical>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Rower:
                    Assert.IsType<Rower>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.Climber:
                    Assert.IsType<Climber>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.NordicSkier:
                    Assert.IsType<NordicSkier>(fitnessEquipment);
                    break;
                case FitnessEquipmentType.TrainerStationaryBike:
                    Assert.IsType<TrainerStationaryBike>(fitnessEquipment);
                    break;
                default:
                    Assert.Fail();
                    break;
            }
        }

        [Fact]
        public void Parse_GeneralDataPage_DataValid()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            fitnessEquipment.Parse([16, 0, 0, 0, 0, 0, 0, 0x34]);
            byte[] dataPage = [16, 0, 128, 64, 0x00, 0x80, 70, 0x34];

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.Equal(64, fitnessEquipment.GeneralData.DistanceTraveled);
            Assert.True(fitnessEquipment.GeneralData.ElapsedTime == TimeSpan.FromSeconds(128 / 4));
            Assert.Equal(32.768, fitnessEquipment.GeneralData.InstantaneousSpeed);
            Assert.Equal(70, fitnessEquipment.GeneralData.InstantaneousHeartRate);
        }

        [Theory]
        [InlineData(0x00, HRDataSource.Invalid, false, false)]
        [InlineData(0x01, HRDataSource.HeartRateMonitor, false, false)]
        [InlineData(0x02, HRDataSource.EMHeartRateMonitor, false, false)]
        [InlineData(0x03, HRDataSource.HandContactSensors, false, false)]
        [InlineData(0x04, HRDataSource.Invalid, true, false)]
        [InlineData(0x08, HRDataSource.Invalid, false, true)]
        public void Parse_GeneralDataPage_ExpectedCapabilities(int caps, HRDataSource hrSrc, bool expDistanceTravelEn, bool expVirtualSpeedFlag)
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            byte[] dataPage = [16, 0, 0, 0, 0, 0, 0, (byte)caps];

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.Equal(hrSrc, fitnessEquipment.GeneralData.HeartRateSource);
            Assert.Equal(expDistanceTravelEn, fitnessEquipment.GeneralData.DistanceTraveledEnabled);
            Assert.Equal(expVirtualSpeedFlag, fitnessEquipment.GeneralData.VirtualSpeedFlag);
        }

        [Fact]
        public void Parse_GeneralSettings_Expected()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double cycleLen = 1.28;
            double incline = 50.0;
            double resistance = 25.5;
            byte[] inclineVal = BitConverter.GetBytes((ushort)(incline / 0.01));
            byte[] dataPage = [17, 0xFF, 0xFF, (byte)(cycleLen / 0.01), inclineVal[0], inclineVal[1], (byte)(resistance / 0.5), 0];

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.Equal(cycleLen, fitnessEquipment.GeneralSettings.CycleLength);
            Assert.Equal(incline, fitnessEquipment.GeneralSettings.Incline);
            Assert.Equal(resistance, fitnessEquipment.GeneralSettings.ResistanceLevel);
        }

        [Fact]
        public void Parse_GeneralMetabolicData_Expected()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double metabolicEquivalents = 50.01;
            double calBurnRate = 3000.0;
            byte calories = 128;
            byte[] metabolic = BitConverter.GetBytes((ushort)(metabolicEquivalents / 0.01));
            byte[] cbrVal = BitConverter.GetBytes((ushort)(calBurnRate / 0.1));
            byte[] dataPage = [18, 0xFF, metabolic[0], metabolic[1], cbrVal[0], cbrVal[1], calories, 0];

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.Equal(metabolicEquivalents, fitnessEquipment.GeneralMetabolic.InstantaneousMET);
            Assert.Equal(calBurnRate, fitnessEquipment.GeneralMetabolic.CaloricBurnRate);
            Assert.Equal(0, fitnessEquipment.GeneralMetabolic.AccumulatedCalories);
        }

        [Theory]
        [InlineData(SupportedTrainingModes.Simulation)]
        [InlineData(SupportedTrainingModes.BasicResistance)]
        [InlineData(SupportedTrainingModes.TargetPower)]
        public void Parse_FECapabilities_Expected(SupportedTrainingModes trainingModes)
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            ushort maxResistance = 32768;
            byte[] maxResVal = BitConverter.GetBytes(maxResistance);
            byte[] dataPage = [54, 0xFF, 0xFF, 0xFF, 0xFF, maxResVal[0], maxResVal[1], (byte)trainingModes];

            // Act
            fitnessEquipment.Parse(
                dataPage);

            // Assert
            Assert.Equal(fitnessEquipment.MaxTrainerResistance, maxResistance);
            Assert.Equal(fitnessEquipment.TrainingModes, trainingModes);
        }
    }
}
