using AntPlus.DeviceProfiles.FitnessEquipment;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class NordicSkierViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly FitnessEquipment fitnessEquipment;

        public int StrideCount => fitnessEquipment.NordicSkier.StrideCount;
        public byte Cadence => fitnessEquipment.NordicSkier.Cadence;
        public int InstantaneousPower => fitnessEquipment.NordicSkier.InstantaneousPower;

        public NordicSkierViewModel(FitnessEquipment fitnessEquipment)
        {
            this.fitnessEquipment = fitnessEquipment;
            fitnessEquipment.NordicSkier.NordicSkierChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
