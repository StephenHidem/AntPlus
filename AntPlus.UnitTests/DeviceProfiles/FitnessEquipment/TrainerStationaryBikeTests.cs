using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.TrainerStationaryBike;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class TrainerStationaryBikeTests
    {
        TrainerStationaryBike trainer;

        [TestInitialize]
        public void Initialize()
        {
            trainer = new TrainerStationaryBike();
            trainer.Parse(new byte[8]);     // initialize trainer
        }

        [TestMethod]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
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

            // Act
            trainer.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(targetPowerLimit, trainer.TargetPower);
        }
    }
}
