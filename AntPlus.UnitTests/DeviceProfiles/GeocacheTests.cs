using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles
{
    public class GeocacheTests
    {
        private readonly ChannelId cid = new(0);
        private readonly byte[] dp0 = [0x00, 0xA2, 0xFB, 0x65, 0x86, 0xCB, 0xEE, 0x94];    // beacon
        private readonly byte[] dp1 = [0x01, 0xFF, 0xD2, 0x04, 0x00, 0x00, 0x0A, 0xFF];    // pin page
        private readonly byte[] dp2 = [0x02, 0x02, 0x54, 0x68, 0x69, 0x73, 0x20, 0x69];    // hint pages
        private readonly byte[] dp3 = [0x03, 0x02, 0x73, 0x20, 0x61, 0x20, 0x76, 0x65];
        private readonly byte[] dp4 = [0x04, 0x02, 0x72, 0x79, 0x20, 0x6C, 0x6F, 0x6E];
        private readonly byte[] dp5 = [0x05, 0x02, 0x67, 0x20, 0x68, 0x69, 0x6E, 0x74];
        private readonly byte[] dp6 = [0x06, 0x02, 0x2E, 0x00, 0x00, 0x00, 0x00, 0x00];
        private readonly byte[] dp7 = [0x07, 0x00, 0x00, 0x00, 0x00, 0x20, 0xFF, 0xFF];    // latitude
        private readonly byte[] dp8 = [0x08, 0x04, 0xB3, 0x1D, 0xE5, 0x3E, 0x01, 0x00];    // logged visits
        private readonly byte[] dp9 = [0x09, 0x01, 0xDE, 0xDD, 0xDD, 0xBD, 0xFF, 0xFF];    // longitude

        private readonly byte[] cp80 = [0x50, 0xFF, 0xFF, 0x01, 0x0F, 0x00, 0x85, 0x83];   // manufacturer ID
        private readonly byte[] cp81 = [0x51, 0xFF, 0xFF, 0x01, 0x01, 0x00, 0x00, 0x00];   // product info
        private readonly byte[] cp82 = [0x52, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x55, 0x93];   // battery status

        private readonly string trackableId = "HOMEALONE";
        private readonly uint pin = 1234;
        private readonly string nextStageHint = "This is a very long hint.";
        private readonly double nextStageLat = 45.0;
        private readonly double nextStageLong = -93.0;
        private readonly ushort numVisits = 1;
        private readonly DateTime lastVisit = new(2023, 6, 8, 23, 52, 19);

        private readonly MockRepository mockRepository;
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger<Geocache>> mockLogger;

        public GeocacheTests()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>(MockBehavior.Loose);
            mockLogger = mockRepository.Create<ILogger<Geocache>>(MockBehavior.Loose);
        }

        [Fact]
        public void Parse_OutOfOrderDataPages_AllPropertiesCorrect()
        {
            // Arrange
            Geocache geocache = new(cid, mockAntChannel.Object, mockLogger.Object, null);
            List<byte[]> dataPages = new() { dp0, dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9, cp80, cp81, cp82,
                dp0, dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9, cp80, cp81, cp82 };

            // Act
            for (int i = 0; i < 32; i++)
            {
                // randomize list of data pages
                dataPages = Utils.ShuffleDataPages(dataPages);
                foreach (byte[] page in dataPages)
                {
                    geocache.Parse(page);
                }

                // Assert
                Assert.Equal(trackableId, geocache.TrackableId);
                Assert.Equal(pin, geocache.ProgrammingPIN);
                Assert.Equal(nextStageHint, geocache.Hint);
                Assert.Equal(nextStageLat, geocache.NextStageLatitude, 0.000001);
                Assert.Equal(nextStageLong, geocache.NextStageLongitude, 0.000001);
                Assert.Equal(numVisits, geocache.NumberOfVisits);
                Assert.Equal(lastVisit, geocache.LastVisitTimestamp);
            }
        }

        [Fact]
        public async Task Parse_AuthenticationPage_ExpectedToken()
        {
            // Arrange
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(cid, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).Returns(MessagingReturnCode.Pass);
            Geocache geocache = new(cid, mockAntChannel.Object, mockLogger.Object, null);
            byte[] token = [1, 2, 3, 4, 5, 6, 7];
            byte[] page = new byte[] { 32 }.Concat(token).ToArray();
            var result = await geocache.RequestAuthentication(0);

            // Act
            geocache.Parse(page);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.True(token.SequenceEqual(geocache.AuthenticationToken),
                string.Format("Expected {0}, Actual {1}", BitConverter.ToString(token), BitConverter.ToString(geocache.AuthenticationToken)));
        }

        [Fact]
        public void RequestPinPage_StateInitialize_StateCleared()
        {
            // Arrange
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(cid, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).Returns(MessagingReturnCode.Pass);
            List<byte[]> dataPages = new() { dp0, dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9 };
            Geocache geocache = new(cid, mockAntChannel.Object, mockLogger.Object, null);
            foreach (byte[] page in dataPages) { geocache.Parse(page); }

            // Act
            _ = geocache.RequestPinPage();

            // Assert
            Assert.Equal(string.Empty, geocache.TrackableId);
            Assert.Equal(default, geocache.ProgrammingPIN);
            Assert.Equal(default, geocache.TotalPagesProgrammed);
            Assert.Equal(default, geocache.NextStageLatitude);
            Assert.Equal(default, geocache.NextStageLongitude);
            Assert.Equal(string.Empty, geocache.Hint);
            Assert.Equal(default, geocache.NumberOfVisits);
            Assert.Equal(default, geocache.LastVisitTimestamp);
        }

        [Fact]
        public async Task UpdateLoggedVisits_LoggedVisitPageUnprogrammed_ThrowsException()
        {
            // Arrange
            Geocache geocache = new(cid, mockAntChannel.Object, mockLogger.Object, null);

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => geocache.UpdateLoggedVisits());
        }

        [Fact]
        public void UpdateLoggedVisits_OnePriorVisit_AddsVisit()
        {
            // Arrange
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(cid, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).Returns(MessagingReturnCode.Pass);
            Geocache geocache = new(cid, mockAntChannel.Object, mockLogger.Object, null);
            geocache.Parse(dp8);    // initialize logged visits
            DateTime dateTime = DateTime.UtcNow;

            // Act
            var result = geocache.UpdateLoggedVisits();

            // Assert
            Assert.Equal((ushort?)2, geocache.NumberOfVisits);
            Assert.Equal(0, (dateTime - geocache.LastVisitTimestamp).Value.Minutes);
        }

        [Fact]
        public async Task ProgramGeocache_StateInitialize_StateCleared()
        {
            // Arrange
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(cid, It.IsAny<byte[]>(), It.IsAny<uint>()).Result).Returns(MessagingReturnCode.Pass);
            List<byte[]> dataPages = new() { dp0, dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9 };
            Geocache geocache = new(cid, mockAntChannel.Object, mockLogger.Object, null);
            foreach (byte[] page in dataPages) { geocache.Parse(page); }

            // Act
            var result = await geocache.ProgramGeocache(
                trackableId,
                pin,
                nextStageLat,
                nextStageLong,
                nextStageHint);

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            Assert.Equal(string.Empty, geocache.TrackableId);
            Assert.Equal(default, geocache.ProgrammingPIN);
            Assert.Equal(default, geocache.TotalPagesProgrammed);
            Assert.Equal(default, geocache.NextStageLatitude);
            Assert.Equal(default, geocache.NextStageLongitude);
            Assert.Equal(string.Empty, geocache.Hint);
            Assert.Equal(default, geocache.NumberOfVisits);
            Assert.Equal(default, geocache.LastVisitTimestamp);
        }

        [Fact]
        public async Task EraseGeocache_Success_ReturnsPass()
        {
            // Arrange
            mockAntChannel.Setup(ac => ac.SendExtAcknowledgedDataAsync(cid, It.IsAny<byte[]>(), It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass);
            Geocache geocache = new(cid, mockAntChannel.Object, mockLogger.Object, null);

            // Act
            var result = await geocache.EraseGeocache();

            // Assert
            Assert.Equal(MessagingReturnCode.Pass, result);
            mockAntChannel.Verify(ac => ac.SendExtAcknowledgedDataAsync(
                cid,
                It.Is<byte[]>(msg =>
                    msg.Length == 8 &&
                    msg[0] == 0 &&
                    msg.Skip(1).All(el => el == 0)),
                It.IsAny<uint>()),
                Times.Once);
            mockAntChannel.Verify(ac => ac.SendExtAcknowledgedDataAsync(
                cid,
                It.Is<byte[]>(msg =>
                    msg.Length == 8 &&
                    msg[0] >= 1 && msg[0] <= 31 &&
                    msg.Skip(1).All(el => el == 0xFF)),
                It.IsAny<uint>()),
                Times.Exactly(31));
        }

        [Fact]
        public async Task EraseGeocache_Failure_ReturnsError()
        {
            // Arrange
            mockAntChannel.SetupSequence(ac => ac.SendExtAcknowledgedDataAsync(cid, It.IsAny<byte[]>(), It.IsAny<uint>()).Result)
                .Returns(MessagingReturnCode.Pass)
                .Returns(MessagingReturnCode.Fail);
            Geocache geocache = new(cid, mockAntChannel.Object, mockLogger.Object, null);

            // Act
            var result = await geocache.EraseGeocache();

            // Assert
            Assert.Equal(MessagingReturnCode.Fail, result);
            mockAntChannel.Verify(ac => ac.SendExtAcknowledgedDataAsync(cid, It.IsAny<byte[]>(), It.IsAny<uint>()), Times.AtLeastOnce);
        }
    }
}
