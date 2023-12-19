using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiAntClientApp.Views.FitnessEquipment;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntClientApp.ViewModels
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
            switch (FitnessEquipment.GeneralData.EquipmentType)
            {
                case Equipment.FitnessEquipmentType.Treadmill:
                    SpecificEquipmentView = new TreadmillView(FitnessEquipment.Treadmill);
                    break;
                case Equipment.FitnessEquipmentType.Elliptical:
                    SpecificEquipmentView = new EllipticalView(FitnessEquipment.Elliptical);
                    break;
                case Equipment.FitnessEquipmentType.Rower:
                    SpecificEquipmentView = new RowerView(FitnessEquipment.Rower);
                    break;
                case Equipment.FitnessEquipmentType.Climber:
                    SpecificEquipmentView = new ClimberView(FitnessEquipment.Climber);
                    break;
                case Equipment.FitnessEquipmentType.NordicSkier:
                    SpecificEquipmentView = new NordicSkierView(FitnessEquipment.NordicSkier);
                    break;
                case Equipment.FitnessEquipmentType.TrainerStationaryBike:
                    SpecificEquipmentView = new TrainerStationaryBikeView(FitnessEquipment.TrainerStationaryBike);
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
        private async Task<MessagingReturnCode> FECapabilitiesRequest() => await FitnessEquipment.RequestFECapabilities();
        [RelayCommand]
        private async Task<MessagingReturnCode> SetUserConfig() => await FitnessEquipment.SetUserConfiguration(UserWeight, WheelDiameterOffset, BikeWeight, WheelDiameter, GearRatio);
        [RelayCommand]
        private async Task<MessagingReturnCode> SetBasicResistance(string percent) => await FitnessEquipment.SetBasicResistance(double.Parse(percent));
        [RelayCommand]
        private async Task<MessagingReturnCode> SetTargetPower(string power) => await FitnessEquipment.SetTargetPower(double.Parse(power));
        [RelayCommand]
        private async Task<MessagingReturnCode> SetWindResistance() => await FitnessEquipment.SetWindResistance(0.51, -30, 0.9);
        [RelayCommand]
        private async Task<MessagingReturnCode> SetTrackResistance() => await FitnessEquipment.SetTrackResistance(-15);
    }
}
