using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    public partial class AntRadio : IAntUsbRadio
    {
        /// <inheritdoc/>
        public uint GetBaudRate() => antDevice.getOpenedUSBBaudRate();

        /// <inheritdoc/>
        public int GetDeviceNum() => antDevice.getOpenedUSBDeviceNum();

        /// <inheritdoc/>
        public ushort GetPID() => antDevice.getDeviceUSBPID();

        /// <inheritdoc/>
        public ushort GetVID() => antDevice.getDeviceUSBVID();

        /// <inheritdoc/>
        public string GetProductDescription() => antDevice.getDeviceUSBInfo().printProductDescription();

        /// <inheritdoc/>
        public string GetSerialString() => antDevice.getDeviceUSBInfo().printSerialString();

        /// <inheritdoc/>
        public void ResetUSB() => antDevice.ResetUSB();
    }
}
