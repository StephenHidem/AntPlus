using ANT_Managed_Library;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmallEarthTech.AntUsbStick
{
    /// <summary>This class implements the IAntRadio interface.</summary>
    public partial class AntRadio : IAntRadio, IDisposable
    {
        private readonly ILogger<AntRadio> _logger;
        private ANT_Device _antDevice;
        private IAntChannel[] _channels;
        private readonly object _lock = new object();

        /// <inheritdoc/>
        public event EventHandler<AntResponse> RadioResponse;

        /// <inheritdoc/>
        public int NumChannels => _antDevice.getNumChannels();

        /// <inheritdoc/>
        public string ProductDescription => _antDevice.getDeviceUSBInfo().printProductDescription();

        /// <inheritdoc/>
        public uint SerialNumber => _antDevice.getSerialNumber();

        /// <inheritdoc/>
        public string Version => Encoding.Default.GetString(RequestMessageAndResponse(RequestMessageID.Version, 500).Payload).TrimEnd('\0');

        /// <inheritdoc/>
        public void CancelTransfers(int cancelWaitTime) => _antDevice.cancelTransfers(cancelWaitTime);

        /// <summary>Initializes a new instance of the <see cref="AntRadio" /> class.</summary>
        /// <param name="logger">The logger</param>
        public AntRadio(ILogger<AntRadio> logger)
        {
            _logger = logger;
            _antDevice = new ANT_Device();
            _antDevice.deviceResponse += OnDeviceResponse;
            _antDevice.serialError += OnAntDeviceSerialError;
            _logger.LogDebug("Created AntRadio #{DeviceNum}", _antDevice.getOpenedUSBDeviceNum());
        }

        private void OnAntDeviceSerialError(ANT_Device sender, ANT_Device.serialErrorCode error, bool isCritical)
        {
            _logger.LogError("OnAntDeviceSerialError: Sender: {Sender} Error: {Error} Critical: {IsCritical}", sender, error, isCritical);

            // if the error is critical, close the device and reinitialize
            if (isCritical)
            {
                Dispose();
                Thread.Sleep(5000); // wait for the device to close
                _antDevice = new ANT_Device();
                _antDevice.deviceResponse += OnDeviceResponse;
                _antDevice.serialError += OnAntDeviceSerialError;
                _logger.LogWarning("Reinitialized AntRadio #{DeviceNum}", _antDevice.getOpenedUSBDeviceNum());
            }
        }

        private void OnDeviceResponse(ANT_Response response)
        {
            _logger.LogDebug("OnDeviceResponse: {Channel}, {ResponseId}, {Data}", response.antChannel, (MessageId)response.responseID, BitConverter.ToString(response.messageContents));
            RadioResponse?.Invoke(this, new UsbAntResponse(response));
        }

        /// <summary>Initializes the ANT radio for continuous scan mode.</summary>
        /// <returns>
        /// Returns an array of ANT channels.
        /// </returns>
        /// <remarks>
        /// The first element of the array (ANT channel 0) is used for continuous scan mode to receive broadcast messages from ANT master devices.
        /// The remaining channels are configured so messages may be sent to ANT master devices.
        /// </remarks>
        public Task<IAntChannel[]> InitializeContinuousScanMode()
        {
            // multiple clients may attempt to initialize
            lock (_lock)
            {
                // test if channels have not been allocated (first time initialization)
                if (_channels == null)
                {
                    // allocate channels for this radio
                    _logger.LogInformation("Allocating channels for continuous scan mode.");
                    _channels = new IAntChannel[NumChannels];

                    // configure channel 0 for continuous scan mode
                    _ = SetNetworkKey(0, new byte[] { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 });
                    _ = EnableRxExtendedMessages(true);
                    _channels[0] = GetChannel(0);
                    _ = _channels[0].AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
                    _ = _channels[0].SetChannelID(new ChannelId(0), 500);
                    _ = _channels[0].SetChannelFreq(57, 500);
                    _ = OpenRxScanMode();

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

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>This method disposes of the underlying ANT device and its associated resources. It
        /// also unsubscribes from event handlers to prevent memory leaks. After calling this method, the instance
        /// should not be used, as its state becomes undefined.</remarks>
        public void Dispose()
        {
            if (_antDevice != null)
            {
                _logger.LogDebug("Disposed AntRadio #{DeviceNum}", _antDevice.getOpenedUSBDeviceNum());
                _antDevice.deviceResponse -= OnDeviceResponse;
                _antDevice.serialError -= OnAntDeviceSerialError;
                _antDevice.Dispose();
                _antDevice = null;
                _channels = null;
            }
        }

        /// <inheritdoc/>
        public IAntChannel GetChannel(int num) => new AntChannel(_antDevice.getChannel(num), _logger);

        /// <inheritdoc/>
        public Task<DeviceCapabilities> GetDeviceCapabilities(bool forceNewCopy = false, uint responseWaitTime = 1500)
        {
            ANT_DeviceCapabilities deviceCapabilities = _antDevice.getDeviceCapabilities(forceNewCopy, responseWaitTime);
            return Task.FromResult<DeviceCapabilities>(new UsbDeviceCapabilities(deviceCapabilities));
        }

        /// <inheritdoc/>
        public AntResponse ReadUserNvm(ushort address, byte size, uint responseWaitTime = 500) => new UsbAntResponse(_antDevice.readUserNvm(address, size, responseWaitTime));

        /// <inheritdoc/>
        public AntResponse RequestMessageAndResponse(RequestMessageID messageID, uint responseWaitTime, byte channelNum = 0)
        {
            return new UsbAntResponse(_antDevice.requestMessageAndResponse(channelNum, (ANT_ReferenceLibrary.RequestMessageID)messageID, responseWaitTime));
        }

        /// <inheritdoc/>
        public bool WriteRawMessageToDevice(byte msgID, byte[] msgData) => _antDevice.writeRawMessageToDevice(msgID, msgData);
    }
}
