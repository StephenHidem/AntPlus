using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class CTFViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private CrankTorqueFrequencySensor? sensor;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Sensor = (CrankTorqueFrequencySensor)query["Sensor"];
            Sensor.PropertyChanged += Sensor_PropertyChanged;
        }

        private void Sensor_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CalibrationStatus")
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ManualCalRequestCommand.NotifyCanExecuteChanged();
                    SaveSlopeCommand.NotifyCanExecuteChanged();
                    SaveSerialNumberCommand.NotifyCanExecuteChanged();
                });
            }
        }

        [RelayCommand(CanExecute = nameof(CheckCanExecute))]
        private async Task ManualCalRequest()
        {
            _ = await Sensor!.RequestManualCalibration();
        }

        [RelayCommand(CanExecute = nameof(CheckCanExecute))]
        private async Task SaveSlope(string slope)
        {
            _ = await Sensor!.SaveSlopeToFlash(double.Parse(slope));
        }

        [RelayCommand(CanExecute = nameof(CheckCanExecute))]
        private async Task SaveSerialNumber(string serialNumber)
        {
            _ = await Sensor!.SaveSerialNumberToFlash(ushort.Parse(serialNumber));
        }

        private bool CheckCanExecute => Sensor?.CalibrationStatus != CalibrationResponse.InProgress;
    }
}
