using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class BicycleWheelTorqueView : ContentView
{
    public BicycleWheelTorqueView(StandardWheelTorqueSensor sensor)
    {
        BindingContext = sensor;
        InitializeComponent();
    }
}