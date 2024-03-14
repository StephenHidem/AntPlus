using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class BicycleCalibrationView : ContentView
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(BicyclePowerViewModel), typeof(BicycleCalibrationView));
    public BicyclePowerViewModel ViewModel
    {
        get => (BicyclePowerViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public BicycleCalibrationView()
    {
        InitializeComponent();
    }
}