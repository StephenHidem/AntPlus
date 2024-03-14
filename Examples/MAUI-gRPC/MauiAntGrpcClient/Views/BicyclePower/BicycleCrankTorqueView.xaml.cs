using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class BicycleCrankTorqueView : ContentView
{
    public BicycleCrankTorqueView(BicyclePowerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}