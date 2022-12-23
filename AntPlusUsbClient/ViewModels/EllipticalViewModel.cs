using AntPlus.DeviceProfiles.FitnessEquipment;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class EllipticalViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly FitnessEquipment fitnessEquipment;

        public int StrideCount => fitnessEquipment.Elliptical.StrideCount;
        public byte Cadence => fitnessEquipment.Elliptical.Cadence;
        public double PosVerticalDistance => fitnessEquipment.Elliptical.PosVerticalDistance;
        public int InstantaneousPower => fitnessEquipment.Elliptical.InstantaneousPower;

        public EllipticalViewModel(FitnessEquipment fitnessEquipment)
        {
            this.fitnessEquipment = fitnessEquipment;
            fitnessEquipment.Elliptical.EllipticalChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
