using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntClientApp.ViewModels
{
    [QueryProperty(nameof(BicyclePower), "Sensor")]
    public partial class BicyclePowerViewModel : ObservableObject
    {
        private readonly ILogger<BicyclePowerViewModel> _logger;
        public SensorType SensorType => BicyclePower.Sensor;

        [ObservableProperty]
        private Bicycle bicyclePower = null!;

        [ObservableProperty]
        private string ctfAckMessage = null!;

        public BicyclePowerViewModel(ILogger<BicyclePowerViewModel> logger)
        {
            _logger = logger;
            if (BicyclePower.Sensor == SensorType.CrankTorqueFrequency)
            {
                BicyclePower.CTFSensor.SaveAcknowledged += CTFSensor_SaveAcknowledged;
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
        private async Task<MessagingReturnCode> SetAutoZeroConfig() => await BicyclePower.Calibration.SetAutoZeroConfiguration(Calibration.AutoZero.On);
        private bool CanSetAutoZeroConfig() => BicyclePower.Sensor != SensorType.CrankTorqueFrequency;

        [RelayCommand(CanExecute = nameof(CanGetCustomCalParameters))]
        private async Task<MessagingReturnCode> GetCustomCalParameters() => await BicyclePower.Calibration.RequestCustomParameters();
        private bool CanGetCustomCalParameters() => BicyclePower.Sensor != SensorType.CrankTorqueFrequency;

        [RelayCommand(CanExecute = nameof(CanSetCustomCalParameters))]
        private async Task<MessagingReturnCode> SetCustomCalParameters() => await BicyclePower.Calibration.SetCustomParameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 });
        private bool CanSetCustomCalParameters() => BicyclePower.Sensor != SensorType.CrankTorqueFrequency;

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
