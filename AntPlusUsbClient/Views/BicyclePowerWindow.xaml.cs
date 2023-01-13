using AntPlus.DeviceProfiles.BicyclePower;
using AntPlusUsbClient.Controls;
using AntPlusUsbClient.ViewModels;
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
            BindingOperations.EnableCollectionSynchronization(bicyclePowerViewModel.BicyclePower.Calibration.Measurements, bicyclePowerViewModel.BicyclePower.Calibration.Measurements.collectionLock);

            switch (bicyclePower.Sensor)
            {
                case SensorType.PowerOnly:
                    BicyclePowerControl.Children.Add(new BicyclePowerOnlyControl(bicyclePowerViewModel));
                    //BicyclePowerControl.Children.Add(new BicyclePowerCalibrationControl(bicyclePowerViewModel));
                    break;
                case SensorType.WheelTorque:
                    BicyclePowerControl.Children.Add(new BicycleWheelTorqueControl(bicyclePowerViewModel));
                    BicyclePowerControl.Children.Add(new BicyclePowerCalibrationControl(bicyclePowerViewModel));
                    break;
                case SensorType.CrankTorque:
                    BicyclePowerControl.Children.Add(new BicycleCrankTorqueControl(bicyclePowerViewModel));
                    BicyclePowerControl.Children.Add(new BicyclePowerCalibrationControl(bicyclePowerViewModel));
                    break;
                case SensorType.CrankTorqueFrequency:
                    BicyclePowerControl.Children.Add(new CTFControl(bicyclePower.CTFSensor));
                    break;
                default:
                    break;
            }

            CommandBindings.AddRange(bicyclePowerViewModel.CommandBindings);
            DataContext = bicyclePowerViewModel;
        }
    }
}
