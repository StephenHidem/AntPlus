using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfUsbStickApp.Controls;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class FitnessEquipmentViewModel : ObservableObject
    {
        public Equipment FitnessEquipment { get; }
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
        [ObservableProperty]
        private TimeSpan lapSplitTime;

        public FitnessEquipmentViewModel(Equipment fitnessEquipment)
        {
            FitnessEquipment = fitnessEquipment;
            FitnessEquipment.LapToggled += FitnessEquipment_LapToggled;
            switch (fitnessEquipment.GeneralData.EquipmentType)
            {
                case Equipment.FitnessEquipmentType.Treadmill:
                    FeControl = new TreadmillControl(fitnessEquipment);
                    break;
                case Equipment.FitnessEquipmentType.Elliptical:
                    FeControl = new EllipticalControl(fitnessEquipment);
                    break;
                case Equipment.FitnessEquipmentType.Rower:
                    FeControl = new RowerControl(fitnessEquipment);
                    break;
                case Equipment.FitnessEquipmentType.Climber:
                    FeControl = new ClimberControl(fitnessEquipment);
                    break;
                case Equipment.FitnessEquipmentType.NordicSkier:
                    FeControl = new NordicSkierControl(fitnessEquipment);
                    break;
                case Equipment.FitnessEquipmentType.TrainerStationaryBike:
                    FeControl = new TrainerStationaryBikeControl(fitnessEquipment);
                    break;
                default:
                    break;
            }
        }

        private void FitnessEquipment_LapToggled(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                LapSplitTime = ((Equipment)sender).GeneralData.ElapsedTime;
            }
        }

        [RelayCommand]
        private async Task FECapabilitiesRequest() => await FitnessEquipment.RequestFECapabilities();
        [RelayCommand]
        private async Task SetUserConfig() => await FitnessEquipment.SetUserConfiguration(UserWeight, WheelDiameterOffset, BikeWeight, WheelDiameter, GearRatio);
        [RelayCommand]
        private async Task SetBasicResistance(string percent) => await FitnessEquipment.SetBasicResistance(double.Parse(percent));
        [RelayCommand]
        private async Task SetTargetPower(string power) => await FitnessEquipment.SetTargetPower(double.Parse(power));
        [RelayCommand]
        private async Task SetWindResistance() => await FitnessEquipment.SetWindResistance(0.51, -30, 0.9);
        [RelayCommand]
        private async Task SetTrackResistance() => await FitnessEquipment.SetTrackResistance(-15);
    }
}
