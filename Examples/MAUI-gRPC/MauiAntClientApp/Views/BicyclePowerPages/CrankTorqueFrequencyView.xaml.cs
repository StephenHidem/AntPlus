using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views.BicyclePowerPages;

public partial class CrankTorqueFrequencyView : ContentView
{
    public CrankTorqueFrequencyView(BicyclePowerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}