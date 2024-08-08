using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Climber;

namespace AntPlus.UnitTests.DeviceProfiles.FitnessEquipment
{
    [TestClass]
    public class ClimberTests
    {
        private MockRepository mockRepository;
        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<Climber>> mockLogger;


        [TestInitialize]
        public void Initialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Climber>>(MockBehavior.Loose);
        }

        private Climber CreateClimber()
        {
            return new Climber(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object, null);
        }

        [TestMethod]
        public void Parse_InstantaneousCadenceAndPower_Matches()
        {
            // Arrange
            var climber = CreateClimber();
            byte[] dataPage = new byte[] { 23, 0, 0, 0, 128, 0, 0x80, 0 };

            // Act
            climber.Parse(dataPage);

            // Assert
            Assert.IsTrue(climber.Cadence == 128);
            Assert.IsTrue(climber.InstantaneousPower == 32768);
        }

        [TestMethod]
        [DataRow(new byte[] { 23, 0, 0, 0, 0, 0, 0, 0xFF }, CapabilityFlags.TxStrides)]
        [DataRow(new byte[] { 23, 0, 0, 0, 0, 0, 0, 0xFE }, CapabilityFlags.None)]
        public void Parse_Capabilities_Matches(byte[] dataPage, CapabilityFlags capabilities)
        {
            // Arrange
            var climber = CreateClimber();

            // Act
            climber.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(capabilities, climber.Capabilities);
        }

        [TestMethod]
        public void Parse_StrideCyclesRollover_MatchesExpectedValue()
        {
            // Arrange
            var climber = CreateClimber();
            byte[] dataPage = new byte[] { 23, 0xFF, 0xFF, 255, 0, 0, 0, 0 };
            climber.Parse(
                dataPage);
            dataPage[3] = 19;

            // Act
            climber.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(climber.StrideCycles == 20);
        }
    }
}
