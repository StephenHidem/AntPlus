using ANT_Managed_Library;
using SmallEarthTech.AntRadioInterface;

namespace SmallEarthTech.AntUsbStick
{
    /// <summary>The USB stick device information class.</summary>
    public class DeviceInfo : IDeviceInfo
    {
        private readonly ANT_DeviceInfo deviceInfo;

        /// <inheritdoe/>
        public string ProductDescription => deviceInfo.printProductDescription();

        /// <inheritdoe/>
        public string SerialString => deviceInfo.printSerialString();

        /// <summary>Initializes a new instance of the <see cref="DeviceInfo" /> class.</summary>
        /// <param name="deviceInfo">USB stick device info.</param>
        public DeviceInfo(ANT_DeviceInfo deviceInfo)
        {
            this.deviceInfo = deviceInfo;
        }
    }
}
