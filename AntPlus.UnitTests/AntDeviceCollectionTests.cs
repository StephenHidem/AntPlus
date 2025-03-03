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
        private readonly MockRepository _mockRepository;
        private readonly Mock<IAntRadio> _mockAntRadio;
        private readonly Mock<IAntChannel> _mockAntChannel;
        private readonly Mock<ILogger> _mockLogger;

        private readonly AntDeviceCollection _antDevices;

        public AntDeviceCollectionTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);

            _mockAntRadio = _mockRepository.Create<IAntRadio>();
            _mockAntChannel = _mockRepository.Create<IAntChannel>();
            _mockLogger = _mockRepository.Create<ILogger>();
            _mockLogger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            var mockLoggerFactory = _mockRepository.Create<ILoggerFactory>();
            mockLoggerFactory.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            var mockChannels = new Mock<IAntChannel>[8];
            Array.Fill(mockChannels, _mockAntChannel);
            _mockAntRadio.Setup(r => r.InitializeContinuousScanMode()).ReturnsAsync([.. mockChannels.Select(m => m.Object)]);
            _antDevices = new(_mockAntRadio.Object, mockLoggerFactory.Object);
        }

        [Fact]
        public async Task MultithreadedAdd_ExpectedCount()
        {
            // Arrange
            await _antDevices.StartScanning();
            int numberOfDevices = 16;
            using SemaphoreSlim semaphore = new(0, numberOfDevices);
            Task[] tasks = new Task[numberOfDevices];
            for (int i = 0; i < numberOfDevices; i++)
            {
                Mock<AntDevice> antDevice = new(new ChannelId((uint)i), _mockAntChannel.Object, _mockLogger.Object, (int)500);
                tasks[i] = Task.Run(() =>
                {
                    semaphore.Wait();
                    _antDevices.Add(antDevice.Object);
                });
            }

            // Act
            semaphore.Release(numberOfDevices);
            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(numberOfDevices, _antDevices.Count);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public async Task MultithreadedRemove_ExpectedCount()
        {
            // Arrange
            await _antDevices.StartScanning();
            int numberOfDevices = 16;
            using SemaphoreSlim semaphore = new(0, numberOfDevices);
            Task[] tasks = new Task[numberOfDevices];
            for (int i = 0; i < numberOfDevices; i++)
            {
                Mock<AntDevice> antDevice = new(new ChannelId((uint)i), _mockAntChannel.Object, _mockLogger.Object, (int)500);
                _antDevices.Add(antDevice.Object);
                tasks[i] = Task.Run(() =>
                {
                    semaphore.Wait();
                    _ = _antDevices.Remove(antDevice.Object);
                });
            }

            // Act
            semaphore.Release(numberOfDevices);
            await Task.WhenAll(tasks);

            // Assert
            Assert.Empty(_antDevices);
            _mockRepository.VerifyAll();
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
            _mockAntChannel.SetupAdd(m => m.ChannelResponse += It.IsAny<EventHandler<AntResponse>>());
            _mockAntChannel.SetupRemove(m => m.ChannelResponse -= It.IsAny<EventHandler<AntResponse>>());
            var mockResponse = new MockResponse(MessageId.BroadcastData, cid, new byte[8]);
            await _antDevices.StartScanning();

            // Act
            _mockAntChannel.Raise(m => m.ChannelResponse += null, _mockAntChannel.Object, mockResponse);

            Assert.Single(_antDevices);
            Assert.Equal(deviceType, _antDevices[0].GetType());
        }

        class MockResponse : AntResponse
        {
            public MockResponse(MessageId responseId, ChannelId channelId, byte[] payload)
            {
                ResponseId = responseId;
                ChannelId = channelId;
                Payload = payload;
            }
        }

        [Fact]
        public async Task MessageHandler_NullChannelId_LogsWarning()
        {
            // Arrange
            await _antDevices.StartScanning();
            var response = new MockResponse(MessageId.BroadcastData, null, new byte[8]);

            // Act
            _mockAntChannel.Raise(m => m.ChannelResponse += null, _mockAntChannel.Object, response);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled ANT response.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task MessageHandler_NullPayload_LogsWarning()
        {
            // Arrange
            await _antDevices.StartScanning();
            var response = new MockResponse(MessageId.BroadcastData, new ChannelId(1), null);

            // Act
            _mockAntChannel.Raise(m => m.ChannelResponse += null, _mockAntChannel.Object, response);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled ANT response.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
