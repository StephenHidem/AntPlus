using AntPlus.DeviceProfiles.FitnessEquipment;
using System.ComponentModel;
using static AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment;

namespace AntPlusUsbClient.ViewModels
{
    internal class FitnessEquipmentViewModel : INotifyPropertyChanged
    {
        private FitnessEquipment fitnessEquipment;

        public event PropertyChangedEventHandler PropertyChanged;

        public FitnessEquipment FitnessEquipment => fitnessEquipment;
        public GeneralDataPage GeneralData { get; private set; }
        public GeneralSettingsPage GeneralSettings { get; private set; }
        public GeneralMetabolicPage GeneralMetabolic { get; private set; }
        public Treadmill Treadmill { get; private set; }
        public Elliptical Elliptical { get; private set; }
        public Rower Rower { get; private set; }
        public Climber Climber { get; private set; }
        public NordicSkier NordicSkier { get; private set; }
        public TrainerStationaryBike TrainerStationaryBike { get; private set; }

        public FitnessEquipmentViewModel(FitnessEquipment fitnessEquipment)
        {
            this.fitnessEquipment = fitnessEquipment;
            fitnessEquipment.GeneralDataPageChanged += (s, e) => { GeneralData = e; RaisePropertyChange("GeneralData"); };
            fitnessEquipment.GeneralSettingsPageChanged += (s, e) => { GeneralSettings = e; RaisePropertyChange("GeneralSettings"); };
            fitnessEquipment.GeneralMetabolicPageChanged += (s, e) => { GeneralMetabolic = e; RaisePropertyChange("GeneralMetabolic"); };
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
