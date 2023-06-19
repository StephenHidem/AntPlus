using Moq;
using SmallEarthTech.AntRadioInterface;
using System;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class FitnessEquipmentTests
    {
        private MockRepository? mockRepository;

        private ChannelId mockChannelId = new(0);
        private Mock<IAntChannel>? mockAntChannel;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
        }

        private SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment CreateFitnessEquipment()
        {
            return new SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment(
                mockChannelId,
                mockAntChannel?.Object);
        }

        [TestMethod]
        [DataRow(new byte[8] { 16, 19, 0, 0, 0, 0, 0, 0 }, FitnessEquipmentType.Treadmill)]
        [DataRow(new byte[8] { 16, 20, 0, 0, 0, 0, 0, 0 }, FitnessEquipmentType.Elliptical)]
        [DataRow(new byte[8] { 16, 22, 0, 0, 0, 0, 0, 0 }, FitnessEquipmentType.Rower)]
        [DataRow(new byte[8] { 16, 23, 0, 0, 0, 0, 0, 0 }, FitnessEquipmentType.Climber)]
        [DataRow(new byte[8] { 16, 24, 0, 0, 0, 0, 0, 0 }, FitnessEquipmentType.NordicSkier)]
        [DataRow(new byte[8] { 16, 25, 0, 0, 0, 0, 0, 0 }, FitnessEquipmentType.TrainerStationaryBike)]
        public void Parse_GeneralDataPage_ExpectedEquipmentCreated(byte[] dataPage, FitnessEquipmentType equipmentType)
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();

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
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 48, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(resistance / 0.5) },
                500)).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetBasicResistance(
                resistance);

            // Assert
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void SetTargetPower_Message_Matches()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double power = 4000;
            byte[] expPow = BitConverter.GetBytes((ushort)(power / 0.25));
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 49, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, expPow[0], expPow[1] },
                500)).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetTargetPower(
                power);

            // Assert
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void SetWindResistance_Message_Matches()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double windResistanceCoefficient = 1.86;
            sbyte windSpeed = 0;
            double draftingFactor = 0.5;
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 50, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(windResistanceCoefficient / 0.01), (byte)(windSpeed + 127), (byte)(draftingFactor / 0.01) },
                500)).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetWindResistance(
                windResistanceCoefficient,
                windSpeed,
                draftingFactor);

            // Assert
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void SetTrackResistance_Message_Matches()
        {
            // Arrange
            var fitnessEquipment = CreateFitnessEquipment();
            double grade = 0;
            double rollingResistanceCoefficient = 0.004;
            byte[] expGrade = BitConverter.GetBytes((ushort)((grade + 200) / 0.01));
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 51, 0xFF, 0xFF, 0xFF, 0xFF, expGrade[0], expGrade[1], (byte)(rollingResistanceCoefficient / 0.00005) },
                500)).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetTrackResistance(
                grade,
                rollingResistanceCoefficient);

            // Assert
            mockRepository.VerifyAll();
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
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedData(
                mockChannelId,
                new byte[8] { 55, expWeight[0], expWeight[1], 0xFF, (byte)((wheelDiameterOffset & 0x0F) | (expBikeWeight[0] & 0xF0)), expBikeWeight[1], (byte)(wheelDiameter / 0.01), (byte)(gearRatio / 0.03) },
                500)).Returns(MessagingReturnCode.Pass);

            // Act
            fitnessEquipment.SetUserConfiguration(
                userWeight,
                wheelDiameterOffset,
                bikeWeight,
                wheelDiameter,
                gearRatio);

            // Assert
            mockRepository.VerifyAll();
        }
    }
}
