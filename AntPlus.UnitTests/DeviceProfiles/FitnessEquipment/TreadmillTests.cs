using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Treadmill;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class TreadmillTests
    {
        private MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<Treadmill>> mockLogger;


        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Treadmill>>(MockBehavior.Loose);
        }

        private Treadmill CreateTreadmill()
        {
            return new Treadmill(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null);
        }

        [TestMethod]
        public void Parse_InstantaneousCadence_Matches()
        {
            // Arrange
            var treadmill = CreateTreadmill();
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
            var treadmill = CreateTreadmill();
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
            var treadmill = CreateTreadmill();
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
            var treadmill = CreateTreadmill();

            // Act
            treadmill.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(capabilities, treadmill.Capabilities);
        }
    }
}
