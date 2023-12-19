using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System.Threading.Tasks;

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
        private async Task<MessagingReturnCode> RequestSummary() => await SDM.RequestSummaryPage();

        [RelayCommand]
        private async Task<MessagingReturnCode> RequestCapabilities() => await SDM.RequestBroadcastCapabilities();
    }
}
