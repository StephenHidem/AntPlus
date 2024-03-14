using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class BicyclePowerPage : ContentPage
{
    public BicyclePowerPage(BicyclePowerViewModel bicyclePowerViewModel)
    {
        BindingContext = bicyclePowerViewModel;
        InitializeComponent();
    }
}