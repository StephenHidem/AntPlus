using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class RadioCapabilitiesPage : ContentPage
{
    public RadioCapabilitiesPage(RadioCapabilitiesViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}