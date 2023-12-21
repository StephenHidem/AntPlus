using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views.BicyclePowerPages;

public partial class BicyclePowerPage : ContentPage
{
    public BicyclePowerPage(BicyclePowerViewModel bicyclePowerViewModel)
    {
        BindingContext = bicyclePowerViewModel;
        InitializeComponent();
    }
}