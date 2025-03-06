using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAntGrpcClient.Views.FitnessEquipment;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class FitnessEquipmentViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        public partial FitnessEquipment? FitnessEquipment { get; set; }
        [ObservableProperty]
        public partial ContentView? SpecificEquipmentView { get; set; }

        [ObservableProperty]
        public partial double UserWeight { get; set; }

        [ObservableProperty]
        public partial byte WheelDiameterOffset { get; set; }

        [ObservableProperty]
        public partial double BikeWeight { get; set; }

        [ObservableProperty]
        public partial double WheelDiameter { get; set; }

        [ObservableProperty]
        public partial double GearRatio { get; set; }

        [ObservableProperty]
        public partial TimeSpan LapSplitTime { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(
            nameof(SetBasicResistanceCommand),
            nameof(SetTargetPowerCommand),
            nameof(SetWindResistanceCommand),
            nameof(SetTrackResistanceCommand))]
        public partial SupportedTrainingModes SupportedTrainingModes { get; set; }
        [ObservableProperty]
        public partial string[]? Capabilities { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            FitnessEquipment = (FitnessEquipment)query["Sensor"];
            FitnessEquipment.LapToggled += FitnessEquipment_LapToggled;
            FitnessEquipment.PropertyChanged += FitnessEquipment_PropertyChanged;

            switch (FitnessEquipment)
            {
                case Treadmill:
                    SpecificEquipmentView = new TreadmillView((Treadmill)FitnessEquipment);
                    break;
                case Elliptical:
                    SpecificEquipmentView = new EllipticalView((Elliptical)FitnessEquipment);
                    break;
                case Rower:
                    SpecificEquipmentView = new RowerView((Rower)FitnessEquipment);
                    break;
                case Climber:
                    SpecificEquipmentView = new ClimberView((Climber)FitnessEquipment);
                    break;
                case NordicSkier:
                    SpecificEquipmentView = new NordicSkierView((NordicSkier)FitnessEquipment);
                    break;
                case TrainerStationaryBike:
                    SpecificEquipmentView = new TrainerStationaryBikeView((TrainerStationaryBike)FitnessEquipment);
                    break;
                default:
                    break;
            }

            _ = FitnessEquipment.RequestFECapabilities();
        }

        private void FitnessEquipment_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TrainingModes")
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SupportedTrainingModes = FitnessEquipment!.TrainingModes;
                    Capabilities = FitnessEquipment.TrainingModes.ToString().Split(',');
                });
            }
        }

        private void FitnessEquipment_LapToggled(object? sender, EventArgs e)
        {
            LapSplitTime = ((FitnessEquipment)sender!).GeneralData.ElapsedTime;
        }

        [RelayCommand]
        private async Task SetUserConfig() => _ = await FitnessEquipment!.SetUserConfiguration(UserWeight, WheelDiameterOffset, BikeWeight, WheelDiameter, GearRatio);

        [RelayCommand(CanExecute = nameof(CanSetBasicResistance))]
        private async Task SetBasicResistance(string percent) => _ = await FitnessEquipment!.SetBasicResistance(double.Parse(percent));
        private bool CanSetBasicResistance() => FitnessEquipment != null && FitnessEquipment.TrainingModes.HasFlag(SupportedTrainingModes.BasicResistance);

        [RelayCommand(CanExecute = nameof(CanSetTargetPower))]
        private async Task SetTargetPower(string power) => _ = await FitnessEquipment!.SetTargetPower(double.Parse(power));
        private bool CanSetTargetPower() => FitnessEquipment != null && FitnessEquipment.TrainingModes.HasFlag(SupportedTrainingModes.TargetPower);

        [RelayCommand(CanExecute = nameof(CanSetWindResistance))]
        private async Task SetWindResistance() => _ = await FitnessEquipment!.SetWindResistance(0.51, -30, 0.9);
        private bool CanSetWindResistance() => FitnessEquipment != null && FitnessEquipment.TrainingModes.HasFlag(SupportedTrainingModes.Simulation);

        [RelayCommand(CanExecute = nameof(CanSetTrackResistance))]
        private async Task SetTrackResistance(string grade) => _ = await FitnessEquipment!.SetTrackResistance(double.Parse(grade));
        private bool CanSetTrackResistance() => FitnessEquipment != null && FitnessEquipment.TrainingModes.HasFlag(SupportedTrainingModes.Simulation);
    }
}
