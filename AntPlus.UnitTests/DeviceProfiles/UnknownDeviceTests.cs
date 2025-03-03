using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;
using Xunit;

namespace AntPlus.UnitTests.DeviceProfiles
{
    public class UnknownDeviceTests
    {
        private readonly UnknownDevice _unknownDevice;

        public UnknownDeviceTests()
        {
            _unknownDevice = new(new ChannelId(0), Mock.Of<IAntChannel>(), Mock.Of<ILogger<UnknownDevice>>(), It.IsAny<int>());
        }

        [Fact]
        public void Parse_EmptyPageCollection_PageAdded()
        {
            // Arrange
            byte[] dataPage = [0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88];

            // Act
            _unknownDevice.Parse(dataPage);

            // Assert
            Assert.Single(_unknownDevice.DataPages);
            Assert.True(_unknownDevice.DataPages[0].SequenceEqual(dataPage));
        }

        [Fact]
        public void Parse_Update_PageUpdated()
        {
            // Arrange
            byte[] dataPage = [0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88];
            byte[] updatePage = [0x11, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x88];

            // Act
            _unknownDevice.Parse(dataPage);
            _unknownDevice.Parse(updatePage);

            // Assert
            Assert.Single(_unknownDevice.DataPages);
            Assert.True(_unknownDevice.DataPages[0].SequenceEqual(updatePage));
        }
    }
}
