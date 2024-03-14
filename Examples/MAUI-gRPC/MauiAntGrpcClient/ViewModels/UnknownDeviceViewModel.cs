using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(UnknownDevice), "Sensor")]
    public partial class UnknownDeviceViewModel : ObservableObject
    {
        private readonly ILogger<UnknownDeviceViewModel> _logger;
        [ObservableProperty]
        private UnknownDevice unknownDevice = null!;

        public UnknownDeviceViewModel(ILogger<UnknownDeviceViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("Created UnknownDeviceViewModel");
        }
    }
}
