using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views;

public partial class HeartRateCapabilitiesView : ContentView
{
    public static readonly BindableProperty HeartRateCapabilitiesProperty =
        BindableProperty.Create(
            nameof(HeartRateCapabilities),
            typeof(HeartRateViewModel),
            typeof(HeartRateCapabilitiesView));

    public HeartRateViewModel HeartRateCapabilities
    {
        get => (HeartRateViewModel)GetValue(HeartRateCapabilitiesProperty);
        set => SetValue(HeartRateCapabilitiesProperty, value);
    }

    public HeartRateCapabilitiesView()
    {
        InitializeComponent();
    }
}