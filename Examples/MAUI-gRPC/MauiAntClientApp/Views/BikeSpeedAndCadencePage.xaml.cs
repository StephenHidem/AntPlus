using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class BikeSpeedAndCadencePage : ContentPage
{
    public BikeSpeedAndCadencePage(BikeSpeedAndCadenceViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}