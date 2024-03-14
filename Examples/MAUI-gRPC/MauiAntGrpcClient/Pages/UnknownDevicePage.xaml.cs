using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class UnknownDevicePage : ContentPage
{
    public UnknownDevicePage(UnknownDeviceViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}