using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntClientApp.Views.FitnessEquipmentPages;

public partial class ClimberView : ContentView
{
    public ClimberView(Climber climber)
    {
        BindingContext = climber;
        InitializeComponent();
    }
}