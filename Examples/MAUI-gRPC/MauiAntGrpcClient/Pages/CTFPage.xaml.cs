using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class CTFPage : ContentPage
{
    public CTFPage(CTFViewModel ctfViewModel)
    {
        BindingContext = ctfViewModel;
        InitializeComponent();
    }
}