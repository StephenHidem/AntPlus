using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(BikeSpeedSensor), "Sensor")]
    public partial class BikeSpeedViewModel : ObservableObject
    {
        private readonly ILogger<BikeSpeedViewModel> _logger;
        [ObservableProperty]
        private BikeSpeedSensor bikeSpeedSensor = null!;

        public BikeSpeedViewModel(ILogger<BikeSpeedViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("Created BikeSpeedViewModel");
        }
    }
}
