using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntClientApp.Views.FitnessEquipmentPages;

public partial class EllipticalView : ContentView
{
    public EllipticalView(Elliptical elliptical)
    {
        BindingContext = elliptical;
        InitializeComponent();
    }
}