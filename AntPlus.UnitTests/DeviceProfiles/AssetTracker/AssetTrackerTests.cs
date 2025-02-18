using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntRadioInterface;
using System.Collections.Generic;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles.AssetTracker
{
    public class AssetTrackerTests
    {
        private readonly MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<Tracker>> mockLogger;

        public AssetTrackerTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Tracker>>(MockBehavior.Loose);
        }

        private Tracker CreateAssetTracker()
        {
            return new Tracker(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object,
                null);
        }

        [Fact]
        public void Parse_OutOfOrderDataPages_ExpectedAssetProperties()
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);
            List<byte[]> dataPages = [
                [1, 0xE1, 20, 0, 128, 0, 0x00, 0x00],
                [2, 0xE1, 0x00, 0x20, 0xDE, 0xDD, 0xDD, 0xBD],
                [16, 0xE1, 128, (byte)'C', (byte)'a', (byte)'r', (byte)'l', (byte)'o'],
                [17, 0xE1, 0, (byte)'s', (byte)' ', (byte)'C', (byte)'a', (byte)'t']
            ];

            // Act
            for (int i = 0; i < 32; i++)
            {
                var tracker = CreateAssetTracker();

                // randomize list of data pages
                dataPages = Utils.ShuffleDataPages(dataPages);
                foreach (byte[] dataPage in dataPages)
                {
                    tracker.Parse(
                            dataPage);
                }

                // Assert
                Assert.Single(tracker.Assets);
                Assert.Equal(1, tracker.Assets[0].Index);
                Assert.Equal(20, tracker.Assets[0].Distance);
                Assert.Equal(180.0, tracker.Assets[0].Bearing, 1.41);
                Assert.Equal(45, tracker.Assets[0].Latitude, 0.000001);
                Assert.Equal(-93, tracker.Assets[0].Longitude, 0.000001);
                Assert.Equal(128, tracker.Assets[0].Color);
                Assert.Equal("Carlos Cat", tracker.Assets[0].Name);
                Assert.Equal(Asset.AssetType.AssetTracker, tracker.Assets[0].Type);
                mockRepository.VerifyAll();
            }
        }

        [Theory]
        [InlineData(0, Asset.AssetSituation.Sitting)]
        [InlineData(1, Asset.AssetSituation.Moving)]
        [InlineData(2, Asset.AssetSituation.Pointing)]
        [InlineData(3, Asset.AssetSituation.Treed)]
        [InlineData(4, Asset.AssetSituation.Unknown)]
        [InlineData(7, Asset.AssetSituation.Undefined)]
        public void Parse_AssetSituation_ExpectedSituation(int situation, Asset.AssetSituation expSituation)
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            dataPage[5] = (byte)situation;
            var tracker = CreateAssetTracker();

            // Act
            tracker.Parse(dataPage);

            // Assert
            Assert.Equal(expSituation, tracker.Assets[0].Situation);
        }

        [Theory]
        [InlineData(0, Asset.AssetStatus.Good)]
        [InlineData(0x08, Asset.AssetStatus.LowBattery)]
        [InlineData(0x10, Asset.AssetStatus.GPSLost)]
        [InlineData(0x20, Asset.AssetStatus.CommunicationLost)]
        public void Parse_AssetStatus_ExpectedStatus(int status, Asset.AssetStatus expStatus)
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            dataPage[5] = (byte)status;
            var tracker = CreateAssetTracker();

            // Act
            tracker.Parse(dataPage);

            // Assert
            Assert.Equal(expStatus, tracker.Assets[0].Status);
        }

        [Fact]
        public void Parse_AssetStatusRemoveAsset_ExpectedAssetCountIsZero()
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            var tracker = CreateAssetTracker();
            tracker.Parse(dataPage);
            dataPage[0] = 1;
            dataPage[5] = (byte)Asset.AssetStatus.RemoveAsset;

            // Act
            tracker.Parse(dataPage);

            // Assert
            Assert.Empty(tracker.Assets);
        }

        [Fact]
        public void Parse_NoAssets_ExpectedAssetCountIsZero()
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            var tracker = CreateAssetTracker();
            tracker.Parse(dataPage);
            dataPage[0] = 3;

            // Act
            tracker.Parse(dataPage);

            // Assert
            Assert.Empty(tracker.Assets);
        }

        [Fact]
        public void Parse_Disconnected_IsDisconnected()
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).
                Returns(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            var tracker = CreateAssetTracker();
            tracker.Parse(dataPage);
            dataPage[0] = 32;

            // Act
            tracker.Parse(dataPage);

            // Assert
            Assert.True(tracker.Disconnected);
        }
    }
}
