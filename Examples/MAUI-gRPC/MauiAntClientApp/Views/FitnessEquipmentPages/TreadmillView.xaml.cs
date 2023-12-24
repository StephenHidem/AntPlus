using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntClientApp.Views.FitnessEquipmentPages;

public partial class TreadmillView : ContentView
{
    public TreadmillView(Treadmill treadmill)
    {
        BindingContext = treadmill;
        InitializeComponent();
    }
}