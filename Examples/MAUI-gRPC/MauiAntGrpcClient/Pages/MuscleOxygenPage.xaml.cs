using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class MuscleOxygenPage : ContentPage
{
    public MuscleOxygenPage(MuscleOxygenViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}