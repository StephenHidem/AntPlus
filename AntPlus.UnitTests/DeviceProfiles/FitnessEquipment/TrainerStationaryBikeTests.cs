using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.TrainerStationaryBike;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class TrainerStationaryBikeTests
    {
        [TestMethod]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var trainerStationaryBike = new TrainerStationaryBike();
            byte[] dataPage = new byte[] { 25, 0, 128, 0, 0, 0x55, 0x0A, 0 };

            // Act
            trainerStationaryBike.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(trainerStationaryBike.InstantaneousCadence == 128);
            Assert.IsTrue(trainerStationaryBike.InstantaneousPower == 2645);
        }

        [TestMethod]
        public void Parse_AveragePower_RolloverCorrect()
        {
            // Arrange
            var trainerStationaryBike = new TrainerStationaryBike();
            byte[] dataPage = new byte[] { 25, 0, 0, 0xFF, 0xFF, 0, 0, 0 };
            trainerStationaryBike.Parse(
                dataPage);
            dataPage[1] = 1; dataPage[3] = 19; dataPage[4] = 0;

            // Act
            trainerStationaryBike.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(trainerStationaryBike.AveragePower == 20.0);
        }

        [TestMethod]
        [DataRow(new byte[] { 25, 0, 0, 0, 0, 0, 0x00, 0 }, TrainerStatusField.None)]
        [DataRow(new byte[] { 25, 0, 0, 0, 0, 0, 0x10, 0 }, TrainerStatusField.BikePowerCalRequired)]
        [DataRow(new byte[] { 25, 0, 0, 0, 0, 0, 0x20, 0 }, TrainerStatusField.ResistanceCalRequired)]
        [DataRow(new byte[] { 25, 0, 0, 0, 0, 0, 0x40, 0 }, TrainerStatusField.UserConfigRequired)]
        public void Parse_TrainerStatus_Matches(byte[] dataPage, TrainerStatusField trainerStatus)
        {
            // Arrange
            var trainerStationaryBike = new TrainerStationaryBike();

            // Act
            trainerStationaryBike.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(trainerStatus, trainerStationaryBike.TrainerStatus);
        }

        [TestMethod]
        [DataRow(new byte[] { 25, 0, 0, 0, 0, 0, 0, 0x00 }, TargetPowerLimit.AtTargetPower)]
        [DataRow(new byte[] { 25, 0, 0, 0, 0, 0, 0, 0x01 }, TargetPowerLimit.TooLow)]
        [DataRow(new byte[] { 25, 0, 0, 0, 0, 0, 0, 0x02 }, TargetPowerLimit.TooHigh)]
        [DataRow(new byte[] { 25, 0, 0, 0, 0, 0, 0, 0x03 }, TargetPowerLimit.Undetermined)]
        public void Parse_Flags_Matches(byte[] dataPage, TargetPowerLimit targetPowerLimit)
        {
            // Arrange
            var trainerStationaryBike = new TrainerStationaryBike();

            // Act
            trainerStationaryBike.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(targetPowerLimit, trainerStationaryBike.TargetPower);
        }
    }
}
