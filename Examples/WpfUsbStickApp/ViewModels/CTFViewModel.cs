using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows;

namespace WpfUsbStickApp.ViewModels
{
    public partial class CTFViewModel : ObservableObject
    {
        public CrankTorqueFrequencySensor Sensor { get; }

        public CTFViewModel(CrankTorqueFrequencySensor sensor)
        {
            Sensor = sensor;
            Sensor.PropertyChanged += Sensor_PropertyChanged;
        }

        private void Sensor_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CalibrationStatus")
            {
                _ = Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    ManualCalRequestCommand.NotifyCanExecuteChanged();
                    SaveSlopeCommand.NotifyCanExecuteChanged();
                    SaveSerialNumberCommand.NotifyCanExecuteChanged();
                });
            }
        }

        [RelayCommand(CanExecute = "CheckCanExecute")]
        private void ManualCalRequest()
        {
            _ = Sensor.RequestManualCalibration();
        }

        [RelayCommand(CanExecute = "CheckCanExecute")]
        private void SaveSlope(string slope)
        {
            _ = Sensor.SaveSlopeToFlash(double.Parse(slope));
        }

        [RelayCommand(CanExecute = "CheckCanExecute")]
        private void SaveSerialNumber(string serialNumber)
        {
            _ = Sensor.SaveSerialNumberToFlash(ushort.Parse(serialNumber));
        }

        private bool CheckCanExecute => Sensor.CalibrationStatus != CalibrationResponse.InProgress;
    }
}
