using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.StrideBasedSpeedAndDistance;
using SmallEarthTech.AntRadioInterface;

namespace AntPlus.UnitTests.DeviceProfiles
{
    [TestClass]
    public class StrideBasedSpeedAndDistanceTests
    {
        private MockRepository mockRepository;

        private Mock<ChannelId> mockChannelId;
        private Mock<IAntChannel> mockAntChannel;

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockChannelId = this.mockRepository.Create<ChannelId>();
            this.mockAntChannel = this.mockRepository.Create<IAntChannel>();
        }

        private StrideBasedSpeedAndDistance CreateStrideBasedSpeedAndDistance()
        {
            return new StrideBasedSpeedAndDistance(
                this.mockChannelId.Object,
                this.mockAntChannel.Object);
        }

        [TestMethod]
        public void Parse_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var strideBasedSpeedAndDistance = this.CreateStrideBasedSpeedAndDistance();
            byte[] dataPage = null;

            // Act
            strideBasedSpeedAndDistance.Parse(
                dataPage);

            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        public void RequestSummaryPage_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var strideBasedSpeedAndDistance = this.CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.RequestSummaryPage();

            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        public void RequestBroadcastCapabilities_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var strideBasedSpeedAndDistance = this.CreateStrideBasedSpeedAndDistance();

            // Act
            strideBasedSpeedAndDistance.RequestBroadcastCapabilities();

            // Assert
            Assert.Fail();
            this.mockRepository.VerifyAll();
        }
    }
}
