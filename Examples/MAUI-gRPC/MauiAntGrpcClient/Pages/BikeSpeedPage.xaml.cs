using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class BikeSpeedPage : ContentPage
{
    public BikeSpeedPage(BikeSpeedViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}