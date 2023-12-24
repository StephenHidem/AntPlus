using MauiAntClientApp.ViewModels;

namespace MauiAntClientApp.Views.FitnessEquipmentPages;

public partial class CapsAndSettingsView : ContentView
{
    public static readonly BindableProperty ViewModelProperty =
        BindableProperty.Create(nameof(ViewModel), typeof(FitnessEquipmentViewModel), typeof(CapsAndSettingsView));
    public FitnessEquipmentViewModel ViewModel
    {
        get => (FitnessEquipmentViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public CapsAndSettingsView()
    {
        InitializeComponent();
    }
}