using AntPlus.DeviceProfiles.BicyclePower;
using AntPlusUsbClient.Controls;
using AntPlusUsbClient.ViewModels;
using System.Windows;

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

            switch (bicyclePower.Sensor)
            {
                case SensorType.PowerOnly:
                    BicyclePowerControl.Children.Add(new BicyclePowerOnlyControl(bicyclePower));
                    BicyclePowerControl.Children.Add(new TEPSControl(bicyclePower.PowerOnlySensor));
                    BicyclePowerControl.Children.Add(new BicyclePowerCalibrationControl(bicyclePowerViewModel));
                    break;
                case SensorType.WheelTorque:
                    BicyclePowerControl.Children.Add(new BicycleWheelTorqueControl(bicyclePower));
                    BicyclePowerControl.Children.Add(new TEPSControl(bicyclePower.WheelTorqueSensor));
                    BicyclePowerControl.Children.Add(new BicyclePowerCalibrationControl(bicyclePowerViewModel));
                    break;
                case SensorType.CrankTorque:
                    BicyclePowerControl.Children.Add(new BicycleCrankTorqueControl(bicyclePower));
                    BicyclePowerControl.Children.Add(new TEPSControl(bicyclePower.CrankTorqueSensor));
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
