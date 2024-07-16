using CommunityToolkit.Mvvm.ComponentModel;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(CombinedSpeedAndCadenceSensor), "Sensor")]
    public partial class BikeSpeedAndCadenceViewModel : ObservableObject
    {
        [ObservableProperty]
        private CombinedSpeedAndCadenceSensor? combinedSpeedAndCadenceSensor;
    }
}
