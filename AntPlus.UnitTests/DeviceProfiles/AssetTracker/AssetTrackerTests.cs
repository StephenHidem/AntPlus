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
        private readonly Mock<IAntChannel> _mockAntChannel;
        private readonly Mock<ILogger<Tracker>> _mockLogger;
        private readonly Tracker _tracker;

        public AssetTrackerTests()
        {
            _mockAntChannel = new Mock<IAntChannel> { CallBase = true };
            _mockLogger = new Mock<ILogger<Tracker>>();
            _tracker = new Tracker(new ChannelId(0), _mockAntChannel.Object, _mockLogger.Object, It.IsAny<int>());
        }

        [Fact]
        public void Parse_OutOfOrderDataPages_ExpectedAssetProperties()
        {
            // Arrange
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(It.IsAny<ChannelId>(), It.IsAny<byte[]>(), It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);
            List<byte[]> dataPages = [
                [1, 0xE1, 20, 0, 128, 0, 0x00, 0x00],
                [2, 0xE1, 0x00, 0x20, 0xDE, 0xDD, 0xDD, 0xBD],
                [16, 0xE1, 128, (byte)'C', (byte)'a', (byte)'r', (byte)'l', (byte)'o'],
                [17, 0xE1, 0, (byte)'s', (byte)' ', (byte)'C', (byte)'a', (byte)'t']
            ];

            // Act
            for (int i = 0; i < 32; i++)
            {
                var tracker = new Tracker(new ChannelId(0), _mockAntChannel.Object, _mockLogger.Object, It.IsAny<int>());

                // randomize list of data pages
                dataPages = Utils.ShuffleDataPages(dataPages);
                foreach (byte[] dataPage in dataPages)
                {
                    tracker.Parse(dataPage);
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
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(It.IsAny<ChannelId>(), It.IsAny<byte[]>(), It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            dataPage[5] = (byte)situation;

            // Act
            _tracker.Parse(dataPage);

            // Assert
            Assert.Equal(expSituation, _tracker.Assets[0].Situation);
        }

        [Theory]
        [InlineData(0, Asset.AssetStatus.Good)]
        [InlineData(0x08, Asset.AssetStatus.LowBattery)]
        [InlineData(0x10, Asset.AssetStatus.GPSLost)]
        [InlineData(0x20, Asset.AssetStatus.CommunicationLost)]
        public void Parse_AssetStatus_ExpectedStatus(int status, Asset.AssetStatus expStatus)
        {
            // Arrange
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(It.IsAny<ChannelId>(), It.IsAny<byte[]>(), It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            dataPage[5] = (byte)status;

            // Act
            _tracker.Parse(dataPage);

            // Assert
            Assert.Equal(expStatus, _tracker.Assets[0].Status);
        }

        [Fact]
        public void Parse_AssetStatusRemoveAsset_ExpectedAssetCountIsZero()
        {
            // Arrange
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(It.IsAny<ChannelId>(), It.IsAny<byte[]>(), It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            _tracker.Parse(dataPage);
            dataPage[0] = 1;
            dataPage[5] = (byte)Asset.AssetStatus.RemoveAsset;

            // Act
            _tracker.Parse(dataPage);

            // Assert
            Assert.Empty(_tracker.Assets);
        }

        [Fact]
        public void Parse_NoAssets_ExpectedAssetCountIsZero()
        {
            // Arrange
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(It.IsAny<ChannelId>(), It.IsAny<byte[]>(), It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            _tracker.Parse(dataPage);
            dataPage[0] = 3;

            // Act
            _tracker.Parse(dataPage);

            // Assert
            Assert.Empty(_tracker.Assets);
        }

        [Fact]
        public void Parse_Disconnected_IsDisconnected()
        {
            // Arrange
            _mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedDataAsync(It.IsAny<ChannelId>(), It.IsAny<byte[]>(), It.IsAny<uint>())).
                ReturnsAsync(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            _tracker.Parse(dataPage);
            dataPage[0] = 32;

            // Act
            _tracker.Parse(dataPage);

            // Assert
            Assert.True(_tracker.Disconnected);
        }
    }
}
