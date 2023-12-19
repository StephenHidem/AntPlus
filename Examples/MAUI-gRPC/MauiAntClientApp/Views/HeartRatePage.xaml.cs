using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class HeartRatePage : ContentPage
{
    public HeartRatePage(HeartRateViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}