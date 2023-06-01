using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;
using System;

namespace SmallEarthTech.AntUsbStick
{
    /// <summary>This class implements the IAntRadio interface.</summary>
    public partial class AntRadio : IAntRadio, IDisposable
    {
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
        public AntRadio()
        {
            antDevice = new ANT_Device();
            antDevice.deviceResponse += AntDevice_deviceResponse;
        }

        private void AntDevice_deviceResponse(ANT_Response response)
        {
            RadioResponse?.Invoke(this, new UsbAntResponse(response));
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() => antDevice.Dispose();

        /// <inheritdoc/>
        public IAntChannel GetChannel(int num) => new AntChannel(antDevice.getChannel(num));

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
