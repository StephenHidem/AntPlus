using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAntGrpcClient.Views.FitnessEquipment;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;
using static SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment.Equipment;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class FitnessEquipmentViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ILogger<FitnessEquipmentViewModel> _logger;

        [ObservableProperty]
        private Equipment fitnessEquipment = null!;
        [ObservableProperty]
        private ContentView specificEquipmentView = null!;

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
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(
            nameof(SetBasicResistanceCommand),
            nameof(SetTargetPowerCommand),
            nameof(SetWindResistanceCommand),
            nameof(SetTrackResistanceCommand))]
        private SupportedTrainingModes supportedTrainingModes;
        [ObservableProperty]
        private string[]? capabilities;

        public FitnessEquipmentViewModel(ILogger<FitnessEquipmentViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("Created FitnessEquipment");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _logger.LogInformation($"{nameof(ApplyQueryAttributes)}");
            FitnessEquipment = (Equipment)query["Sensor"];
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
            switch (e.PropertyName)
            {
                case nameof(FitnessEquipment.TrainingModes):
                    SupportedTrainingModes = FitnessEquipment.TrainingModes;
                    Capabilities = FitnessEquipment.TrainingModes.ToString().Split(',');
                    break;
                default:
                    break;
            }
        }

        private void FitnessEquipment_LapToggled(object? sender, EventArgs e)
        {
            LapSplitTime = ((Equipment)sender!).GeneralData.ElapsedTime;
        }

        [RelayCommand]
        private async Task<MessagingReturnCode> SetUserConfig() => await FitnessEquipment.SetUserConfiguration(UserWeight, WheelDiameterOffset, BikeWeight, WheelDiameter, GearRatio);

        [RelayCommand(CanExecute = nameof(CanSetBasicResistance))]
        private async Task<MessagingReturnCode> SetBasicResistance(string percent) => await FitnessEquipment.SetBasicResistance(double.Parse(percent));
        private bool CanSetBasicResistance() => FitnessEquipment.TrainingModes.HasFlag(Equipment.SupportedTrainingModes.BasicResistance);

        [RelayCommand(CanExecute = nameof(CanSetTargetPower))]
        private async Task<MessagingReturnCode> SetTargetPower(string power) => await FitnessEquipment.SetTargetPower(double.Parse(power));
        private bool CanSetTargetPower() => FitnessEquipment.TrainingModes.HasFlag(Equipment.SupportedTrainingModes.TargetPower);

        [RelayCommand(CanExecute = nameof(CanSetWindResistance))]
        private async Task<MessagingReturnCode> SetWindResistance() => await FitnessEquipment.SetWindResistance(0.51, -30, 0.9);
        private bool CanSetWindResistance() => FitnessEquipment.TrainingModes.HasFlag(Equipment.SupportedTrainingModes.Simulation);

        [RelayCommand(CanExecute = nameof(CanSetTrackResistance))]
        private async Task<MessagingReturnCode> SetTrackResistance(string grade) => await FitnessEquipment.SetTrackResistance(double.Parse(grade));
        private bool CanSetTrackResistance() => FitnessEquipment.TrainingModes.HasFlag(Equipment.SupportedTrainingModes.Simulation);
    }
}
