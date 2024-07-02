using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.NordicSkier;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class NordicSkierTests
    {
        private MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<Equipment>> mockLogger;


        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Equipment>>(MockBehavior.Loose);
        }

        private NordicSkier CreateNordicSkier()
        {
            return new NordicSkier(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object);
        }

        [TestMethod]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var nordicSkier = CreateNordicSkier();
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
            var nordicSkier = CreateNordicSkier();
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
            var nordicSkier = CreateNordicSkier();

            // Act
            nordicSkier.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(capabilities, nordicSkier.Capabilities);
        }
    }
}
