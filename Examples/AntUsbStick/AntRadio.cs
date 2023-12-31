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
        private readonly ANT_Device _antDevice;
        private IAntChannel[] _channels = new IAntChannel[0];
        private readonly object _lock = new object();

        /// <inheritdoc/>
        public event EventHandler<AntResponse> RadioResponse;

        /// <inheritdoc/>
        public int NumChannels => _antDevice.getNumChannels();

        /// <inheritdoc/>
        public FramerType OpenedFrameType => (FramerType)_antDevice.getOpenedFrameType();

        /// <inheritdoc/>
        public PortType OpenedPortType => (PortType)_antDevice.getOpenedPortType();

        /// <inheritdoc/>
        public uint SerialNumber => _antDevice.getSerialNumber();

        /// <inheritdoc/>
        public void CancelTransfers(int cancelWaitTime) => _antDevice.cancelTransfers(cancelWaitTime);

        /// <summary>Initializes a new instance of the <see cref="AntRadio" /> class.</summary>
        /// <param name="loggerFactory">The factory is used to create <see cref="ILogger"/>s for AntChannels.</param>
        public AntRadio(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            _logger = _loggerFactory.CreateLogger<AntRadio>();
            _antDevice = new ANT_Device();
            _antDevice.deviceResponse += AntDevice_deviceResponse;
            _logger.LogDebug("Created AntRadio #{DeviceNum}", _antDevice.getOpenedUSBDeviceNum());
        }

        private void AntDevice_deviceResponse(ANT_Response response)
        {
            RadioResponse?.Invoke(this, new UsbAntResponse(response));
        }

        /// <inheritdoc/>
        public Task<IAntChannel[]> InitializeContinuousScanMode()
        {
            // multiple clients may attempt to initialize
            lock (_lock)
            {
                // test if channels have not been allocated (first time initialization)
                if (_channels.Length == 0)
                {
                    // allocate channels for this radio
                    _logger.LogInformation("Allocating channels for continuous scan mode.");
                    _channels = new IAntChannel[NumChannels];

                    // configure channel 0 for continuous scan mode
                    SetNetworkKey(0, new byte[] { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 });
                    EnableRxExtendedMessages(true);
                    _channels[0] = GetChannel(0);
                    _channels[0].AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
                    _channels[0].SetChannelID(new ChannelId(0), 500);
                    _channels[0].SetChannelFreq(57, 500);
                    OpenRxScanMode();

                    // assign channels for devices to use for sending messages
                    for (int i = 1; i < NumChannels; i++)
                    {
                        _channels[i] = GetChannel(i);
                        _ = _channels[i].AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
                    }
                }
            }

            return Task.FromResult(_channels);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _logger.LogDebug("Disposed AntRadio #{DeviceNum}", _antDevice.getOpenedUSBDeviceNum());
            _antDevice.Dispose();
        }

        /// <inheritdoc/>
        public IAntChannel GetChannel(int num) => new AntChannel(_antDevice.getChannel(num), _loggerFactory.CreateLogger<AntChannel>());

        /// <inheritdoc/>
        public Task<DeviceCapabilities> GetDeviceCapabilities()
        {
            return Task.FromResult(new UsbDeviceCapabilities(_antDevice.getDeviceCapabilities()) as DeviceCapabilities);
        }

        /// <inheritdoc/>
        public Task<DeviceCapabilities> GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime)
        {
            return Task.FromResult(new UsbDeviceCapabilities(_antDevice.getDeviceCapabilities(forceNewCopy, responseWaitTime)) as DeviceCapabilities);
        }

        /// <inheritdoc/>
        public Task<DeviceCapabilities> GetDeviceCapabilities(uint responseWaitTime)
        {
            return Task.FromResult(new UsbDeviceCapabilities(_antDevice.getDeviceCapabilities(responseWaitTime)) as DeviceCapabilities);
        }

        /// <inheritdoc/>
        public AntResponse ReadUserNvm(ushort address, byte size) => new UsbAntResponse(_antDevice.readUserNvm(address, size));

        /// <inheritdoc/>
        public AntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime) => new UsbAntResponse(_antDevice.readUserNvm(address, size, responseWaitTime));

        /// <inheritdoc/>
        public AntResponse RequestMessageAndResponse(byte channelNum, RequestMessageID messageID, uint responseWaitTime)
        {
            return new UsbAntResponse(_antDevice.requestMessageAndResponse(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID, responseWaitTime));
        }

        /// <inheritdoc/>
        public AntResponse RequestMessageAndResponse(RequestMessageID messageID, uint responseWaitTime)
        {
            return new UsbAntResponse(_antDevice.requestMessageAndResponse((ANT_ReferenceLibrary.RequestMessageID)messageID, responseWaitTime));
        }

        /// <inheritdoc/>
        public bool WriteRawMessageToDevice(byte msgID, byte[] msgData) => _antDevice.writeRawMessageToDevice(msgID, msgData);
    }
}
