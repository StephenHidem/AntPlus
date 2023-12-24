using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views.FitnessEquipmentPages;

public partial class FitnessEquipmentPage : ContentPage
{
    public FitnessEquipmentPage(FitnessEquipmentViewModel fitnessEquipmentViewModel)
    {
        BindingContext = fitnessEquipmentViewModel;
        InitializeComponent();
    }
}