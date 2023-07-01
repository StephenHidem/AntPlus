using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;

namespace AntPlus.UnitTests.DeviceProfiles
{
    [TestClass]
    public class UnknownDeviceTests
    {
        readonly ChannelId cid = new(0);
        readonly Mock<ILogger<UnknownDevice>> mockLogger = new();

        [TestMethod]
        public void Parse_EmptyPageCollection_PageAdded()
        {
            // Arrange
            UnknownDevice unknownDevice = new(cid, null, mockLogger.Object);
            byte[] dataPage = { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };

            // Act
            unknownDevice.Parse(
                dataPage);

            // Assert
            Assert.IsTrue(unknownDevice.DataPages.Count == 1);
            Assert.IsTrue(unknownDevice.DataPages[0].SequenceEqual(dataPage));
        }

        [TestMethod]
        public void Parse_Update_PageUpdated()
        {
            // Arrange
            UnknownDevice unknownDevice = new(cid, null, mockLogger.Object);
            byte[] dataPage = { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };
            byte[] updatePage = { 0x11, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x88 };

            // Act
            unknownDevice.Parse(dataPage);
            unknownDevice.Parse(updatePage);

            // Assert
            Assert.IsTrue(unknownDevice.DataPages.Count == 1);
            Assert.IsTrue(unknownDevice.DataPages[0].SequenceEqual(updatePage));
        }
    }
}
