using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class StrideBasedMonitorPage : ContentPage
{
    public StrideBasedMonitorPage(SDMViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}