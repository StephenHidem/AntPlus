using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntClientApp.ViewModels
{
    public partial class SDMViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ILogger<SDMViewModel> _logger;
        [ObservableProperty]
        private StrideBasedSpeedAndDistance sDM = null!;
        [ObservableProperty]
        private CommonDataPages commonDataPages = null!;

        public SDMViewModel(ILogger<SDMViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("Created SDMViewModel");
        }

        [RelayCommand]
        private async Task<MessagingReturnCode> RequestSummary() => await SDM.RequestSummaryPage();

        [RelayCommand]
        private async Task<MessagingReturnCode> RequestCapabilities() => await SDM.RequestBroadcastCapabilities();

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _logger.LogInformation($"{nameof(ApplyQueryAttributes)}");
            SDM = (StrideBasedSpeedAndDistance)query["Sensor"];
            CommonDataPages = SDM.CommonDataPages;
        }
    }
}
