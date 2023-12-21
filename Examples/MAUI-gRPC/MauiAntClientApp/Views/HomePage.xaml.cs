using MauiAntClientApp.ViewModels;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntClientApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _ = viewModel.SearchAsync();
    }

    private async void Details_Clicked(object sender, EventArgs e)
    {
        switch ((AntDevice)antDevices.SelectedItem)
        {
            case Tracker:
                await Shell.Current.GoToAsync("AssetTracker", new Dictionary<string, object> {
                    { "Sensor", (Tracker)antDevices.SelectedItem },
                });
                break;
            case Bicycle:
                await Shell.Current.GoToAsync("BicyclePower", new Dictionary<string, object> {
                    { "Sensor", (Bicycle)antDevices.SelectedItem },
                });
                break;
            case BikeSpeedSensor:
                await Shell.Current.GoToAsync("BikeSpeed", new Dictionary<string, object> {
                    { "Sensor", (BikeSpeedSensor)antDevices.SelectedItem }
                });
                break;
            case CombinedSpeedAndCadenceSensor:
                await Shell.Current.GoToAsync("SpeedAndCadence", new Dictionary<string, object> {
                    { "Sensor", (CombinedSpeedAndCadenceSensor)antDevices.SelectedItem }
                });
                break;
            case BikeCadenceSensor:
                await Shell.Current.GoToAsync("BikeCadence", new Dictionary<string, object> {
                    { "Sensor", (BikeCadenceSensor)antDevices.SelectedItem }
                });
                break;
            case Equipment:
                await Shell.Current.GoToAsync("FitnessEquipment", new Dictionary<string, object> {
                    { "Sensor", (Equipment)antDevices.SelectedItem }
                });
                break;
            case Geocache:
                await Shell.Current.GoToAsync("Geocache", new Dictionary<string, object> {
                    { "Sensor", (Geocache)antDevices.SelectedItem }
                });
                break;
            case HeartRate:
                await Shell.Current.GoToAsync("HeartRate", new Dictionary<string, object> {
                    { "Sensor", (HeartRate)antDevices.SelectedItem }
                });
                break;
            case MuscleOxygen:
                await Shell.Current.GoToAsync("MuscleOxygen", new Dictionary<string, object> {
                    { "Sensor", (MuscleOxygen)antDevices.SelectedItem }
                });
                break;
            case StrideBasedSpeedAndDistance:
                await Shell.Current.GoToAsync("StrideBasedMonitor", new Dictionary<string, object> {
                    { "Sensor", (StrideBasedSpeedAndDistance)antDevices.SelectedItem }
                });
                break;
            case UnknownDevice:
                await Shell.Current.GoToAsync("UnknownDevice", new Dictionary<string, object> {
                    { "Sensor", (UnknownDevice)antDevices.SelectedItem }
                });
                break;
            default:
                return;
        }
    }
}