using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfUsbStickApp.Controls;

namespace WpfUsbStickApp.ViewModels
{
    public partial class BicyclePowerViewModel : ObservableObject
    {
        public Bicycle BicyclePower { get; }
        public SensorType SensorType => BicyclePower.Sensor;

        public UserControl? BicyclePowerControl { get; }

        public BicyclePowerViewModel(Bicycle bicyclePower)
        {
            BicyclePower = bicyclePower;

            switch (bicyclePower.Sensor)
            {
                case SensorType.Power:
                    BicyclePowerControl = new BicyclePowerOnlyControl(bicyclePower);
                    break;
                case SensorType.WheelTorque:
                    BicyclePowerControl = new BicycleWheelTorqueControl(bicyclePower);
                    break;
                case SensorType.CrankTorque:
                    BicyclePowerControl = new BicycleCrankTorqueControl(bicyclePower);
                    break;
                case SensorType.CrankTorqueFrequency:
                    BicyclePowerControl = new CTFControl(BicyclePower.CTFSensor);
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        private async Task ManualCalRequest() => await BicyclePower.Calibration.RequestManualCalibration();

        [RelayCommand(CanExecute = nameof(CanSetAutoZeroConfig))]
        private async Task SetAutoZeroConfig() => await BicyclePower.Calibration.SetAutoZeroConfiguration(Calibration.AutoZero.On);
        private bool CanSetAutoZeroConfig() => BicyclePower.Sensor != SensorType.CrankTorqueFrequency;

        [RelayCommand(CanExecute = nameof(CanGetCustomCalParameters))]
        private async Task GetCustomCalParameters() => await BicyclePower.Calibration.RequestCustomParameters();
        private bool CanGetCustomCalParameters() => BicyclePower.Sensor != SensorType.CrankTorqueFrequency;

        [RelayCommand(CanExecute = nameof(CanSetCustomCalParameters))]
        private async Task SetCustomCalParameters() => await BicyclePower.Calibration.SetCustomParameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 });
        private bool CanSetCustomCalParameters() => BicyclePower.Sensor != SensorType.CrankTorqueFrequency;

        [RelayCommand]
        private async Task GetParameters(Subpage subpage) => await BicyclePower.PowerSensor.Parameters.GetParameters(subpage);

        [RelayCommand]
        private async Task SetCrankLength(string length) => await BicyclePower.PowerSensor.Parameters.SetCrankLength(double.Parse(length));
    }
}
