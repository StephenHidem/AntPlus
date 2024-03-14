using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class HeartRatePage : ContentPage
{
    public HeartRatePage(HeartRateViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}