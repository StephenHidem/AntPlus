using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntPlus.DeviceProfiles.Geocache;
using SmallEarthTech.AntPlus.DeviceProfiles.HeartRate;
using SmallEarthTech.AntPlus.DeviceProfiles.MuscleOxygen;
using SmallEarthTech.AntPlus.DeviceProfiles.StrideBasedSpeedAndDistance;
using SmallEarthTech.AntPlus.DeviceProfiles.UnknownDevice;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfUsbStickApp.ViewModels;
using WpfUsbStickApp.Views;

namespace WpfUsbStickApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
            BindingOperations.EnableCollectionSynchronization(viewModel.AntDevices, viewModel.AntDevices.CollectionLock);
            DataContext = viewModel;
            antDevices.MouseDoubleClick += AntDevices_MouseDoubleClick;
        }

        private void AntDevices_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AntDevice? antDevice = ((ListView)sender).SelectedItem as AntDevice;
            switch (antDevice?.ChannelId.DeviceType)
            {
                case HeartRate.DeviceClass:
                    HeartRateWindow heartRateWindow = new((HeartRate)antDevice);
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
                    MuscleOxygenWindow muscleOxygenWindow = new((MuscleOxygen)antDevice);
                    muscleOxygenWindow.Show();
                    break;
                case Geocache.DeviceClass:
                    GeocacheWindow geocacheWindow = new((Geocache)antDevice);
                    geocacheWindow.Show();
                    break;
                case AssetTracker.DeviceClass:
                    AssetTrackerWindow tracker = new AssetTrackerWindow((AssetTracker)antDevice);
                    tracker.Show();
                    break;
                case StrideBasedSpeedAndDistance.DeviceClass:
                    StrideBasedMonitorWindow sdmWindow = new((StrideBasedSpeedAndDistance)antDevice);
                    sdmWindow.Show();
                    break;
                default:
                    // unknown device
                    if (antDevice is AntDevice ad)
                    {
                        UnknownDeviceWindow unknownDeviceWindow = new((UnknownDevice)ad);
                        unknownDeviceWindow.Show();
                    }
                    break;
            }
        }

        private void Capabilities_Click(object sender, RoutedEventArgs e)
        {
            CapabilitiesWindow capabilitiesWindow = new(viewModel.AntRadio.GetDeviceCapabilities())
            {
                Icon = Icon
            };
            capabilitiesWindow.Show();
        }
    }
}
