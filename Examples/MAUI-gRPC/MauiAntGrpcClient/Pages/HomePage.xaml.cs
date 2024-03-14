using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class HomePage : ContentPage
{
    public HomePage(HomePageViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
        _ = viewModel.SearchAsync();
    }
}