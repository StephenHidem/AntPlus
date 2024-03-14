using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(CombinedSpeedAndCadenceSensor), "Sensor")]
    public partial class BikeSpeedAndCadenceViewModel : ObservableObject
    {
        private readonly ILogger<BikeSpeedAndCadenceViewModel> _logger;
        [ObservableProperty]
        private CombinedSpeedAndCadenceSensor combinedSpeedAndCadenceSensor = null!;

        public BikeSpeedAndCadenceViewModel(ILogger<BikeSpeedAndCadenceViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("Created BikeSpeedAndCadenceViewModel");
        }
    }
}
