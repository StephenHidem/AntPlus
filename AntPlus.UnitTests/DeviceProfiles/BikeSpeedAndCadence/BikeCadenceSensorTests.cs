using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BikeSpeedAndCadence
{
    [TestClass]
    public class BikeCadenceSensorTests
    {
        private BikeCadenceSensor _sensor;

        [TestInitialize]
        public void Initialize()
        {
            _sensor = new BikeCadenceSensor(new ChannelId(0), null);
            byte[] dataPage = new byte[8];
            _sensor.Parse(dataPage);
        }

        [TestMethod]
        public void Parse_Cadence_ExpectedCadence()
        {
            // Arrange
            byte[] dataPage = new byte[8] { 0, 1, 2, 3, 0x00, 0x04, 0x02, 0x00 };

            // Act
            _sensor.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(_sensor.InstantaneousCadence == 120);
        }
    }
}
