using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class BikeSpeedPage : ContentPage
{
    public BikeSpeedPage(BikeSpeedViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}