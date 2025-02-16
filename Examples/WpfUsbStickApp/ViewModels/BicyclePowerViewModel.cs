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
        public StandardPowerSensor Sensor { get; }

        public UserControl? BicyclePowerControl { get; }

        [ObservableProperty]
        private bool autoCrankLength;

        public BicyclePowerViewModel(StandardPowerSensor sensor)
        {
            Sensor = sensor;
            if (Sensor.TorqueSensor != null)
            {
                switch (Sensor.TorqueSensor)
                {
                    case StandardCrankTorqueSensor:
                        BicyclePowerControl = new BicycleCrankTorqueControl(Sensor.TorqueSensor);
                        break;
                    case StandardWheelTorqueSensor:
                        BicyclePowerControl = new BicycleWheelTorqueControl(Sensor.TorqueSensor);
                        break;
                    default:
                        break;
                }
            }
        }

        [RelayCommand]
        private async Task ManualCalRequest() => await Sensor.RequestManualCalibration();

        [RelayCommand]
        private async Task SetAutoZeroConfig() => await Sensor.SetAutoZeroConfiguration(AutoZero.On);

        [RelayCommand]
        private async Task GetCustomCalParameters() => await Sensor.RequestCustomParameters();

        [RelayCommand]
        private async Task SetCustomCalParameters() => await Sensor.SetCustomParameters([0x11, 0x22, 0x33, 0x44, 0x55, 0x66]);

        [RelayCommand]
        private async Task GetParameters(SubPage subpage) => await Sensor.GetParameters(subpage);

        [RelayCommand]
        private async Task SetCrankLength(string length)
        {
            if (AutoCrankLength)
            {
                await Sensor.SetCrankLength(0xFE);
            }
            else
            {
                await Sensor.SetCrankLength(double.Parse(length));
            }
        }
    }
}
