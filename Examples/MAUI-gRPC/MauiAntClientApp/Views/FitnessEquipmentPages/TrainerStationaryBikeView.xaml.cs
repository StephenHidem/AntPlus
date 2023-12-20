using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;

namespace MauiAntClientApp.Views.FitnessEquipmentPages;

public partial class TrainerStationaryBikeView : ContentView
{
    public TrainerStationaryBikeView(TrainerStationaryBike trainerStationaryBike)
    {
        BindingContext = trainerStationaryBike;
        InitializeComponent();
    }
}