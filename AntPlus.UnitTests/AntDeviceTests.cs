using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntRadioInterface;
using System.Threading;

namespace SmallEarthTech.AntPlus.UnitTests
{
    [TestClass]
    public class AntDeviceTests
    {
        private MockRepository mockRepository;

        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger<Tracker>> mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);

            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger<Tracker>>(MockBehavior.Loose);
        }

        [TestMethod]
        [DataRow((uint)0x0FFF0001, (uint)0x00000001)]
        [DataRow((uint)0xFFFF0001, (uint)0x000F0001)]
        public void Ctor_ChannelID_VerifyDeviceNumber(uint channelId, uint expectedDeviceNumber)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, mockAntChannel.Object, mockLogger.Object, 500);

            // Assert
            Assert.AreEqual((uint)expectedDeviceNumber, antDevice.Object.ChannelId.DeviceNumber);
        }

        [TestMethod]
        [DataRow((uint)0xFF81FFFF, (byte)0x01)]
        [DataRow((uint)0xFFFFFFFF, (byte)0x7F)]
        public void Ctor_ChannelID_VerifyDeviceType(uint channelId, byte expectedDeviceType)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, mockAntChannel.Object, mockLogger.Object, 500);

            // Assert
            Assert.AreEqual(expectedDeviceType, antDevice.Object.ChannelId.DeviceType);
        }

        [TestMethod]
        [DataRow((uint)0x00800000, true)]
        [DataRow((uint)0xFF7FFFFF, false)]
        public void Ctor_ChannelID_VerifyPairingBit(uint channelId, bool expectedResult)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, mockAntChannel.Object, mockLogger.Object, 500);

            // Assert
            Assert.AreEqual(expectedResult, antDevice.Object.ChannelId.IsPairingBitSet);
        }

        [TestMethod]
        [DataRow((uint)0x04000000, true)]
        [DataRow((uint)0xFBFFFFFF, false)]
        public void Ctor_ChannelID_VerifyAreGlobalPageUsed(uint channelId, bool expectedResult)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, mockAntChannel.Object, mockLogger.Object, 500);

            // Assert
            Assert.AreEqual(expectedResult, antDevice.Object.ChannelId.AreGlobalDataPagesUsed);
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
        public void Ctor_ChannelID_VerifyTransmissionType(uint channelId, ChannelSharing expectedTransmissionType)
        {
            // Arrange
            ChannelId cid = new((uint)channelId);

            // Act
            Mock<AntDevice> antDevice = new(cid, mockAntChannel.Object, mockLogger.Object, 500);

            // Assert
            Assert.AreEqual(expectedTransmissionType, antDevice.Object.ChannelId.TransmissionType);
        }

        [TestMethod]
        public void Timeout_Offline_ExpectedEventAndStatus()
        {
            // Arrange
            bool offline = false;
            ChannelId cid = new(0);
            Mock<AntDevice> antDevice = new(cid, mockAntChannel.Object, mockLogger.Object, 50);
            antDevice.Object.DeviceWentOffline += (sender, e) => { offline = antDevice.Object.Offline; };

            // Act
            Thread.Sleep(100);

            // Assert
            Assert.IsTrue(offline == true);
            Assert.IsTrue(antDevice.Object.Offline == true);
        }
    }
}
