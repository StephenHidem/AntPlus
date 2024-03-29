using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class TEPSView : ContentView
{
    public static readonly BindableProperty TEPSProperty =
        BindableProperty.Create(
            nameof(TEPS),
            typeof(TorqueEffectivenessAndPedalSmoothness),
            typeof(TEPSView));

    public TorqueEffectivenessAndPedalSmoothness TEPS
    {
        get => (TorqueEffectivenessAndPedalSmoothness)GetValue(TEPSProperty);
        set => SetValue(TEPSProperty, value);
    }

    public TEPSView()
    {
        InitializeComponent();
    }
}