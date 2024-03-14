using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class BikeSpeedAndCadencePage : ContentPage
{
    public BikeSpeedAndCadencePage(BikeSpeedAndCadenceViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}