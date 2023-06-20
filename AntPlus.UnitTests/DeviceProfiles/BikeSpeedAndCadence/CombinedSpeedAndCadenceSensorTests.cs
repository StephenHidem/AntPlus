using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    [TestClass]
    public class CombinedSpeedAndCadenceSensorTests
    {
        private CombinedSpeedAndCadenceSensor _sensor;

        [TestInitialize]
        public void Initialize()
        {
            _sensor = new CombinedSpeedAndCadenceSensor(new ChannelId(0), null);
            byte[] dataPage = new byte[8];
            _sensor.Parse(dataPage);
        }

        [TestMethod]
        public void Parse_SpeedCadenceDistance_ExpectedSpeedCadenceDistance()
        {
            // Arrange
            byte[] dataPage = new byte[8] { 0x00, 0x04, 0x02, 0x00, 0x00, 0x04, 0x05, 0x00 };

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(_sensor.InstantaneousCadence == 120);
            Assert.IsTrue(_sensor.InstantaneousSpeed == 11);
            Assert.IsTrue(_sensor.AccumulatedDistance == 11);
        }
    }
}
