using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.StrideBasedSpeedAndDistance;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class SDMViewModel : ObservableObject
    {
        private readonly StrideBasedSpeedAndDistance sdm;
        public StrideBasedSpeedAndDistance SDM => sdm;

        public SDMViewModel(StrideBasedSpeedAndDistance sdm)
        {
            this.sdm = sdm;
        }

        [RelayCommand]
        void RequestSummary() => SDM.RequestSummaryPage();

        [RelayCommand]
        void RequestCapabilities() => SDM.RequestBroadcastCapabilities();
    }
}
