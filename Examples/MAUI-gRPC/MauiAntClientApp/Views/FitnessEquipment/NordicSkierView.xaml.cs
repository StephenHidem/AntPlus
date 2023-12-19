using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntClientApp.Views.FitnessEquipment;

public partial class NordicSkierView : ContentView
{
    public NordicSkierView(NordicSkier nordicSkier)
    {
        BindingContext = nordicSkier;
        InitializeComponent();
    }
}