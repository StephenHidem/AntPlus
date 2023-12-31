using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IAntUsbRadio
    {
        /// <inheritdoc/>
        public uint GetBaudRate() => _antDevice.getOpenedUSBBaudRate();

        /// <inheritdoc/>
        public int GetDeviceNum() => _antDevice.getOpenedUSBDeviceNum();

        /// <inheritdoc/>
        public ushort GetPID() => _antDevice.getDeviceUSBPID();

        /// <inheritdoc/>
        public ushort GetVID() => _antDevice.getDeviceUSBVID();

        /// <inheritdoc/>
        public string GetProductDescription() => _antDevice.getDeviceUSBInfo().printProductDescription();

        /// <inheritdoc/>
        public string GetSerialString() => _antDevice.getDeviceUSBInfo().printSerialString();

        /// <inheritdoc/>
        public void ResetUSB() => _antDevice.ResetUSB();
    }
}
