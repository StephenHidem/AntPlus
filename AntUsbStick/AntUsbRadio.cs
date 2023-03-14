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
        public IDeviceInfo GetDeviceUSBInfo()
        {
            return new DeviceInfo(antDevice.getDeviceUSBInfo());
        }

        /// <inheritdoc/>
        public IDeviceInfo GetDeviceUSBInfo(byte deviceNum)
        {
            return new DeviceInfo(antDevice.getDeviceUSBInfo(deviceNum));
        }

        /// <inheritdoc/>
        public ushort GetPID() => antDevice.getDeviceUSBPID();

        /// <inheritdoc/>
        public ushort GetVID() => antDevice.getDeviceUSBVID();

        /// <inheritdoc/>
        public void ResetUSB() => antDevice.ResetUSB();
    }
}
