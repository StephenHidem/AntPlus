using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(StrideSpeedDistanceMonitor), "Sensor")]
    public partial class SDMViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial StrideBasedSpeedAndDistance? StrideSpeedDistanceMonitor { get; set; }

        [RelayCommand]
        private async Task RequestSummary() => _ = await StrideSpeedDistanceMonitor!.RequestSummaryPage();

        [RelayCommand]
        private async Task RequestCapabilities() => _ = await StrideSpeedDistanceMonitor!.RequestBroadcastCapabilities();
    }
}
