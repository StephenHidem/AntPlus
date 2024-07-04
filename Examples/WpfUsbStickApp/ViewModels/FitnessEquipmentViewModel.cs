using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfUsbStickApp.Controls;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class FitnessEquipmentViewModel : ObservableObject
    {
        public Equipment FitnessEquipment { get; }
        public UserControl? FeControl { get; }

        [ObservableProperty]
        public string[]? trainingModes;

        [ObservableProperty]
        private TimeSpan lapSplitTime;

        public FitnessEquipmentViewModel(Equipment fitnessEquipment)
        {
            FitnessEquipment = fitnessEquipment;
            TrainingModes = fitnessEquipment.TrainingModes.ToString().Split(',');
            FitnessEquipment.LapToggled += FitnessEquipment_LapToggled;
            FitnessEquipment.PropertyChanged += FitnessEquipment_PropertyChanged;

            _ = FitnessEquipment.RequestFECapabilities();

            switch (fitnessEquipment)
            {
                case Climber:
                    FeControl = new ClimberControl((Climber)fitnessEquipment);
                    break;
                case Elliptical:
                    FeControl = new EllipticalControl((Elliptical)fitnessEquipment);
                    break;
                case NordicSkier:
                    FeControl = new NordicSkierControl((NordicSkier)fitnessEquipment);
                    break;
                case Rower:
                    FeControl = new RowerControl((Rower)fitnessEquipment);
                    break;
                case TrainerStationaryBike:
                    FeControl = new TrainerStationaryBikeControl((TrainerStationaryBike)fitnessEquipment);
                    break;
                case Treadmill:
                    FeControl = new TreadmillControl((Treadmill)fitnessEquipment);
                    break;
                default:
                    break;
            }
        }

        private void FitnessEquipment_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender != null && e.PropertyName == nameof(FitnessEquipment.TrainingModes))
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    TrainingModes = ((Equipment)sender).TrainingModes.ToString().Split(',');
                });
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
        private async Task SetBasicResistance(string percent) => await FitnessEquipment.SetBasicResistance(double.Parse(percent));

        [RelayCommand]
        private async Task SetTargetPower(string power) => await FitnessEquipment.SetTargetPower(double.Parse(power));

        [RelayCommand]
        private async Task SetWindResistance() => await FitnessEquipment.SetWindResistance(0.51, -30, 0.9);

        [RelayCommand]
        private async Task SetTrackResistance(string grade) => await FitnessEquipment.SetTrackResistance(double.Parse(grade));
    }
}
