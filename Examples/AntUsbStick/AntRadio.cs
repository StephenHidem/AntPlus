using ANT_Managed_Library;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Threading.Tasks;

namespace SmallEarthTech.AntUsbStick
{
    /// <summary>This class implements the IAntRadio interface.</summary>
    public partial class AntRadio : IAntRadio, IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<IAntRadio> _logger;
        private readonly ANT_Device antDevice;

        /// <inheritdoc/>
        public event EventHandler<AntResponse> RadioResponse;

        /// <inheritdoc/>
        public int NumChannels => antDevice.getNumChannels();

        /// <inheritdoc/>
        public FramerType OpenedFrameType => (FramerType)antDevice.getOpenedFrameType();

        /// <inheritdoc/>
        public PortType OpenedPortType => (PortType)antDevice.getOpenedPortType();

        /// <inheritdoc/>
        public uint SerialNumber => antDevice.getSerialNumber();

        /// <inheritdoc/>
        public void CancelTransfers(int cancelWaitTime) => antDevice.cancelTransfers(cancelWaitTime);

        /// <summary>Initializes a new instance of the <see cref="AntRadio" /> class.</summary>
        /// <param name="loggerFactory">The factory is used to create <see cref="ILogger"/>s for AntChannels.</param>
        public AntRadio(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            _logger = _loggerFactory.CreateLogger<AntRadio>();
            antDevice = new ANT_Device();
            antDevice.deviceResponse += AntDevice_deviceResponse;
            _logger.LogDebug("Created AntRadio #{DeviceNum}", antDevice.getOpenedUSBDeviceNum());
        }

        private void AntDevice_deviceResponse(ANT_Response response)
        {
            RadioResponse?.Invoke(this, new UsbAntResponse(response));
        }

        /// <inheritdoc/>
        public Task<IAntChannel[]> InitializeContinuousScanMode()
        {
            IAntChannel[] channels = new IAntChannel[NumChannels];

            // configure channel 0 for continuous scan mode
            SetNetworkKey(0, new byte[] { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 });
            EnableRxExtendedMessages(true);
            channels[0] = GetChannel(0);
            channels[0].AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
            channels[0].SetChannelID(new ChannelId(0), 500);
            channels[0].SetChannelFreq(57, 500);
            OpenRxScanMode();

            // assign channels for devices to use for sending messages
            for (int i = 1; i < NumChannels; i++)
            {
                channels[i] = GetChannel(i);
                _ = channels[i].AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
            }

            return Task.FromResult(channels);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _logger.LogDebug("Disposed AntRadio #{DeviceNum}", antDevice.getOpenedUSBDeviceNum());
            antDevice.Dispose();
        }

        /// <inheritdoc/>
        public IAntChannel GetChannel(int num) => new AntChannel(antDevice.getChannel(num), _loggerFactory.CreateLogger<AntChannel>());

        /// <inheritdoc/>
        public Task<DeviceCapabilities> GetDeviceCapabilities()
        {
            return Task.FromResult(new UsbDeviceCapabilities(antDevice.getDeviceCapabilities()) as DeviceCapabilities);
        }

        /// <inheritdoc/>
        public Task<DeviceCapabilities> GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime)
        {
            return Task.FromResult(new UsbDeviceCapabilities(antDevice.getDeviceCapabilities(forceNewCopy, responseWaitTime)) as DeviceCapabilities);
        }

        /// <inheritdoc/>
        public Task<DeviceCapabilities> GetDeviceCapabilities(uint responseWaitTime)
        {
            return Task.FromResult(new UsbDeviceCapabilities(antDevice.getDeviceCapabilities(responseWaitTime)) as DeviceCapabilities);
        }

        /// <inheritdoc/>
        public AntResponse ReadUserNvm(ushort address, byte size) => new UsbAntResponse(antDevice.readUserNvm(address, size));

        /// <inheritdoc/>
        public AntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime) => new UsbAntResponse(antDevice.readUserNvm(address, size, responseWaitTime));

        /// <inheritdoc/>
        public AntResponse RequestMessageAndResponse(byte channelNum, RequestMessageID messageID, uint responseWaitTime)
        {
            return new UsbAntResponse(antDevice.requestMessageAndResponse(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID, responseWaitTime));
        }

        /// <inheritdoc/>
        public AntResponse RequestMessageAndResponse(RequestMessageID messageID, uint responseWaitTime)
        {
            return new UsbAntResponse(antDevice.requestMessageAndResponse((ANT_ReferenceLibrary.RequestMessageID)messageID, responseWaitTime));
        }

        /// <inheritdoc/>
        public bool WriteRawMessageToDevice(byte msgID, byte[] msgData) => antDevice.writeRawMessageToDevice(msgID, msgData);
    }
}
