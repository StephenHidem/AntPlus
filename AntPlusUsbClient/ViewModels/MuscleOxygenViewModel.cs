using AntPlus;
using AntPlus.DeviceProfiles;
using System.ComponentModel;
using static AntPlus.DeviceProfiles.MuscleOxygen;

namespace AntPlusUsbClient.ViewModels
{
    internal class MuscleOxygenViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly MuscleOxygen muscleOxygen;

        public bool UtcTimeRequired => muscleOxygen.UtcTimeRequired;
        public bool SupportsAntFs => muscleOxygen.SupportsAntFs;
        public MeasurementInterval Interval => muscleOxygen.Interval;
        public TotalHemoglobin TotalHemoglobinConcentration => muscleOxygen.TotalHemoglobinConcentration;
        public SaturatedHemoglobin PreviousSaturatedHemoglobin => muscleOxygen.PreviousSaturatedHemoglobin;
        public SaturatedHemoglobin CurrentSaturatedHemoglobin => muscleOxygen.CurrentSaturatedHemoglobin;
        public CommonDataPages.ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public CommonDataPages.ProductInfoPage ProductInfo { get; private set; }
        public CommonDataPages.BatteryStatusPage BatteryStatus { get; private set; }

        public MuscleOxygenViewModel(MuscleOxygen muscleOxygen)
        {
            this.muscleOxygen = muscleOxygen;
            muscleOxygen.MuscleOxygenChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
            muscleOxygen.CommonDataPages.ManufacturerInfoPageChanged += (s, e) => { ManufacturerInfo = e; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CommonDataPages")); };
            muscleOxygen.CommonDataPages.ProductInfoPageChanged += (s, e) => { ProductInfo = e; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CommonDataPages")); };
            muscleOxygen.CommonDataPages.BatteryStatusPageChanged += (s, e) => { BatteryStatus = e; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CommonDataPages")); };
        }
    }
}
