using AntPlus;
using AntPlus.DeviceProfiles.FitnessEquipment;
using System.ComponentModel;
using static AntPlus.DeviceProfiles.FitnessEquipment.FitnessEquipment;

namespace AntPlusUsbClient.ViewModels
{
    internal class FitnessEquipmentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FitnessEquipment FitnessEquipment { get; private set; }
        public GeneralDataPage GeneralData { get; private set; }
        public GeneralSettingsPage GeneralSettings { get; private set; }
        public GeneralMetabolicPage GeneralMetabolic { get; private set; }
        public Treadmill Treadmill { get; private set; }
        public Elliptical Elliptical { get; private set; }
        public Rower Rower { get; private set; }
        public Climber Climber { get; private set; }
        public NordicSkier NordicSkier { get; private set; }
        public TrainerStationaryBike TrainerStationaryBike { get; private set; }
        public CommonDataPages.ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public CommonDataPages.ProductInfoPage ProductInfo { get; private set; }

        public FitnessEquipmentViewModel(FitnessEquipment fitnessEquipment)
        {
            FitnessEquipment = fitnessEquipment;
            fitnessEquipment.GeneralDataPageChanged += (s, e) => { GeneralData = e; RaisePropertyChange("GeneralData"); };
            fitnessEquipment.GeneralSettingsPageChanged += (s, e) => { GeneralSettings = e; RaisePropertyChange("GeneralSettings"); };
            fitnessEquipment.GeneralMetabolicPageChanged += (s, e) => { GeneralMetabolic = e; RaisePropertyChange("GeneralMetabolic"); };
            fitnessEquipment.CommonDataPages.ManufacturerInfoPageChanged += (s, e) => { ManufacturerInfo = e; RaisePropertyChange("ManufacturerInfo"); };
            fitnessEquipment.CommonDataPages.ProductInfoPageChanged += (s, e) => { ProductInfo = e; RaisePropertyChange("ProductInfo"); };
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
