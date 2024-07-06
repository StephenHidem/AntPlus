using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System;
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
        private readonly MainWindowViewModel? viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // the view model may throw an exception if ANT radio is not available
            try
            {
                viewModel = new MainWindowViewModel();
                BindingOperations.EnableCollectionSynchronization(viewModel.AntDevices, viewModel.AntDevices.CollectionLock);
                DataContext = viewModel;
                antDevices.MouseDoubleClick += AntDevices_MouseDoubleClick;
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "Unable to initialize!", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
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
                    BicyclePowerWindow bpWindow = new((BicyclePower)antDevice);
                    bpWindow.Show();
                    break;
                case BikeSpeedSensor.DeviceClass:
                    BikeSpeedWindow bikeSpeedWindow = new((BikeSpeedSensor)antDevice);
                    bikeSpeedWindow.Show();
                    break;
                case BikeCadenceSensor.DeviceClass:
                    BikeCadenceWindow bikeCadenceWindow = new((BikeCadenceSensor)antDevice);
                    bikeCadenceWindow.Show();
                    break;
                case CombinedSpeedAndCadenceSensor.DeviceClass:
                    BikeSpeedAndCadenceWindow speedAndCadenceWindow = new((CombinedSpeedAndCadenceSensor)antDevice);
                    speedAndCadenceWindow.Show();
                    break;
                case Equipment.DeviceClass:
                    FitnessEquipmentWindow fitnessEquipmentWindow = new((Equipment)antDevice);
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
                case Tracker.DeviceClass:
                    AssetTrackerWindow tracker = new((Tracker)antDevice);
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

        private async void Capabilities_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                CapabilitiesWindow capabilitiesWindow = new(await viewModel.UsbAntRadio.GetDeviceCapabilities())
                {
                    Icon = Icon
                };
                capabilitiesWindow.Show();
            }
        }
    }
}
