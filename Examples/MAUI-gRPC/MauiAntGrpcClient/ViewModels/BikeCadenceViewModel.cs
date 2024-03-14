using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(BikeCadenceSensor), "Sensor")]
    public partial class BikeCadenceViewModel : ObservableObject
    {
        private readonly ILogger<BikeCadenceViewModel> _logger;
        [ObservableProperty]
        private BikeCadenceSensor bikeCadenceSensor = null!;

        public BikeCadenceViewModel(ILogger<BikeCadenceViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("Created BikeCadenceViewModel");
        }
    }
}
