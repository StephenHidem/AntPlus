using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class BicycleCalibrationView : ContentView
{
    public static readonly BindableProperty BicycleCalibrationProperty =
        BindableProperty.Create(
            nameof(BicycleCalibration),
            typeof(BicyclePowerViewModel),
            typeof(BicycleCalibrationView));
    public BicyclePowerViewModel BicycleCalibration
    {
        get => (BicyclePowerViewModel)GetValue(BicycleCalibrationProperty);
        set => SetValue(BicycleCalibrationProperty, value);
    }

    public BicycleCalibrationView()
    {
        InitializeComponent();
    }
}