using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class TrainerTorqueTests
    {
        TrainerTorque trainerTorque;

        [TestInitialize]
        public void Initialize()
        {
            // zero out class so next event is used
            trainerTorque = new TrainerTorque();
            trainerTorque.Parse(new byte[8]);
        }

        [TestMethod]
        public void Parse_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            byte[] dataPage = new byte[8] { 26, 1, 5, 0x11, 0x22, 0x33, 0x44, 0 };

            // Act
            trainerTorque.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(trainerTorque.AverageSpeed > 1.859 && trainerTorque.AverageSpeed < 1.860);
            Assert.IsTrue(trainerTorque.AverageAngularVelocity > 1.475 && trainerTorque.AverageAngularVelocity < 1.476);
            Assert.IsTrue(trainerTorque.AverageTorque > 545.593 && trainerTorque.AverageTorque < 545.594);
            Assert.IsTrue(trainerTorque.AveragePower > 805.031 && trainerTorque.AveragePower < 805.032);
            Assert.IsTrue(trainerTorque.AccumulatedDistance == 11);
        }
    }
}
