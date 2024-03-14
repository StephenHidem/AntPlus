using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class BicycleWheelTorqueView : ContentView
{
    public BicycleWheelTorqueView(BicyclePowerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}