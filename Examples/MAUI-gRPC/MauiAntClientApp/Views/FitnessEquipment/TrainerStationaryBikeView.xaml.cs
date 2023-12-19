using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntClientApp.Views.FitnessEquipment;

public partial class TrainerStationaryBikeView : ContentView
{
    public TrainerStationaryBikeView(TrainerStationaryBike trainerStationaryBike)
    {
        BindingContext = trainerStationaryBike;
        InitializeComponent();
    }
}