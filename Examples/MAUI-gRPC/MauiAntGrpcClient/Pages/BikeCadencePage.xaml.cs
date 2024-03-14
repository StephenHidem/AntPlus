using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class BikeCadencePage : ContentPage
{
    public BikeCadencePage(BikeCadenceViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}