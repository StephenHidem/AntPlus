using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class BicycleParametersView : ContentView
{
    public static readonly BindableProperty BicycleParametersProperty =
        BindableProperty.Create(
            nameof(BicycleParameters),
            typeof(BicyclePowerViewModel),
            typeof(BicycleParametersView));

    public BicyclePowerViewModel BicycleParameters
    {
        get => (BicyclePowerViewModel)GetValue(BicycleParametersProperty);
        set => SetValue(BicycleParametersProperty, value);
    }

    public BicycleParametersView()
    {
        InitializeComponent();
    }
}