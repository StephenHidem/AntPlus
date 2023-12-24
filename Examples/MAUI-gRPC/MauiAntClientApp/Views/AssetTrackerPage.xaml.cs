using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class AssetTrackerPage : ContentPage
{
    public AssetTrackerPage(AssetTrackerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}