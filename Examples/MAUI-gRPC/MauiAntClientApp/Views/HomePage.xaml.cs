using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomePageViewModel viewModel)
    {
        BindingContext = viewModel;
        _ = viewModel.SearchAsync();
        InitializeComponent();
    }
}