using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;

namespace Hosting.UnitTests
{
    public class AntCollectionTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IAntRadio> _mockAntRadio;
        private readonly Mock<ILogger<AntCollection>> _mockLogger;
        private readonly Mock<IOptions<TimeoutOptions>> _mockOptions;
        private readonly TimeoutOptions _timeoutOptions;
        private readonly AntCollection _antCollection;

        public AntCollectionTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockAntRadio = new Mock<IAntRadio>();
            _mockLogger = new Mock<ILogger<AntCollection>>();
            _mockOptions = new Mock<IOptions<TimeoutOptions>>();
            _timeoutOptions = new TimeoutOptions { Timeout = 2000, MissedMessages = 8 };
            _mockOptions.Setup(o => o.Value).Returns(_timeoutOptions);

            _antCollection = new AntCollection(_mockServiceProvider.Object, _mockAntRadio.Object, _mockLogger.Object, _mockOptions.Object);
        }

        [Fact]
        public async Task StartScanning_ShouldInitializeChannels()
        {
            // Arrange
            var mockChannels = new Mock<IAntChannel>[2];
            for (int i = 0; i < mockChannels.Length; i++)
            {
                mockChannels[i] = new Mock<IAntChannel>();
            }
            _mockAntRadio.Setup(r => r.InitializeContinuousScanMode()).ReturnsAsync(mockChannels.Select(m => m.Object).ToArray());

            // Act
            await _antCollection.StartScanning();

            // Assert
            _mockAntRadio.Verify(r => r.InitializeContinuousScanMode(), Times.Once);
            Assert.NotNull(_antCollection);
        }

        [Fact]
        public void Add_ShouldAddDeviceToCollection()
        {
            // Arrange
            var mockDevice = new Mock<AntDevice>(MockBehavior.Loose, new ChannelId(0), new Mock<IAntChannel>().Object, _mockLogger.Object, _timeoutOptions);

            // Act
            _antCollection.Add(mockDevice.Object);

            // Assert
            Assert.Contains(mockDevice.Object, _antCollection);
        }

        [Fact]
        public void Remove_ShouldRemoveDeviceFromCollection()
        {
            // Arrange
            var mockDevice = new Mock<AntDevice>(MockBehavior.Loose, new ChannelId(0), new Mock<IAntChannel>().Object, _mockLogger.Object, _timeoutOptions);
            _antCollection.Add(mockDevice.Object);

            // Act
            var result = _antCollection.Remove(mockDevice.Object);

            // Assert
            Assert.True(result);
            Assert.DoesNotContain(mockDevice.Object, _antCollection);
        }

        [Fact]
        public async Task Dispose_ShouldReleaseChannels()
        {
            // Arrange
            var mockChannels = new Mock<IAntChannel>[2];
            for (int i = 0; i < mockChannels.Length; i++)
            {
                mockChannels[i] = new Mock<IAntChannel>();
            }
            _mockAntRadio.Setup(r => r.InitializeContinuousScanMode()).ReturnsAsync(mockChannels.Select(m => m.Object).ToArray());
            await _antCollection.StartScanning();

            // Act
            _antCollection.Dispose();

            // Assert
            foreach (var mockChannel in mockChannels)
            {
                mockChannel.Verify(c => c.Dispose(), Times.Once);
            }
        }
    }
}
