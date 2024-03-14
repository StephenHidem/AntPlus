using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class GeocachePage : ContentPage
{
    public GeocachePage(GeocacheViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}