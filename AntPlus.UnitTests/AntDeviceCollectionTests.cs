using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AntPlus.UnitTests
{
    [TestClass]
    public class AntDeviceCollectionTests
    {
        private MockRepository mockRepository;

        private Mock<IAntRadio> mockAntRadio;
        private Mock<IAntChannel> mockAntChannel;
        private Mock<ILogger> mockLogger;
        private Mock<ILoggerFactory> mockLoggerFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockAntRadio = mockRepository.Create<IAntRadio>();
            mockAntChannel = mockRepository.Create<IAntChannel>();
            mockLogger = mockRepository.Create<ILogger>();
            mockLoggerFactory = mockRepository.Create<ILoggerFactory>();
            mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);
        }

        private AntDeviceCollection CreateAntDeviceCollection()
        {
            var mockChannels = new Mock<IAntChannel>[8];
            Array.Fill(mockChannels, mockAntChannel);
            mockAntRadio.Setup(r => r.InitializeContinuousScanMode()).ReturnsAsync(mockChannels.Select(m => m.Object).ToArray());
            AntDeviceCollection adc = new(
                mockAntRadio.Object,
                mockLoggerFactory.Object);
            return adc;
        }

        [TestMethod]
        public async Task MultithreadedAdd_Collection_ExpectedCount()
        {
            // Arrange
            var antDeviceCollection = CreateAntDeviceCollection();
            await antDeviceCollection.StartScanning();
            int numberOfDevices = 16;
            using SemaphoreSlim semaphore = new(0, numberOfDevices);
            Task[] tasks = new Task[numberOfDevices];
            for (int i = 0; i < numberOfDevices; i++)
            {
                Mock<AntDevice> antDevice = new(new ChannelId((uint)i), mockAntChannel.Object, mockLogger.Object, (int)500);
                tasks[i] = Task.Run(() =>
                {
                    semaphore.Wait();
                    antDeviceCollection.Add(
                        antDevice.Object);
                });
            }

            // Act
            semaphore.Release(numberOfDevices);
            Task.WaitAll(tasks);

            // Assert
            Assert.AreEqual(numberOfDevices, antDeviceCollection.Count);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        public async Task MultithreadedRemove_Collection_ExpectedCount()
        {
            // Arrange
            var antDeviceCollection = CreateAntDeviceCollection();
            await antDeviceCollection.StartScanning();
            int numberOfDevices = 16;
            using SemaphoreSlim semaphore = new(0, numberOfDevices);
            Task[] tasks = new Task[numberOfDevices];
            for (int i = 0; i < numberOfDevices; i++)
            {
                Mock<AntDevice> antDevice = new(new ChannelId((uint)i), mockAntChannel.Object, mockLogger.Object, (int)500);
                antDeviceCollection.Add(antDevice.Object);
                tasks[i] = Task.Run(() =>
                {
                    semaphore.Wait();
                    _ = antDeviceCollection.Remove(
                        antDevice.Object);
                });
            }

            // Act
            semaphore.Release(numberOfDevices);
            Task.WaitAll(tasks);

            // Assert
            Assert.AreEqual(0, antDeviceCollection.Count);
            mockRepository.VerifyAll();
        }

        [TestMethod]
        [DataRow(HeartRate.DeviceClass, typeof(HeartRate))]
        [DataRow(BicyclePower.DeviceClass, typeof(StandardPowerSensor))]
        [DataRow(BikeSpeedSensor.DeviceClass, typeof(BikeSpeedSensor))]
        [DataRow(BikeCadenceSensor.DeviceClass, typeof(BikeCadenceSensor))]
        [DataRow(CombinedSpeedAndCadenceSensor.DeviceClass, typeof(CombinedSpeedAndCadenceSensor))]
        [DataRow(FitnessEquipment.DeviceClass, typeof(UnknownDevice))]
        [DataRow(MuscleOxygen.DeviceClass, typeof(MuscleOxygen))]
        [DataRow(Geocache.DeviceClass, typeof(Geocache))]
        [DataRow(Tracker.DeviceClass, typeof(Tracker))]
        [DataRow(StrideBasedSpeedAndDistance.DeviceClass, typeof(StrideBasedSpeedAndDistance))]
        public async Task ChannelResponseEvent_Collection_ExpectedDeviceInCollection(byte deviceClass, Type deviceType)
        {
            // Arrange
            byte[] id = new byte[4] { 1, 0, deviceClass, 0 };
            ChannelId cid = new(BitConverter.ToUInt32(id));
            mockAntChannel.SetupAdd(m => m.ChannelResponse += It.IsAny<EventHandler<AntResponse>>());
            mockAntChannel.SetupRemove(m => m.ChannelResponse -= It.IsAny<EventHandler<AntResponse>>());
            var mockResponse = new MockResponse(cid, new byte[8]);
            var antDeviceCollection = CreateAntDeviceCollection();
            await antDeviceCollection.StartScanning();

            // Act
            mockAntChannel.Raise(m => m.ChannelResponse += null, mockAntChannel.Object, mockResponse);

            Assert.AreEqual(1, antDeviceCollection.Count);
            Assert.AreEqual(deviceType, antDeviceCollection[0].GetType());
        }

        class MockResponse : AntResponse
        {
            public MockResponse(ChannelId channelId, byte[] payload)
            {
                ChannelId = channelId;
                Payload = payload;
            }
        }

        //[TestMethod]
        //public async Task DeviceOffline_RemovesDeviceFromCollection()
        //{
        //    // Arrange
        //    var antDeviceCollection = CreateAntDeviceCollection();
        //    await antDeviceCollection.StartScanning();
        //    Mock<AntDevice> antDevice = new(new ChannelId(1), mockAntChannel.Object, Mock.Of<ILogger>(), 50);
        //    antDeviceCollection.Add(antDevice.Object);

        //    // Act
        //    antDevice.Raise(d => d.DeviceWentOffline += null, EventArgs.Empty);
        //    //Thread.Sleep(100);

        //    // Assert
        //    Assert.AreEqual(0, antDeviceCollection.Count);
        //}

        [TestMethod]
        public async Task MessageHandler_NullChannelId_LogsCritical()
        {
            // Arrange
            var antDeviceCollection = CreateAntDeviceCollection();
            await antDeviceCollection.StartScanning();
            var response = new MockResponse(null, new byte[8]);

            // Act
            mockAntChannel.Raise(m => m.ChannelResponse += null, mockAntChannel.Object, response);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("ChannelId or Payload is null")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [TestMethod]
        public async Task MessageHandler_NullPayload_LogsCritical()
        {
            // Arrange
            var antDeviceCollection = CreateAntDeviceCollection();
            await antDeviceCollection.StartScanning();
            var response = new MockResponse(new ChannelId(1), null);

            // Act
            mockAntChannel.Raise(m => m.ChannelResponse += null, mockAntChannel.Object, response);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("ChannelId or Payload is null")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        //[TestMethod]
        //public void CreateAntDevice_ValidChannelId_CreatesCorrectDevice()
        //{
        //    // Arrange
        //    var antDeviceCollection = CreateAntDeviceCollection();
        //    var channelId = new ChannelId(1) { DeviceType = HeartRate.DeviceClass };
        //    var dataPage = new byte[8];

        //    // Act
        //    var device = antDeviceCollection.CreateAntDevice(channelId, dataPage);

        //    // Assert
        //    Assert.IsInstanceOfType(device, typeof(HeartRate));
        //}

    }
}
