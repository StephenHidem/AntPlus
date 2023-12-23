using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAntClientApp.Views.BicyclePowerPages;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;
using System.ComponentModel;

namespace MauiAntClientApp.ViewModels
{
    public partial class BicyclePowerViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ILogger<BicyclePowerViewModel> _logger;
        public SensorType SensorType => BicyclePower.Sensor;

        [ObservableProperty]
        private Bicycle bicyclePower = null!;
        [ObservableProperty]
        private ContentView specificBicycleView = null!;
        [ObservableProperty]
        private bool options;

        [ObservableProperty]
        private string ctfAckMessage = null!;

        public BicyclePowerViewModel(ILogger<BicyclePowerViewModel> logger)
        {
            _logger = logger;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BicyclePower = (Bicycle)query["Sensor"];
            BicyclePower.Calibration.PropertyChanged += OnPropertyChanged;
            switch (BicyclePower.Sensor)
            {
                case SensorType.Power:
                    SpecificBicycleView = new BicyclePowerOnlyView(this);
                    break;
                case SensorType.WheelTorque:
                    SpecificBicycleView = new BicycleWheelTorqueView(this);
                    break;
                case SensorType.CrankTorque:
                    SpecificBicycleView = new BicycleCrankTorqueView(this);
                    break;
                case SensorType.CrankTorqueFrequency:
                    SpecificBicycleView = new CrankTorqueFrequencyView(this);
                    BicyclePower.CTFSensor.SaveAcknowledged += CTFSensor_SaveAcknowledged;
                    break;
                default:
                    break;
            }
            Options = BicyclePower.Sensor != SensorType.CrankTorqueFrequency;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _logger.LogInformation("Sender: {Sender}, Property: {PropName}", sender, e.PropertyName);
            if (e.PropertyName == "AutoZeroSupported")
            {
                SetAutoZeroConfigCommand.NotifyCanExecuteChanged();
            }
            if (e.PropertyName == "CalibrationStatus")
            {
                _logger.LogInformation("{Status}", ((Calibration)sender).CalibrationStatus);
            }
        }

        private void CTFSensor_SaveAcknowledged(object? sender, CrankTorqueFrequencySensor.CTFDefinedId e)
        {
            switch (e)
            {
                case CrankTorqueFrequencySensor.CTFDefinedId.Slope:
                    CtfAckMessage = "Slope saved.";
                    break;
                case CrankTorqueFrequencySensor.CTFDefinedId.SerialNumber:
                    CtfAckMessage = "Serial number saved.";
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        private async Task<MessagingReturnCode> ManualCalRequest() => await BicyclePower.Calibration.RequestManualCalibration();

        [RelayCommand(CanExecute = nameof(CanSetAutoZeroConfig))]
        private async Task<MessagingReturnCode> SetAutoZeroConfig() => await BicyclePower.Calibration.SetAutoZeroConfiguration(BicyclePower.Calibration.AutoZeroStatus == Calibration.AutoZero.Off ? Calibration.AutoZero.On : Calibration.AutoZero.Off);
        private bool CanSetAutoZeroConfig()
        {
            return BicyclePower != null && BicyclePower.Sensor != SensorType.CrankTorqueFrequency && BicyclePower.Calibration.AutoZeroSupported;
        }

        [RelayCommand(CanExecute = nameof(CanGetCustomCalParameters))]
        private async Task<MessagingReturnCode> GetCustomCalParameters() => await BicyclePower.Calibration.RequestCustomParameters();
        private bool CanGetCustomCalParameters()
        {
            return BicyclePower != null && BicyclePower.Sensor != SensorType.CrankTorqueFrequency;
        }

        [RelayCommand(CanExecute = nameof(CanSetCustomCalParameters))]
        private async Task<MessagingReturnCode> SetCustomCalParameters(string parms) => await BicyclePower.Calibration.SetCustomParameters(Convert.FromHexString(parms));
        private bool CanSetCustomCalParameters()
        {
            return BicyclePower != null && BicyclePower.Sensor != SensorType.CrankTorqueFrequency;
        }

        [RelayCommand]
        private async Task<MessagingReturnCode> GetParameters(Subpage subpage) => await BicyclePower.PowerSensor.Parameters.GetParameters(subpage);

        [RelayCommand]
        private async Task<MessagingReturnCode> SetCrankLength(string length) => await BicyclePower.PowerSensor.Parameters.SetCrankLength(Convert.ToDouble(length));

        [RelayCommand]
        private async Task<MessagingReturnCode> SaveSlope(string slope)
        {
            CtfAckMessage = "Save slope";
            return await BicyclePower.CTFSensor.SaveSlopeToFlash(Convert.ToDouble(slope));
        }

        [RelayCommand]
        private async Task<MessagingReturnCode> SaveSerialNumber(string sn)
        {
            CtfAckMessage = "Save SN";
            return await BicyclePower.CTFSensor.SaveSerialNumberToFlash(Convert.ToUInt16(sn));
        }
    }
}
