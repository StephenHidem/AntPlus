using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class PowerView : ContentView
{
    public static readonly BindableProperty PowerProperty =
        BindableProperty.Create(
        nameof(Power),
        typeof(StandardPowerSensor),
        typeof(PowerView)
    );

    public StandardPowerSensor Power
    {
        get => (StandardPowerSensor)GetValue(PowerProperty);
        set => SetValue(PowerProperty, value);
    }

    public PowerView()
    {
        InitializeComponent();
    }
}