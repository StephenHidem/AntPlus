using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntClientApp.Views.FitnessEquipment;

public partial class RowerView : ContentView
{
    public RowerView(Rower rower)
    {
        BindingContext = rower;
        InitializeComponent();
    }
}