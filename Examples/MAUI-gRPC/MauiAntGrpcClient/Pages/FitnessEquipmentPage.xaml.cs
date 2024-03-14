using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Pages;

public partial class FitnessEquipmentPage : ContentPage
{
    public FitnessEquipmentPage(FitnessEquipmentViewModel fitnessEquipmentViewModel)
    {
        BindingContext = fitnessEquipmentViewModel;
        InitializeComponent();
    }
}