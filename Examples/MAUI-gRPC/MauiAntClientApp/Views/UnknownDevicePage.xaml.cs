using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class UnknownDevicePage : ContentPage
{
    public UnknownDevicePage(UnknownDeviceViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}