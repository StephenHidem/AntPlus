using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.Generic;

namespace AntPlus.UnitTests.DeviceProfiles
{
    [TestClass]
    public class GeocacheTests
    {
        private readonly ChannelId cid = new(0);
        private readonly byte[] dp0 = new byte[8] { 0x00, 0xA2, 0xFB, 0x65, 0x86, 0xCB, 0xEE, 0x94 };    // beacon
        private readonly byte[] dp1 = new byte[8] { 0x01, 0xFF, 0xD2, 0x04, 0x00, 0x00, 0x0A, 0xFF };    // pin page
        private readonly byte[] dp2 = new byte[8] { 0x02, 0x02, 0x54, 0x68, 0x69, 0x73, 0x20, 0x69 };    // hint pages
        private readonly byte[] dp3 = new byte[8] { 0x03, 0x02, 0x73, 0x20, 0x61, 0x20, 0x76, 0x65 };
        private readonly byte[] dp4 = new byte[8] { 0x04, 0x02, 0x72, 0x79, 0x20, 0x6C, 0x6F, 0x6E };
        private readonly byte[] dp5 = new byte[8] { 0x05, 0x02, 0x67, 0x20, 0x68, 0x69, 0x6E, 0x74 };
        private readonly byte[] dp6 = new byte[8] { 0x06, 0x02, 0x2E, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private readonly byte[] dp7 = new byte[8] { 0x07, 0x00, 0x00, 0x00, 0x00, 0x20, 0xFF, 0xFF };    // latitude
        private readonly byte[] dp8 = new byte[8] { 0x08, 0x04, 0xB3, 0x1D, 0xE5, 0x3E, 0x01, 0x00 };    // logged visits
        private readonly byte[] dp9 = new byte[8] { 0x09, 0x01, 0xDE, 0xDD, 0xDD, 0xBD, 0xFF, 0xFF };    // longitude

        private readonly byte[] cp80 = new byte[8] { 0x50, 0xFF, 0xFF, 0x01, 0x0F, 0x00, 0x85, 0x83 };   // manufacturer ID
        private readonly byte[] cp81 = new byte[8] { 0x51, 0xFF, 0xFF, 0x01, 0x01, 0x00, 0x00, 0x00 };   // product info
        private readonly byte[] cp82 = new byte[8] { 0x52, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x55, 0x93 };   // battery status

        private readonly string trackableId = "HOMEALONE";
        private readonly uint pin = 1234;
        private readonly string nextStageHint = "This is a very long hint.";
        private readonly int nextStageLat = 45;
        private readonly int nextStageLong = -93;
        private readonly ushort numVisits = 1;
        private readonly DateTime lastVisit = new(2023, 6, 8, 23, 52, 19);

        [TestMethod]
        public void Parse_OutOfOrderDataPages_AllPropertiesCorrect()
        {
            // Arrange
            Geocache geocache = new(cid, null);
            List<byte[]> dataPages = new() { dp0, dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9, cp80, cp81, cp82,
                dp0, dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9, cp80, cp81, cp82 };

            // Act
            for (int i = 0; i < 32; i++)
            {
                // randomize list of datapages
                dataPages = Utils.ShuffleDataPages(dataPages);
                foreach (byte[] page in dataPages)
                {
                    geocache.Parse(page);
                }

                // Assert
                Assert.IsTrue(geocache.TrackableId == trackableId);
                Assert.IsTrue(geocache.ProgrammingPIN == pin);
                Assert.IsTrue(geocache.Hint == nextStageHint);
                Assert.IsTrue(Convert.ToInt32(geocache.NextStageLatitude) == nextStageLat);
                Assert.IsTrue(Convert.ToInt32(geocache.NextStageLongitude) == nextStageLong);
                Assert.IsTrue(geocache.NumberOfVisits == numVisits);
                Assert.IsTrue(geocache.LastVisitTimestamp == lastVisit);
            }
        }

        [TestMethod]
        public void RequestPinPage_StateInitialize_StateCleared()
        {
            // Arrange
            Mock<IAntChannel> antChannel = new();
            List<byte[]> dataPages = new() { dp0, dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9 };
            Geocache geocache = new(cid, antChannel.Object);
            foreach (byte[] page in dataPages) { geocache.Parse(page); }

            // Act
            _ = geocache.RequestPinPage();

            // Assert
            Assert.IsTrue(geocache.TrackableId == string.Empty);
            Assert.IsTrue(geocache.ProgrammingPIN == default);
            Assert.IsTrue(geocache.TotalPagesProgrammed == default);
            Assert.IsTrue(geocache.NextStageLatitude == default);
            Assert.IsTrue(geocache.NextStageLongitude == default);
            Assert.IsTrue(geocache.Hint == string.Empty);
            Assert.IsTrue(geocache.NumberOfVisits == default);
            Assert.IsTrue(geocache.LastVisitTimestamp == default);
        }

        //[TestMethod]
        //public void RequestAuthentication_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    Geocache geocache = new(cid, null);
        //    uint gpsSerialNumber = 0;

        //    // Act
        //    var result = geocache.RequestAuthentication(
        //        gpsSerialNumber);

        //    // Assert
        //    Assert.Fail();
        //}

        [TestMethod]
        public void UpdateLoggedVisits_LoggedVisitPageUnprogrammed_ThrowsException()
        {
            // Arrange
            Geocache geocache = new(cid, null);

            // Act and Assert
            _ = Assert.ThrowsException<InvalidOperationException>(() => geocache.UpdateLoggedVisits());
        }

        [TestMethod]
        public void UpdateLoggedVisits_OnePriorVisit_AddsVisit()
        {
            // Arrange
            Mock<IAntChannel> antChannel = new();
            Geocache geocache = new(cid, antChannel.Object);
            geocache.Parse(dp8);    // initialize logged visits
            DateTime dateTime = DateTime.UtcNow;

            // Act
            var result = geocache.UpdateLoggedVisits();

            // Assert
            Assert.IsTrue(geocache.NumberOfVisits == 2);
            Assert.IsTrue((dateTime - geocache.LastVisitTimestamp).Value.Minutes == 0);
        }

        [TestMethod]
        public void ProgramGeocache_StateInitialize_StateCleared()
        {
            // Arrange
            Mock<IAntChannel> antChannel = new();
            antChannel.Setup(ac => ac.SendExtAcknowledgedData(cid, It.IsAny<byte[]>(), It.IsAny<uint>())).Returns(MessagingReturnCode.Pass);
            List<byte[]> dataPages = new() { dp0, dp1, dp2, dp3, dp4, dp5, dp6, dp7, dp8, dp9 };
            Geocache geocache = new(cid, antChannel.Object);
            foreach (byte[] page in dataPages) { geocache.Parse(page); }

            // Act
            geocache.ProgramGeocache(
                trackableId,
                pin,
                nextStageLat,
                nextStageLong,
                nextStageHint);

            // Assert
            Assert.IsTrue(geocache.TrackableId == string.Empty);
            Assert.IsTrue(geocache.ProgrammingPIN == default);
            Assert.IsTrue(geocache.TotalPagesProgrammed == default);
            Assert.IsTrue(geocache.NextStageLatitude == default);
            Assert.IsTrue(geocache.NextStageLongitude == default);
            Assert.IsTrue(geocache.Hint == string.Empty);
            Assert.IsTrue(geocache.NumberOfVisits == default);
            Assert.IsTrue(geocache.LastVisitTimestamp == default);
        }
    }
}
