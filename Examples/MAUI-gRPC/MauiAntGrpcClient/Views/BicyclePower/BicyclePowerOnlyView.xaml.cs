using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views.BicyclePower;

public partial class BicyclePowerOnlyView : ContentView
{
    public BicyclePowerOnlyView(BicyclePowerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}