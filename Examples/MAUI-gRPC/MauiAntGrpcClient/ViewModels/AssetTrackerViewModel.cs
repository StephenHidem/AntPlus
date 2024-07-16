using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class AssetTrackerViewModel(ILogger<AssetTrackerViewModel> logger) : ObservableObject, IQueryAttributable
    {
        private readonly ILogger<AssetTrackerViewModel> _logger = logger;

        [ObservableProperty]
        private Tracker? tracker;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _logger.LogInformation($"{nameof(ApplyQueryAttributes)}");
            Tracker = (Tracker)query["Sensor"];
        }
    }
}
