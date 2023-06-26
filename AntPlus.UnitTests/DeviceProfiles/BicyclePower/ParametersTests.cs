using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System.Data;

namespace AntPlus.UnitTests.DeviceProfiles.BicyclePower
{
    [TestClass]
    public class ParametersTests
    {
        private MockRepository mockRepository;

        private ChannelId mockChannelId;
        private Mock<IAntChannel> mockAntChannel;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockChannelId = new ChannelId(0);
            mockAntChannel = mockRepository.Create<IAntChannel>();
        }

        private Parameters CreateParameters()
        {
            return new Parameters(
                new SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower.BicyclePower(mockChannelId, mockAntChannel.Object));
        }

        [TestMethod]
        [DataRow(Subpage.CrankParameters)]
        [DataRow(Subpage.AdvancedCapabilities1)]
        [DataRow(Subpage.AdvancedCapabilities2)]
        [DataRow(Subpage.RiderPositionConfiguration)]
        [DataRow(Subpage.PowerPhaseConfiguration)]
        public void GetParameters_RequestedSubpage_ExpectedDataPage(Subpage page)
        {
            // Arrange
            var parameters = CreateParameters();
            byte[] dataPage = new byte[8] {
                (byte)CommonDataPage.RequestDataPage,
                0xFF,
                0xFF,
                (byte)page,
                0xFF,
                4,
                (byte)DataPage.GetSetParameters,
                (byte)SmallEarthTech.AntPlus.CommandType.DataPage
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, dataPage, It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);

            // Act
            parameters.GetParameters(
                page);

            // Assert
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(172.5)]
        [DataRow(237)]
        public void SetCrankLength_Message_ExpectedDataPage(double length)
        {
            // Arrange
            var parameters = CreateParameters();
            byte expVal = (byte)((length - 110) / 0.5);
            byte[] dataPage = new byte[8] {
                (byte)DataPage.GetSetParameters,
                (byte)Subpage.CrankParameters,
                0xFF,
                0xFF,
                expVal,
                0x00,
                0x00,
                0xFF
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, dataPage, It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);

            // Act
            parameters.SetCrankLength(
                length);

            // Assert
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void SetTransitionTimeOffset_Message_ExpectedDataPage()
        {
            // Arrange
            var parameters = CreateParameters();
            byte offset = 128;
            byte[] dataPage = new byte[8] {
                (byte)DataPage.GetSetParameters,
                (byte)Subpage.RiderPositionConfiguration,
                offset,
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, dataPage, It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);

            // Act
            parameters.SetTransitionTimeOffset(
                offset);

            // Assert
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public void SetPeakTorqueThreshold_Message_ExpectedDataPage()
        {
            // Arrange
            var parameters = CreateParameters();
            double threshold = 50;
            byte[] dataPage = new byte[8] {
                (byte)DataPage.GetSetParameters,
                (byte)Subpage.PowerPhaseConfiguration,
                (byte)(threshold / 0.5),
                0xFF,
                0xFF,
                0xFF,
                0xFF,
                0xFF
            };
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, dataPage, It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);

            // Act
            parameters.SetPeakTorqueThreshold(
                threshold);

            // Assert
            mockRepository.VerifyAll();
        }
    }
}
