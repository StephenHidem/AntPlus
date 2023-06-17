using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Treadmill;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class TreadmillTests
    {
        [TestMethod]
        public void Parse_InstantaneousCadence_Matches()
        {
            // Arrange
            var treadmill = new Treadmill();
            byte[] dataPage = new byte[] { 19, 0, 0, 0, 128, 0, 0, 0 };

            // Act
            treadmill.Parse(dataPage);

            // Assert
            Assert.IsTrue(treadmill.Cadence == 128);
        }

        [TestMethod]
        public void Parse_PositiveVerticalDistance_Updated()
        {
            // Arrange
            var treadmill = new Treadmill();
            byte[] dataPage = new byte[] { 19, 0, 0, 0, 0, 0, 255, 0 };
            treadmill.Parse(dataPage);
            dataPage[6] = 19;

            // Act
            treadmill.Parse(dataPage);

            // Assert
            Assert.IsTrue(treadmill.PosVerticalDistance == 2.0);
        }

        [TestMethod]
        public void Parse_NegativeVerticalDistance_Updated()
        {
            // Arrange
            var treadmill = new Treadmill();
            byte[] dataPage = new byte[] { 19, 0, 0, 0, 0, 255, 0, 0 };
            treadmill.Parse(dataPage);
            dataPage[5] = 19;

            // Act
            treadmill.Parse(dataPage);

            // Assert
            Assert.IsTrue(treadmill.NegVerticalDistance == -2.0);
        }

        [TestMethod]
        [DataRow(new byte[] { 19, 0, 0, 0, 0, 0, 0, 0xFD }, CapabilityFlags.TxPosVertDistance)]
        [DataRow(new byte[] { 19, 0, 0, 0, 0, 0, 0, 0xFE }, CapabilityFlags.TxNegVertDistance)]
        public void Parse_Capabilities_Matches(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var treadmill = new Treadmill();

            // Act
            treadmill.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(capabilities, treadmill.Capabilities);
        }
    }
}
