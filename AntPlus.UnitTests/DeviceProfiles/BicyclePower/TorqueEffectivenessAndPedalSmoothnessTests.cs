using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePower
{
    [TestClass]
    public class TorqueEffectivenessAndPedalSmoothnessTests
    {
        private MockRepository mockRepository;

        private Bicycle mockBicycle;
        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<Bicycle>> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Bicycle>>();
        }

        private Bicycle CreateBicyclePower()
        {
            return new Bicycle(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object);
        }

        private TorqueEffectivenessAndPedalSmoothness CreateTorqueEffectivenessAndPedalSmoothness()
        {
            mockBicycle = CreateBicyclePower();
            mockBicycle.Parse(new byte[8] { (byte)DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0 });
            return mockBicycle.PowerSensor.TorqueEffectiveness;
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(200, 100)]
        [DataRow(0xFF, double.NaN)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedTorqueEffectiveness(int value, double expPct)
        {
            // Arrange
            var torqueEffectiveness = CreateTorqueEffectivenessAndPedalSmoothness();
            byte[] dataPage = new byte[8] { (byte)DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, (byte)value, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            mockBicycle.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPct, torqueEffectiveness.LeftTorqueEffectiveness);
            Assert.AreEqual(expPct, torqueEffectiveness.RightTorqueEffectiveness);
        }

        [TestMethod]
        [DataRow(0, 0, 0, false)]
        [DataRow(200, 200, 100, false)]
        [DataRow(0xFF, 0xFF, double.NaN, false)]
        [DataRow(100, 0xFE, 50, true)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedPedalSmoothness(int left, int right, double expPct, bool expCombined)
        {
            // Arrange
            var torqueEffectiveness = CreateTorqueEffectivenessAndPedalSmoothness();
            byte[] dataPage = new byte[8] { (byte)DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, 0xFF, 0xFF, (byte)left, (byte)right, 0xFF, 0xFF };

            // Act
            mockBicycle.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPct, torqueEffectiveness.LeftPedalSmoothness);
            Assert.AreEqual(expPct, torqueEffectiveness.RightPedalSmoothness);
            Assert.AreEqual(expCombined, torqueEffectiveness.CombinedPedalSmoothness);
        }
    }
}
