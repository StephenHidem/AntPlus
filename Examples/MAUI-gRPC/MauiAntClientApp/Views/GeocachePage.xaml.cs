using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class GeocachePage : ContentPage
{
    public GeocachePage(GeocacheViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}