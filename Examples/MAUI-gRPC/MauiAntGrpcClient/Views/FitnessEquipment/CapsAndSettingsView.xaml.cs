using MauiAntGrpcClient.ViewModels;

namespace MauiAntGrpcClient.Views.FitnessEquipment;

public partial class CapsAndSettingsView : ContentView
{
    public static readonly BindableProperty CapsAndSettingsProperty =
        BindableProperty.Create(
            nameof(CapsAndSettings),
            typeof(FitnessEquipmentViewModel),
            typeof(CapsAndSettingsView));

    public FitnessEquipmentViewModel CapsAndSettings
    {
        get => (FitnessEquipmentViewModel)GetValue(CapsAndSettingsProperty);
        set => SetValue(CapsAndSettingsProperty, value);
    }

    public CapsAndSettingsView()
    {
        InitializeComponent();
    }
}