using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views;

public partial class MuscleOxygenPage : ContentPage
{
    public MuscleOxygenPage(MuscleOxygenViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}