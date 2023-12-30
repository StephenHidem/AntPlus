using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomePageViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
        _ = viewModel.SearchAsync();
    }
}