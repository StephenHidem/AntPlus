using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntGrpcClient.Views.FitnessEquipment;

public partial class ClimberView : ContentView
{
    public ClimberView(Climber climber)
    {
        BindingContext = climber;
        InitializeComponent();
    }
}