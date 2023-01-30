using SmallEarthTech.AntPlus.DeviceProfiles.StrideBasedSpeedAndDistance;

namespace AntPlusUsbClient.ViewModels
{
    internal class SDMViewModel
    {
        private readonly StrideBasedSpeedAndDistance sdm;

        public StrideBasedSpeedAndDistance SDM => sdm;

        public SDMViewModel(StrideBasedSpeedAndDistance sdm)
        {
            this.sdm = sdm;
        }
    }
}
