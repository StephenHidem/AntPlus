using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Threading.Tasks;

namespace WpfUsbStickApp.ViewModels
{
    public partial class TrainerStationaryBikeViewModel : ObservableObject
    {
        [ObservableProperty]
        private TrainerStationaryBike trainer;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CalibrateCommand))]
        private bool calibrateZeroOffset;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CalibrateCommand))]
        private bool calibrateSpinDown;

        [ObservableProperty]
        private double userWeight;
        [ObservableProperty]
        private byte wheelDiameterOffset;
        [ObservableProperty]
        private double bikeWeight;
        [ObservableProperty]
        private double wheelDiameter;
        [ObservableProperty]
        private double gearRatio;

        public TrainerStationaryBikeViewModel(TrainerStationaryBike trainerStationaryBike)
        {
            trainer = trainerStationaryBike;
        }

        [RelayCommand]
        private async Task SetUserConfig() => await Trainer.SetUserConfiguration(UserWeight, WheelDiameterOffset, BikeWeight, WheelDiameter, GearRatio);

        [RelayCommand(CanExecute = nameof(CanCalibrate))]
        private async Task Calibrate()
        {
            TrainerStationaryBike.CalibrationRequestResponse req = TrainerStationaryBike.CalibrationRequestResponse.None;
            if (CalibrateSpinDown)
            {
                req |= TrainerStationaryBike.CalibrationRequestResponse.SpinDown;
            }
            if (CalibrateZeroOffset)
            {
                req |= TrainerStationaryBike.CalibrationRequestResponse.ZeroOffset;
            }
            await Trainer.CalibrationRequest(req);
        }
        private bool CanCalibrate()
        {
            return CalibrateSpinDown || CalibrateZeroOffset;
        }
    }
}
