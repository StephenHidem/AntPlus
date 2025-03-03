using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    public class TrainerTorqueTests
    {
        private readonly TrainerTorque _trainerTorque;

        public TrainerTorqueTests()
        {
            // zero out class so next event is used
            _trainerTorque = new TrainerTorque();
            _trainerTorque.Parse(new byte[8]);
        }

        [Fact]
        public void Parse_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            byte[] dataPage = [26, 1, 5, 0x11, 0x22, 0x33, 0x44, 0];

            // Act
            _trainerTorque.Parse(dataPage);

            // Assert
            Assert.Equal(1.860, _trainerTorque.AverageSpeed, 0.001);
            Assert.Equal(1.476, _trainerTorque.AverageAngularVelocity, 0.001);
            Assert.Equal(545.594, _trainerTorque.AverageTorque, 0.001);
            Assert.Equal(805.032, _trainerTorque.AveragePower, 0.001);
            Assert.Equal(11, _trainerTorque.AccumulatedDistance);
        }
    }
}
