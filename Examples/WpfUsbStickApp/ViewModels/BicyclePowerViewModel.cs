using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfUsbStickApp.Controls;

namespace WpfUsbStickApp.ViewModels
{
    public partial class BicyclePowerViewModel
    {
        public BicyclePower BicyclePower { get; }

        public UserControl? BicyclePowerControl { get; }

        public BicyclePowerViewModel(BicyclePower bicyclePower)
        {
            switch (bicyclePower)
            {
                case StandardPowerSensor:
                    StandardPowerSensor sensor = (StandardPowerSensor)bicyclePower;
                    BicyclePower = sensor;
                    if (sensor.TorqueSensor == null)
                    {
                        BicyclePowerControl = new BicyclePowerOnlyControl(sensor);
                    }
                    else
                    {
                        switch (sensor.TorqueSensor)
                        {
                            case StandardCrankTorqueSensor:
                                BicyclePowerControl = new BicycleCrankTorqueControl(sensor);
                                break;
                            case StandardWheelTorqueSensor:
                                BicyclePowerControl = new BicycleWheelTorqueControl(sensor);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case CrankTorqueFrequencySensor:
                    BicyclePower = (CrankTorqueFrequencySensor)bicyclePower;
                    BicyclePowerControl = new CTFControl((CrankTorqueFrequencySensor)bicyclePower);
                    break;
                default:
                    BicyclePowerControl = null;
                    break;
            }
        }

        [RelayCommand]
        private async Task ManualCalRequest() => await BicyclePower.Calibration.RequestManualCalibration();

        [RelayCommand(CanExecute = nameof(CanSetAutoZeroConfig))]
        private async Task SetAutoZeroConfig() => await BicyclePower.Calibration.SetAutoZeroConfiguration(Calibration.AutoZero.On);
        private bool CanSetAutoZeroConfig() => BicyclePower is not CrankTorqueFrequencySensor;

        [RelayCommand(CanExecute = nameof(CanGetCustomCalParameters))]
        private async Task GetCustomCalParameters() => await BicyclePower.Calibration.RequestCustomParameters();
        private bool CanGetCustomCalParameters() => BicyclePower is not CrankTorqueFrequencySensor;

        [RelayCommand(CanExecute = nameof(CanSetCustomCalParameters))]
        private async Task SetCustomCalParameters() => await BicyclePower.Calibration.SetCustomParameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 });
        private bool CanSetCustomCalParameters() => BicyclePower is not CrankTorqueFrequencySensor;

        [RelayCommand]
        private async Task GetParameters(Subpage subpage) => await ((StandardPowerSensor)BicyclePower).Parameters.GetParameters(subpage);

        [RelayCommand]
        private async Task SetCrankLength(string length) => await ((StandardPowerSensor)BicyclePower).Parameters.SetCrankLength(double.Parse(length));
    }
}
