using CommunityToolkit.Mvvm.ComponentModel;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(BikeCadenceSensor), "Sensor")]
    public partial class BikeCadenceViewModel : ObservableObject
    {
        [ObservableProperty]
        private BikeCadenceSensor? bikeCadenceSensor;
    }
}
