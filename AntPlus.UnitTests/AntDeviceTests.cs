using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntRadioInterface;
using Xunit;

namespace AntPlus.UnitTests
{
    public class AntDeviceTests
    {
        private readonly Mock<IAntChannel> _mockAntChannel;
        private readonly Mock<ILogger<AntDevice>> _mockLogger;

        public AntDeviceTests()
        {
            _mockAntChannel = new Mock<IAntChannel>();
            _mockLogger = new Mock<ILogger<AntDevice>>();
        }

        [Theory]
        [InlineData((uint)0x0FFF0001, (uint)0x00000001)]
        [InlineData((uint)0xFFFF0001, (uint)0x000F0001)]
        public void Ctor_ChannelID_VerifyDeviceNumber(uint channelId, uint expectedDeviceNumber)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, _mockAntChannel.Object, _mockLogger.Object, It.IsAny<int>());

            // Assert
            Assert.Equal((uint)expectedDeviceNumber, antDevice.Object.ChannelId.DeviceNumber);
        }

        [Theory]
        [InlineData(0xFF81FFFF, (byte)0x01)]
        [InlineData(0xFFFFFFFF, (byte)0x7F)]
        public void Ctor_ChannelID_VerifyDeviceType(uint channelId, byte expectedDeviceType)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, _mockAntChannel.Object, _mockLogger.Object, It.IsAny<int>());

            // Assert
            Assert.Equal(expectedDeviceType, antDevice.Object.ChannelId.DeviceType);
        }

        [Theory]
        [InlineData((uint)0x00800000, true)]
        [InlineData((uint)0xFF7FFFFF, false)]
        public void Ctor_ChannelID_VerifyPairingBit(uint channelId, bool expectedResult)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, _mockAntChannel.Object, _mockLogger.Object, It.IsAny<int>());

            // Assert
            Assert.Equal(expectedResult, antDevice.Object.ChannelId.IsPairingBitSet);
        }

        [Theory]
        [InlineData((uint)0x04000000, true)]
        [InlineData((uint)0xFBFFFFFF, false)]
        public void Ctor_ChannelID_VerifyAreGlobalPageUsed(uint channelId, bool expectedResult)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, _mockAntChannel.Object, _mockLogger.Object, It.IsAny<int>());

            // Assert
            Assert.Equal(expectedResult, antDevice.Object.ChannelId.AreGlobalDataPagesUsed);
        }

        [Theory]
        [InlineData((uint)0xF0FFFFFF, ChannelSharing.Reserved)]
        [InlineData((uint)0xFCFFFFFF, ChannelSharing.Reserved)]
        [InlineData((uint)0xF1FFFFFF, ChannelSharing.IndependentChannel)]
        [InlineData((uint)0xFDFFFFFF, ChannelSharing.IndependentChannel)]
        [InlineData((uint)0xF2FFFFFF, ChannelSharing.SharedChannelOneByteAddress)]
        [InlineData((uint)0xFEFFFFFF, ChannelSharing.SharedChannelOneByteAddress)]
        [InlineData((uint)0xF3FFFFFF, ChannelSharing.SharedChannelTwoByteAddress)]
        [InlineData((uint)0xFFFFFFFF, ChannelSharing.SharedChannelTwoByteAddress)]
        public void Ctor_ChannelID_VerifyTransmissionType(uint channelId, ChannelSharing expectedTransmissionType)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, _mockAntChannel.Object, _mockLogger.Object, It.IsAny<int>());

            // Assert
            Assert.Equal(expectedTransmissionType, antDevice.Object.ChannelId.TransmissionType);
        }

        //[Fact]
        //public void Timeout_Offline_ExpectedEventAndStatus()
        //{
        //    // Arrange
        //    bool offline = false;
        //    Mock<AntDevice> antDevice = new(new ChannelId(0), _mockAntChannel.Object, _mockLogger.Object, (int)50);
        //    antDevice.Object.DeviceWentOffline += (sender, e) => { offline = antDevice.Object.Offline; };

        //    // Act
        //    Thread.Sleep(100);

        //    // Assert
        //    Assert.True(offline, "Event test");
        //    Assert.True(antDevice.Object.Offline, "Device offline status");
        //}
    }
}
