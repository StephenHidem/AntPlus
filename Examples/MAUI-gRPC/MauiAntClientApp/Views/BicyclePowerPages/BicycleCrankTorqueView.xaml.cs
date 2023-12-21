using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views.BicyclePowerPages;

public partial class BicycleCrankTorqueView : ContentView
{
    public BicycleCrankTorqueView(BicyclePowerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}