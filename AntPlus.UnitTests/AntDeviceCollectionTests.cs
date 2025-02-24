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
using Xunit;

namespace AntPlus.UnitTests
{
    public class AntDeviceCollectionTests
    {
        private readonly MockRepository mockRepository;

        private readonly Mock<IAntRadio> mockAntRadio;
        private readonly Mock<IAntChannel> mockAntChannel;
        private readonly Mock<ILogger> mockLogger;
        private readonly Mock<ILoggerFactory> mockLoggerFactory;

        public AntDeviceCollectionTests()
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

        [Fact]
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
            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(numberOfDevices, antDeviceCollection.Count);
            mockRepository.VerifyAll();
        }

        [Fact]
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
            await Task.WhenAll(tasks);

            // Assert
            Assert.Empty(antDeviceCollection);
            mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(HeartRate.DeviceClass, typeof(HeartRate))]
        [InlineData(BicyclePower.DeviceClass, typeof(StandardPowerSensor))]
        [InlineData(BikeSpeedSensor.DeviceClass, typeof(BikeSpeedSensor))]
        [InlineData(BikeCadenceSensor.DeviceClass, typeof(BikeCadenceSensor))]
        [InlineData(CombinedSpeedAndCadenceSensor.DeviceClass, typeof(CombinedSpeedAndCadenceSensor))]
        [InlineData(FitnessEquipment.DeviceClass, typeof(UnknownDevice))]
        [InlineData(MuscleOxygen.DeviceClass, typeof(MuscleOxygen))]
        [InlineData(Geocache.DeviceClass, typeof(Geocache))]
        [InlineData(Tracker.DeviceClass, typeof(Tracker))]
        [InlineData(StrideBasedSpeedAndDistance.DeviceClass, typeof(StrideBasedSpeedAndDistance))]
        public async Task ChannelResponseEvent_Collection_ExpectedDeviceInCollection(byte deviceClass, Type deviceType)
        {
            // Arrange
            byte[] id = [1, 0, deviceClass, 0];
            ChannelId cid = new(BitConverter.ToUInt32(id));
            mockAntChannel.SetupAdd(m => m.ChannelResponse += It.IsAny<EventHandler<AntResponse>>());
            mockAntChannel.SetupRemove(m => m.ChannelResponse -= It.IsAny<EventHandler<AntResponse>>());
            var mockResponse = new MockResponse(cid, new byte[8]);
            var antDeviceCollection = CreateAntDeviceCollection();
            await antDeviceCollection.StartScanning();

            // Act
            mockAntChannel.Raise(m => m.ChannelResponse += null, mockAntChannel.Object, mockResponse);

            Assert.Single(antDeviceCollection);
            Assert.Equal(deviceType, antDeviceCollection[0].GetType());
        }

        class MockResponse : AntResponse
        {
            public MockResponse(ChannelId channelId, byte[] payload)
            {
                ChannelId = channelId;
                Payload = payload;
            }
        }

        [Fact]
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

        [Fact]
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
    }
}
