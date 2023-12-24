using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class StrideBasedMonitorPage : ContentPage
{
    public StrideBasedMonitorPage(SDMViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}