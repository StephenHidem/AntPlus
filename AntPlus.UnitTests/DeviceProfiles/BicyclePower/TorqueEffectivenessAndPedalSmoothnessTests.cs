using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePowerTests
{
    [TestClass]
    public class TorqueEffectivenessAndPedalSmoothnessTests
    {
        private MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<NullLoggerFactory> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<NullLoggerFactory>();
        }

        private StandardPowerSensor CreateStandardPowerSensor()
        {
            byte[] page = new byte[8] { (byte)DataPage.PowerOnly, 0, 0, 0, 0, 0, 0, 0 };
            return BicyclePower.GetBicyclePowerSensor(page, mockChannelId, mockAntChannel.Object, mockLogger.Object, missedMessages: 8) as StandardPowerSensor;
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(200, 100)]
        [DataRow(0xFF, double.NaN)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedTorqueEffectiveness(int value, double expPct)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, (byte)value, (byte)value, 0xFF, 0xFF, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expPct, sensor.LeftTorqueEffectiveness);
            Assert.AreEqual(expPct, sensor.RightTorqueEffectiveness);
        }

        [TestMethod]
        [DataRow(0, 0, 0, 0, false)]
        [DataRow(200, 200, 100, 100, false)]
        [DataRow(0xFF, 0xFF, double.NaN, double.NaN, false)]
        [DataRow(100, 0xFE, 50, double.NaN, true)]
        public void Parse_TorqueEffectivenessAndPedalSmoothness_ExpectedPedalSmoothness(int left, int right, double expLeftPct, double expRightPct, bool expCombined)
        {
            // Arrange
            var sensor = CreateStandardPowerSensor();
            byte[] dataPage = new byte[8] { (byte)DataPage.TorqueEffectivenessAndPedalSmoothness, 0xFF, 0xFF, 0xFF, (byte)left, (byte)right, 0xFF, 0xFF };

            // Act
            sensor.Parse(
                dataPage);

            // Assert
            Assert.AreEqual(expLeftPct, sensor.LeftPedalSmoothness);
            Assert.AreEqual(expRightPct, sensor.RightPedalSmoothness);
            Assert.AreEqual(expCombined, sensor.CombinedPedalSmoothness);
        }
    }
}
