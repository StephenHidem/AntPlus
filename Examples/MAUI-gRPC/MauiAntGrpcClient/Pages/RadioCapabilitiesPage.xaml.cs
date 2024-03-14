using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class RadioCapabilitiesPage : ContentPage
{
    public RadioCapabilitiesPage(RadioCapabilitiesViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}