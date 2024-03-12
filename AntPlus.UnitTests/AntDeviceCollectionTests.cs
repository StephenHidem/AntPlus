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

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Loose);

            mockAntRadio = mockRepository.Create<IAntRadio>();
            mockAntChannel = mockRepository.Create<IAntChannel>();
        }

        private AntDeviceCollection CreateAntDeviceCollection()
        {
            IAntChannel[] mockChannels = new IAntChannel[8];
            Array.Fill(mockChannels, mockAntChannel.Object);
            mockAntRadio.Setup(r => r.InitializeContinuousScanMode().Result).Returns(mockChannels);
            AntDeviceCollection adc = new(
                mockAntRadio.Object,
                null,
                3000);
            Thread.Sleep(2000);     // allow time for background initialization task to complete
            return adc;
        }

        [TestMethod]
        public void MultithreadedAdd_Collection_ExpectedCount()
        {
            // Arrange
            Mock<ILogger<UnknownDevice>> mockLogger = new();
            var antDeviceCollection = CreateAntDeviceCollection();
            int numberOfDevices = 16;
            using SemaphoreSlim semaphore = new(0, numberOfDevices);
            Task[] tasks = new Task[numberOfDevices];
            for (int i = 0; i < numberOfDevices; i++)
            {
                Mock<AntDevice> antDevice = new(new ChannelId((uint)i), mockAntChannel.Object, mockLogger.Object, 500);
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
        public void MultithreadedRemove_Collection_ExpectedCount()
        {
            // Arrange
            Mock<ILogger<UnknownDevice>> mockLogger = new();
            var antDeviceCollection = CreateAntDeviceCollection();
            int numberOfDevices = 16;
            using SemaphoreSlim semaphore = new(0, numberOfDevices);
            Task[] tasks = new Task[numberOfDevices];
            for (int i = 0; i < numberOfDevices; i++)
            {
                Mock<AntDevice> antDevice = new(new ChannelId((uint)i), mockAntChannel.Object, mockLogger.Object, 500);
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
        [DataRow(Bicycle.DeviceClass, typeof(Bicycle))]
        [DataRow(BikeSpeedSensor.DeviceClass, typeof(BikeSpeedSensor))]
        [DataRow(BikeCadenceSensor.DeviceClass, typeof(BikeCadenceSensor))]
        [DataRow(CombinedSpeedAndCadenceSensor.DeviceClass, typeof(CombinedSpeedAndCadenceSensor))]
        [DataRow(Equipment.DeviceClass, typeof(Equipment))]
        [DataRow(MuscleOxygen.DeviceClass, typeof(MuscleOxygen))]
        [DataRow(Geocache.DeviceClass, typeof(Geocache))]
        [DataRow(Tracker.DeviceClass, typeof(Tracker))]
        [DataRow(StrideBasedSpeedAndDistance.DeviceClass, typeof(StrideBasedSpeedAndDistance))]
        public void ChannelResponseEvent_Collection_ExpectedDeviceInCollection(byte deviceClass, Type deviceType)
        {
            // Arrange
            byte[] id = new byte[4] { 1, 0, deviceClass, 0 };
            ChannelId cid = new(BitConverter.ToUInt32(id));
            mockAntChannel.SetupAdd(m => m.ChannelResponse += It.IsAny<EventHandler<AntResponse>>());
            mockAntChannel.SetupRemove(m => m.ChannelResponse -= It.IsAny<EventHandler<AntResponse>>());
            var mockResponse = new MockResponse(cid, new byte[8]);
            var antDeviceCollection = CreateAntDeviceCollection();

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
    }
}
