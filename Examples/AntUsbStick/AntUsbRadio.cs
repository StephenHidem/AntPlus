using SmallEarthTech.AntRadioInterface;
using System;

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
        [Obsolete("This method is redundant. Use IAntRadio.ProductDescription. It will be removed in the next release.")]
        public string GetProductDescription() => _antDevice.getDeviceUSBInfo().printProductDescription();

        /// <inheritdoc/>
        [Obsolete("This method is redundant. Use AntRadio.SerialNumber. It will be removed in the next release.")]
        public string GetSerialString() => _antDevice.getDeviceUSBInfo().printSerialString();

        /// <inheritdoc/>
        public void ResetUSB() => _antDevice.ResetUSB();
    }
}
