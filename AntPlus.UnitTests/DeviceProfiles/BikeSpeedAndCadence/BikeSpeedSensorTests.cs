using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    [TestClass]
    public class BikeSpeedSensorTests
    {
        private BikeSpeedSensor _sensor;

        [TestInitialize]
        public void Initialize()
        {
            _sensor = new BikeSpeedSensor(new ChannelId(0), null);
            byte[] dataPage = new byte[8];
            _sensor.Parse(dataPage);
        }

        [TestMethod]
        public void Parse_SpeedAndDistance_ExpectedSpeedAndDistance()
        {
            // Arrange
            byte[] dataPage = new byte[8] { 0, 1, 2, 3, 0x00, 0x04, 0x05, 0x00 };

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(_sensor.InstantaneousSpeed == 11);
            Assert.IsTrue(_sensor.AccumulatedDistance == 11);
        }
    }
}
