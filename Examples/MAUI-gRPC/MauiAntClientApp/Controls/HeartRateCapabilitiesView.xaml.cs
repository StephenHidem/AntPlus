using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Controls;

public partial class HeartRateCapabilitiesView : ContentView
{
    public static readonly BindableProperty HeartRateViewModelProperty =
    BindableProperty.Create(nameof(HeartRateViewModel), typeof(HeartRateViewModel), typeof(HeartRateCapabilitiesView));
    public HeartRateViewModel HeartRateViewModel
    {
        get => (HeartRateViewModel)GetValue(HeartRateViewModelProperty);
        set => SetValue(HeartRateViewModelProperty, value);
    }

    public HeartRateCapabilitiesView()
    {
        InitializeComponent();
    }
}