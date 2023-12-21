using MauiAntClientApp.Views;
using MauiAntClientApp.Views.BicyclePowerPages;
using MauiAntClientApp.Views.FitnessEquipmentPages;
using MauiAntClientApp.Views.GeocachePages;
using MauiAntClientApp.Views.HeartRatePages;

namespace MauiAntClientApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // initialize routing
            Routing.RegisterRoute("AssetTracker", typeof(AssetTrackerPage));
            Routing.RegisterRoute("BicyclePower", typeof(BicyclePowerPage));
            Routing.RegisterRoute("BikeCadence", typeof(BikeCadencePage));
            Routing.RegisterRoute("BikeSpeed", typeof(BikeSpeedPage));
            Routing.RegisterRoute("SpeedAndCadence", typeof(BikeSpeedAndCadencePage));
            Routing.RegisterRoute("FitnessEquipment", typeof(FitnessEquipmentPage));
            Routing.RegisterRoute("Geocache", typeof(GeocachePage));
            Routing.RegisterRoute("HeartRate", typeof(HeartRatePage));
            Routing.RegisterRoute("MuscleOxygen", typeof(MuscleOxygenPage));
            Routing.RegisterRoute("StrideBasedMonitor", typeof(StrideBasedMonitorPage));
            Routing.RegisterRoute("UnknownDevice", typeof(UnknownDevicePage));
        }
    }
}
