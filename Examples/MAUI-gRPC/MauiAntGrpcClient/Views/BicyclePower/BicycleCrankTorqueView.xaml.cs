using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class BicycleCrankTorqueView : ContentView
{
    public BicycleCrankTorqueView(StandardCrankTorqueSensor sensor)
    {
        BindingContext = sensor;
        InitializeComponent();
    }
}