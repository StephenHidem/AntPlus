using DeviceProfiles;

namespace AntPlus.UnitTests
{
    [TestClass]
    public class AntDeviceTests
    {
        [TestMethod]
        [DataRow((uint)0x0FFF0001, (uint)0x00000001)]
        [DataRow((uint)0xFFFF0001, (uint)0x000F0001)]
        public void TestDeviceNumber(uint channelId, uint expectedDeviceNumber)
        {
            // Arrange
            var antDevice = new UnknownDevice(new ChannelId(channelId));

            // Act

            // Assert
            Assert.AreEqual(expectedDeviceNumber, antDevice.ChannelId.DeviceNumber);
        }

        [TestMethod]
        [DataRow((uint)0xFF81FFFF, (byte)0x01)]
        [DataRow((uint)0xFFFFFFFF, (byte)0x7F)]
        public void TestDeviceType(uint channelId, byte expectedDeviceType)
        {
            // Arrange
            var antDevice = new UnknownDevice(new ChannelId(channelId));

            // Act

            // Assert
            Assert.AreEqual(expectedDeviceType, antDevice.ChannelId.DeviceType);
        }

        [TestMethod]
        [DataRow((uint)0x00800000, true)]
        [DataRow((uint)0xFF7FFFFF, false)]
        public void TestPairingBit(uint channelId, bool expectedResult)
        {
            // Arrange
            var antDevice = new UnknownDevice(new ChannelId(channelId));

            // Act

            // Assert
            Assert.AreEqual(expectedResult, antDevice.ChannelId.IsPairingBitSet);
        }

        [TestMethod]
        [DataRow((uint)0x04000000, true)]
        [DataRow((uint)0xFBFFFFFF, false)]
        public void TestAreGlobalPageUsed(uint channelId, bool expectedResult)
        {
            // Arrange
            var antDevice = new UnknownDevice(new ChannelId(channelId));

            // Act

            // Assert
            Assert.AreEqual(expectedResult, antDevice.ChannelId.AreGlobalDataPagesUsed);
        }

        [TestMethod]
        [DataRow((uint)0xF0FFFFFF, ChannelSharing.Reserved)]
        [DataRow((uint)0xFCFFFFFF, ChannelSharing.Reserved)]
        [DataRow((uint)0xF1FFFFFF, ChannelSharing.IndependentChannel)]
        [DataRow((uint)0xFDFFFFFF, ChannelSharing.IndependentChannel)]
        [DataRow((uint)0xF2FFFFFF, ChannelSharing.SharedChannelOneByteAddress)]
        [DataRow((uint)0xFEFFFFFF, ChannelSharing.SharedChannelOneByteAddress)]
        [DataRow((uint)0xF3FFFFFF, ChannelSharing.SharedChannelTwoByteAddress)]
        [DataRow((uint)0xFFFFFFFF, ChannelSharing.SharedChannelTwoByteAddress)]
        public void TestTransmissionType(uint channelId, ChannelSharing expectedTranmissonType)
        {
            // Arrange
            var antDevice = new UnknownDevice(new ChannelId(channelId));

            // Act

            // Assert
            Assert.AreEqual(expectedTranmissonType, antDevice.ChannelId.TransmissionType);
        }
    }
}
