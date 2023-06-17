using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.NordicSkier;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class NordicSkierTests
    {
        [TestMethod]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var nordicSkier = new NordicSkier();
            byte[] dataPage = new byte[] { 24, 0xFF, 0xFF, 0, 128, 0, 0x80, 0 };

            // Act
            nordicSkier.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(nordicSkier.Cadence == 128);
            Assert.IsTrue(nordicSkier.InstantaneousPower == 32768);
        }

        [TestMethod]
        public void Parse_StrideCount_Matches()
        {
            // Arrange
            var nordicSkier = new NordicSkier();
            byte[] dataPage = new byte[] { 24, 0xFF, 0xFF, 255, 0, 0, 0, 0 };
            nordicSkier.Parse(
                dataPage);
            dataPage[3] = 19;

            // Act
            nordicSkier.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(nordicSkier.StrideCount == 20);
        }

        [TestMethod]
        [DataRow(new byte[] { 24, 0xFF, 0xFF, 0, 0, 0, 0, 0x00 }, CapabilityFlags.None)]
        [DataRow(new byte[] { 24, 0xFF, 0xFF, 0, 0, 0, 0, 0x01 }, CapabilityFlags.TxStrideCount)]
        public void Parse_Capabilities_MatchesExpectedValue(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var nordicSkier = new NordicSkier();

            // Act
            nordicSkier.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(capabilities, nordicSkier.Capabilities);
        }
    }
}
