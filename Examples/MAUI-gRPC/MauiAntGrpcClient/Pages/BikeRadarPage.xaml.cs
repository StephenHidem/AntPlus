using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class BikeRadarPage : ContentPage
{
    public BikeRadarPage(BikeRadarViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}
