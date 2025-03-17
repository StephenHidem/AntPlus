using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAntGrpcClient.Views.BicyclePower;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.ComponentModel;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class BicyclePowerViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        public partial StandardPowerSensor? Sensor { get; set; }

        [ObservableProperty]
        public partial bool AutoCrankLength { get; set; }

        [ObservableProperty]
        public partial ContentView? TorqueSensorView { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Sensor = (StandardPowerSensor)query["Sensor"];
            Sensor.PropertyChanged += OnPropertyChanged;
            if (Sensor.TorqueSensor != null)
            {
                switch (Sensor.TorqueSensor)
                {
                    case StandardCrankTorqueSensor:
                        TorqueSensorView = new BicycleCrankTorqueView((StandardCrankTorqueSensor)Sensor.TorqueSensor);
                        break;
                    case StandardWheelTorqueSensor:
                        TorqueSensorView = new BicycleWheelTorqueView((StandardWheelTorqueSensor)Sensor.TorqueSensor);
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AutoZeroSupported")
            {
                SetAutoZeroConfigCommand.NotifyCanExecuteChanged();
            }
            if (sender != null && e.PropertyName == "CalibrationStatus")
            {
            }
        }

        [RelayCommand(CanExecute = nameof(CheckCanExecute))]
        private async Task ManualCalRequest() => _ = await Sensor!.RequestManualCalibration();

        [RelayCommand(CanExecute = nameof(CheckCanExecute))]
        private async Task SetAutoZeroConfig() => _ = await Sensor!.SetAutoZeroConfiguration(Sensor.AutoZeroStatus == AutoZero.Off ? AutoZero.On : AutoZero.Off);

        [RelayCommand(CanExecute = nameof(CheckCanExecute))]
        private async Task GetCustomCalParameters() => _ = await Sensor!.RequestCustomParameters();

        [RelayCommand(CanExecute = nameof(CheckCanExecute))]
        private async Task SetCustomCalParameters(string parameters) => _ = await Sensor!.SetCustomParameters(Convert.FromHexString(parameters));

        [RelayCommand(CanExecute = nameof(CheckCanExecute))]
        private async Task GetParameters(SubPage subpage) => _ = await Sensor!.GetParameters(subpage);

        [RelayCommand(CanExecute = nameof(CheckCanExecute))]
        private async Task SetCrankLength(string length)
        {
            if (AutoCrankLength)
            {
                _ = await Sensor!.SetCrankLength(0xFE);
            }
            else
            {
                _ = await Sensor!.SetCrankLength(double.Parse(length));
            }
        }

        private bool CheckCanExecute => Sensor?.CalibrationStatus != CalibrationResponse.InProgress;
    }
}
