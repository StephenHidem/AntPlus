using AntPlusUsbClient.Controls;
using AntPlusUsbClient.ViewModels;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows;
using System.Windows.Data;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for BicyclePowerWindow.xaml
    /// </summary>
    public partial class BicyclePowerWindow : Window
    {
        public BicyclePowerWindow(BicyclePower bicyclePower)
        {
            InitializeComponent();
            BicyclePowerViewModel bicyclePowerViewModel = new BicyclePowerViewModel(bicyclePower);
            BindingOperations.EnableCollectionSynchronization(bicyclePowerViewModel.BicyclePower.Calibration.Measurements, bicyclePowerViewModel.BicyclePower.Calibration.Measurements.CollectionLock);

            switch (bicyclePower.Sensor)
            {
                case SensorType.PowerOnly:
                    BicyclePowerControl.Children.Add(new BicyclePowerOnlyControl(bicyclePowerViewModel));
                    break;
                case SensorType.WheelTorque:
                    BicyclePowerControl.Children.Add(new BicycleWheelTorqueControl(bicyclePowerViewModel));
                    break;
                case SensorType.CrankTorque:
                    BicyclePowerControl.Children.Add(new BicycleCrankTorqueControl(bicyclePowerViewModel));
                    break;
                case SensorType.CrankTorqueFrequency:
                    BicyclePowerControl.Children.Add(new CTFControl(bicyclePowerViewModel));
                    break;
                default:
                    break;
            }

            CommandBindings.AddRange(bicyclePowerViewModel.CommandBindings);
            DataContext = bicyclePowerViewModel;
        }
    }
}
