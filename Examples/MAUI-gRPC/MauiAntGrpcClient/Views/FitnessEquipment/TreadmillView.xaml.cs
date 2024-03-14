using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntGrpcClient.Views.FitnessEquipment;

public partial class TreadmillView : ContentView
{
    public TreadmillView(Treadmill treadmill)
    {
        BindingContext = treadmill;
        InitializeComponent();
    }
}