using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;

namespace MauiAntClientApp.ViewModels
{
    public partial class AssetTrackerViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ILogger<AssetTrackerViewModel> _logger;

        [ObservableProperty]
        private Tracker tracker = null!;
        [ObservableProperty]
        private CommonDataPages commonDataPages = null!;

        public AssetTrackerViewModel(ILogger<AssetTrackerViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("Created AssetTrackerViewModel");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _logger.LogInformation($"{nameof(ApplyQueryAttributes)}");
            Tracker = (Tracker)query["Sensor"];
            CommonDataPages = Tracker.CommonDataPages;
        }
    }
}
