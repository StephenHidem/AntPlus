using AntPlus;
using AntPlus.DeviceProfiles.AssetTracker;
using AntPlus.DeviceProfiles.BicyclePower;
using AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using AntPlus.DeviceProfiles.FitnessEquipment;
using AntPlus.DeviceProfiles.Geocache;
using AntPlus.DeviceProfiles.HeartRate;
using AntPlus.DeviceProfiles.MuscleOxygen;
using AntPlus.DeviceProfiles.UnknownDevice;
using AntPlusUsbClient.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AntPlusUsbClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(App.AntDevices, App.AntDevices.collectionLock);
            DataContext = App.AntDevices;
            antDevices.MouseDoubleClick += AntDevices_MouseDoubleClick;
        }

        private void AntDevices_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AntDevice antDevice = ((ListView)sender).SelectedItem as AntDevice;
            switch (antDevice.ChannelId.DeviceType)
            {
                case HeartRate.DeviceClass:
                    HeartRateWindow heartRateWindow = new HeartRateWindow((HeartRate)antDevice);
                    heartRateWindow.Show();
                    break;
                case BicyclePower.DeviceClass:
                    BicyclePowerWindow bpWindow = new BicyclePowerWindow((BicyclePower)antDevice);
                    bpWindow.Show();
                    break;
                case BikeSpeedSensor.DeviceClass:
                    BikeSpeedWindow bikeSpeedWindow = new BikeSpeedWindow((BikeSpeedSensor)antDevice);
                    bikeSpeedWindow.Show();
                    break;
                case BikeCadenceSensor.DeviceClass:
                    BikeCadenceWindow bikeCadenceWindow = new BikeCadenceWindow((BikeCadenceSensor)antDevice);
                    bikeCadenceWindow.Show();
                    break;
                case CombinedSpeedAndCadenceSensor.DeviceClass:
                    BikeSpeedAndCadenceWindow speedAndCadenceWindow = new BikeSpeedAndCadenceWindow((CombinedSpeedAndCadenceSensor)antDevice);
                    speedAndCadenceWindow.Show();
                    break;
                case FitnessEquipment.DeviceClass:
                    FitnessEquipmentWindow fitnessEquipmentWindow = new FitnessEquipmentWindow((FitnessEquipment)antDevice);
                    fitnessEquipmentWindow.Show();
                    break;
                case MuscleOxygen.DeviceClass:
                    MuscleOxygenWindow muscleOxygenWindow = new MuscleOxygenWindow((MuscleOxygen)antDevice);
                    muscleOxygenWindow.Show();
                    break;
                case Geocache.DeviceClass:
                    GeocacheWindow geocacheWindow = new GeocacheWindow((Geocache)antDevice);
                    geocacheWindow.Show();
                    break;
                case AssetTracker.DeviceClass:
                    AssetTrackerWindow tracker = new AssetTrackerWindow((AssetTracker)antDevice);
                    tracker.Show();
                    break;
                default:
                    // unknown device
                    UnknownDeviceWindow unknownDeviceWindow = new UnknownDeviceWindow((UnknownDevice)antDevice);
                    unknownDeviceWindow.Show();
                    break;
            }
        }
    }
}
