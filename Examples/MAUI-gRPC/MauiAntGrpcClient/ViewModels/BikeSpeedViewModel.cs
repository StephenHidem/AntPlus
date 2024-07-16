using CommunityToolkit.Mvvm.ComponentModel;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(BikeSpeedSensor), "Sensor")]
    public partial class BikeSpeedViewModel : ObservableObject
    {
        [ObservableProperty]
        private BikeSpeedSensor? bikeSpeedSensor;
    }
}
