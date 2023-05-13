using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;
using WpfUsbStickApp.Controls;

namespace WpfUsbStickApp.ViewModels
{
    public partial class BicyclePowerViewModel : ObservableObject
    {
        public BicyclePower BicyclePower { get; }
        public SensorType SensorType => BicyclePower.Sensor;

        public UserControl? BicyclePowerControl { get; }

        public BicyclePowerViewModel(BicyclePower bicyclePower)
        {
            BicyclePower = bicyclePower;

            switch (bicyclePower.Sensor)
            {
                case SensorType.PowerOnly:
                    BicyclePowerControl = new BicyclePowerOnlyControl(BicyclePower.PowerOnlySensor);
                    break;
                case SensorType.WheelTorque:
                    BicyclePowerControl = new BicycleWheelTorqueControl(BicyclePower.WheelTorqueSensor);
                    break;
                case SensorType.CrankTorque:
                    BicyclePowerControl = new BicycleCrankTorqueControl(BicyclePower.CrankTorqueSensor);
                    break;
                case SensorType.CrankTorqueFrequency:
                    BicyclePowerControl = new CTFControl(BicyclePower.CTFSensor);
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        private void ManualCalRequest() => BicyclePower.Calibration.RequestManualCalibration();

        [RelayCommand(CanExecute = nameof(CanSetAutoZeroConfig))]
        private void SetAutoZeroConfig() => BicyclePower.Calibration.SetAutoZeroConfiguration(Calibration.AutoZero.On);
        private bool CanSetAutoZeroConfig() => BicyclePower.Sensor != SensorType.CrankTorqueFrequency;

        [RelayCommand(CanExecute = nameof(CanGetCustomCalParameters))]
        private void GetCustomCalParameters() => BicyclePower.Calibration.RequestCustomParameters();
        private bool CanGetCustomCalParameters() => BicyclePower.Sensor != SensorType.CrankTorqueFrequency;

        [RelayCommand(CanExecute = nameof(CanSetCustomCalParameters))]
        private void SetCustomCalParameters() => BicyclePower.Calibration.SetCustomParameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 });
        private bool CanSetCustomCalParameters() => BicyclePower.Sensor != SensorType.CrankTorqueFrequency;

        [RelayCommand]
        private void GetParameters(Subpage subpage) => BicyclePower.PowerOnlySensor.Parameters.GetParameters(subpage);

        [RelayCommand]
        private void SetCrankLength(string length) => BicyclePower.PowerOnlySensor.Parameters.SetCrankLength(double.Parse(length));
    }
}
