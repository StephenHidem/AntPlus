using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class AssetTrackerPage : ContentPage
{
    public AssetTrackerPage(AssetTrackerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}