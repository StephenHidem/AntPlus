using SmallEarthTech.AntPlus.DeviceProfiles.UnknownDevice;

namespace AntPlusUsbClient.ViewModels
{
    internal class UnknownDeviceViewModel
    {
        private readonly UnknownDevice unknownDevice;

        public UnknownDevice UnknownDevice => unknownDevice;

        public UnknownDeviceViewModel(UnknownDevice device)
        {
            unknownDevice = device;
        }
    }
}
