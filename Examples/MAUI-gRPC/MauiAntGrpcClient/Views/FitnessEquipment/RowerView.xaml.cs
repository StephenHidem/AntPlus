using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntGrpcClient.Views.FitnessEquipment;

public partial class RowerView : ContentView
{
    public RowerView(Rower rower)
    {
        BindingContext = rower;
        InitializeComponent();
    }
}