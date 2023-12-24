using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;

namespace MauiAntClientApp.Views.BicyclePowerPages;

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