using SmallEarthTech.AntPlus.DeviceProfiles;

namespace WpfUsbStickApp.ViewModels
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
