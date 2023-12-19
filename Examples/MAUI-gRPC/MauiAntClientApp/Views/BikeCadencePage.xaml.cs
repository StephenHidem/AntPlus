using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class BikeCadencePage : ContentPage
{
    public BikeCadencePage(BikeCadenceViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}