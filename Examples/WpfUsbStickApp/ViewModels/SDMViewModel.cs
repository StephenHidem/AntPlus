using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class SDMViewModel : ObservableObject
    {
        public StrideBasedSpeedAndDistance SDM { get; }
        public CommonDataPages CommonDataPages => SDM.CommonDataPages;

        public SDMViewModel(StrideBasedSpeedAndDistance sdm)
        {
            SDM = sdm;
        }

        [RelayCommand]
        void RequestSummary() => SDM.RequestSummaryPage();

        [RelayCommand]
        void RequestCapabilities() => SDM.RequestBroadcastCapabilities();
    }
}
