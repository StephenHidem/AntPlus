using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views.GeocachePages;

public partial class GeocachePage : ContentPage
{
    public GeocachePage(GeocacheViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}