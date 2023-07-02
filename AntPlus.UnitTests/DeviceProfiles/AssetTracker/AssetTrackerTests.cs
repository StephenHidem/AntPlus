using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.Generic;

namespace AntPlus.UnitTests.DeviceProfiles.AssetTracker
{
    [TestClass]
    public class AssetTrackerTests
    {
        private MockRepository mockRepository;

        private readonly ChannelId mockChannelId = new(0);
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<Tracker>> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Tracker>>(MockBehavior.Loose);
        }

        private SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker.Tracker CreateAssetTracker()
        {
            return new SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker.Tracker(
                mockChannelId,
                mockAntChannel.Object,
                mockLogger.Object);
        }

        [TestMethod]
        public void Parse_OutOfOrderDataPages_ExpectedAssetProperties()
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);
            List<byte[]> dataPages = new() {
                new byte[] { 1, 0xE1, 20, 0, 128, 0, 0x00, 0x00 },
                new byte[] { 2, 0xE1, 0x00, 0x20, 0xDE, 0xDD, 0xDD, 0xBD },
                new byte[] { 16, 0xE1, 128, (byte)'C', (byte)'a', (byte)'r', (byte)'l', (byte)'o' },
                new byte[] { 17, 0xE1, 0, (byte)'s', (byte)' ', (byte)'C', (byte)'a', (byte)'t' }
            };

            // Act
            for (int i = 0; i < 32; i++)
            {
                var tracker = CreateAssetTracker();

                // randomize list of datapages
                dataPages = Utils.ShuffleDataPages(dataPages);
                foreach (byte[] dataPage in dataPages)
                {
                    tracker.Parse(
                            dataPage);
                }

                // Assert
                Assert.IsTrue(tracker.Assets.Count == 1);
                Assert.IsTrue(tracker.Assets[0].Index == 1);
                Assert.IsTrue(tracker.Assets[0].Distance == 20);
                Assert.IsTrue(tracker.Assets[0].Bearing == 180.0);
                Assert.IsTrue(Convert.ToInt32(tracker.Assets[0].Latitude) == 45);
                Assert.IsTrue(Convert.ToInt32(tracker.Assets[0].Longitude) == -93);
                Assert.IsTrue(tracker.Assets[0].Color == 128);
                Assert.IsTrue(tracker.Assets[0].Name == "Carlos Cat");
                Assert.IsTrue(tracker.Assets[0].Type == Asset.AssetType.AssetTracker);
                mockRepository.VerifyAll();
            }
        }

        [TestMethod]
        [DataRow(0, Asset.AssetSituation.Sitting)]
        [DataRow(1, Asset.AssetSituation.Moving)]
        [DataRow(2, Asset.AssetSituation.Pointing)]
        [DataRow(3, Asset.AssetSituation.Treed)]
        [DataRow(4, Asset.AssetSituation.Unknown)]
        [DataRow(7, Asset.AssetSituation.Undefined)]
        public void Parse_AssetSituation_ExpectedSituation(int situation, Asset.AssetSituation expSituation)
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            dataPage[5] = (byte)situation;
            var tracker = CreateAssetTracker();

            // Act
            tracker.Parse(dataPage);

            // Assert
            Assert.AreEqual(expSituation, tracker.Assets[0].Situation);
        }

        [TestMethod]
        [DataRow(0, Asset.AssetStatus.Good)]
        [DataRow(0x08, Asset.AssetStatus.LowBattery)]
        [DataRow(0x10, Asset.AssetStatus.GPSLost)]
        [DataRow(0x20, Asset.AssetStatus.CommunicationLost)]
        [DataRow(0x40, Asset.AssetStatus.RemoveAsset)]
        public void Parse_AssetStatus_ExpectedStatus(int status, Asset.AssetStatus expStatus)
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            dataPage[5] = (byte)status;
            var tracker = CreateAssetTracker();

            // Act
            tracker.Parse(dataPage);

            // Assert
            if (expStatus == Asset.AssetStatus.RemoveAsset)
            {
                Assert.IsTrue(tracker.Assets.Count == 0);
            }
            else
            {
                Assert.AreEqual(expStatus, tracker.Assets[0].Status);
            }
        }

        [TestMethod]
        public void Parse_NoAssets_ExpectedAssetCountIsZero()
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            var tracker = CreateAssetTracker();
            tracker.Parse(dataPage);
            dataPage[0] = 3;

            // Act
            tracker.Parse(dataPage);

            // Assert
            Assert.IsTrue(tracker.Assets.Count == 0);
        }

        [TestMethod]
        public void Parse_Disconnected_IsDisconnected()
        {
            // Arrange
            mockAntChannel.Setup(ac =>
                ac.SendExtAcknowledgedData(mockChannelId, It.IsAny<byte[]>(), It.IsAny<uint>())).
                Returns(MessagingReturnCode.Pass);
            byte[] dataPage = new byte[8];
            dataPage[0] = 1;
            var tracker = CreateAssetTracker();
            tracker.Parse(dataPage);
            dataPage[0] = 32;

            // Act
            tracker.Parse(dataPage);

            // Assert
            Assert.IsTrue(tracker.Disconnected);
        }
    }
}
