using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class CrankTorqueFrequencyView : ContentView
{
    public CrankTorqueFrequencyView(BicyclePowerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}