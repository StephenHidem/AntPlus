using ANT_Managed_Library;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SmallEarthTech.AntRadioInterface;
using System;

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
            _logger.LogDebug("Created AntRadio");
        }

        private void AntDevice_deviceResponse(ANT_Response response)
        {
            RadioResponse?.Invoke(this, new UsbAntResponse(response));
        }

        /// <summary>Initializes the ANT radio for continuous scan mode.</summary>
        /// <returns>Returns the ANT channel used for continuous scan mode. Typically used to receive broadcast messages from ANT master devices.</returns>
        public IAntChannel InitializeContinuousScanMode()
        {
            SetNetworkKey(0, new byte[] { 0xB9, 0xA5, 0x21, 0xFB, 0xBD, 0x72, 0xC3, 0x45 });
            EnableRxExtendedMessages(true);
            IAntChannel antChannel = GetChannel(0);
            antChannel.AssignChannel(ChannelType.BaseSlaveReceive, 0, 500);
            antChannel.SetChannelID(new ChannelId(0), 500);
            antChannel.SetChannelFreq(57, 500);
            OpenRxScanMode();
            return antChannel;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            antDevice.Dispose();
            _logger.LogDebug("Disposed");
        }

        /// <inheritdoc/>
        public IAntChannel GetChannel(int num) => new AntChannel(antDevice.getChannel(num), _loggerFactory.CreateLogger<AntChannel>());

        /// <inheritdoc/>
        public DeviceCapabilities GetDeviceCapabilities()
        {
            return new UsbDeviceCapabilities(antDevice.getDeviceCapabilities());
        }

        /// <inheritdoc/>
        public DeviceCapabilities GetDeviceCapabilities(bool forceNewCopy, uint responseWaitTime)
        {
            return new UsbDeviceCapabilities(antDevice.getDeviceCapabilities(forceNewCopy, responseWaitTime));
        }

        /// <inheritdoc/>
        public DeviceCapabilities GetDeviceCapabilities(uint responseWaitTime)
        {
            return new UsbDeviceCapabilities(antDevice.getDeviceCapabilities(responseWaitTime));
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
