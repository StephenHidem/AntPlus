using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Elliptical;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class EllipticalTests
    {
        private MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment>> mockLogger;


        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment>>(MockBehavior.Loose);
        }

        private Elliptical CreateElliptical()
        {
            return new Elliptical(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null, 8);
        }

        [TestMethod]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var elliptical = CreateElliptical();
            byte[] dataPage = new byte[] { 20, 0xFF, 0, 0, 128, 0, 0x80, 0 };

            // Act
            elliptical.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(elliptical.Cadence == 128);
            Assert.IsTrue(elliptical.InstantaneousPower == 32768);
        }

        [TestMethod]
        public void Parse_PositiveVerticalDistance_Matches()
        {
            // Arrange
            var elliptical = CreateElliptical();
            byte[] dataPage = new byte[] { 20, 0xFF, 255, 0, 0, 0, 0, 0 };
            elliptical.Parse(
                dataPage);
            dataPage[2] = 19;

            // Act
            elliptical.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(elliptical.PosVerticalDistance == 2.0);
        }

        [TestMethod]
        public void Parse_StrideCount_Matches()
        {
            // Arrange
            var elliptical = CreateElliptical();
            byte[] dataPage = new byte[] { 20, 0xFF, 0, 255, 0, 0, 0, 0 };
            elliptical.Parse(
                dataPage);
            dataPage[3] = 19;

            // Act
            elliptical.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(elliptical.StrideCount == 20);
        }

        [TestMethod]
        [DataRow(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x00 }, CapabilityFlags.None)]
        [DataRow(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x01 }, CapabilityFlags.TxStrideCount)]
        [DataRow(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x02 }, CapabilityFlags.TxPosVertDistance)]
        [DataRow(new byte[] { 20, 0xFF, 0, 0, 0, 0, 0, 0x03 }, CapabilityFlags.TxPosVertDistance | CapabilityFlags.TxStrideCount)]
        public void Parse_Capabilities_MatchesExpectedValue(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var elliptical = CreateElliptical();

            // Act
            elliptical.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(capabilities, elliptical.Capabilities);
        }
    }
}
