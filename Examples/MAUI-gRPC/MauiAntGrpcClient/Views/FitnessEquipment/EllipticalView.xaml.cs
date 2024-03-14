using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntGrpcClient.Views.FitnessEquipment;

public partial class EllipticalView : ContentView
{
    public EllipticalView(Elliptical elliptical)
    {
        BindingContext = elliptical;
        InitializeComponent();
    }
}