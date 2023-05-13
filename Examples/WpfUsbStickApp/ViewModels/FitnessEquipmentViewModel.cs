using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;
using WpfUsbStickApp.Controls;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class FitnessEquipmentViewModel : ObservableObject
    {
        public FitnessEquipment FitnessEquipment { get; }
        public UserControl? FeControl { get; }

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

        public FitnessEquipmentViewModel(FitnessEquipment fitnessEquipment)
        {
            FitnessEquipment = fitnessEquipment;

            switch (fitnessEquipment.GeneralData.EquipmentType)
            {
                case FitnessEquipment.FitnessEquipmentType.Treadmill:
                    FeControl = new TreadmillControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.Elliptical:
                    FeControl = new EllipticalControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.Rower:
                    FeControl = new RowerControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.Climber:
                    FeControl = new ClimberControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.NordicSkier:
                    FeControl = new NordicSkierControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.TrainerStationaryBike:
                    FeControl = new TrainerStationaryBikeControl(fitnessEquipment);
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        private void FECapabilitiesRequest() => FitnessEquipment.RequestFECapabilities();
        [RelayCommand]
        private void SetUserConfig() => FitnessEquipment.SetUserConfiguration(UserWeight, WheelDiameterOffset, BikeWeight, WheelDiameter, GearRatio);
        [RelayCommand]
        private void SetBasicResistance(string percent) => FitnessEquipment.SetBasicResistance(double.Parse(percent));
        [RelayCommand]
        private void SetTargetPower(string power) => FitnessEquipment.SetTargetPower(double.Parse(power));
        [RelayCommand]
        private void SetWindResistance() => FitnessEquipment.SetWindResistance(0.51, -30, 0.9);
        [RelayCommand]
        private void SetTrackResistance() => FitnessEquipment.SetTrackResistance(-15);
    }
}
