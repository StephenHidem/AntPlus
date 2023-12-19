using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views.FitnessEquipment;

public partial class FitnessEquipmentPage : ContentPage
{
    public FitnessEquipmentPage(FitnessEquipmentViewModel fitnessEquipmentViewModel)
    {
        BindingContext = fitnessEquipmentViewModel;
        InitializeComponent();
    }
}