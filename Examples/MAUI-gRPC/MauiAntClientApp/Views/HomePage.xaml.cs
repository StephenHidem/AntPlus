using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _ = viewModel.SearchAsync();
    }
}