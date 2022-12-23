using AntPlus.DeviceProfiles.FitnessEquipment;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class TrainerStationaryBikeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly FitnessEquipment fitnessEquipment;

        public int StrideCount => fitnessEquipment.TrainerStationaryBike.StrideCount;
        public byte Cadence => fitnessEquipment.TrainerStationaryBike.Cadence;
        public int InstantaneousPower => fitnessEquipment.TrainerStationaryBike.InstantaneousPower;

        public TrainerStationaryBikeViewModel(FitnessEquipment fitnessEquipment)
        {
            this.fitnessEquipment = fitnessEquipment;
            fitnessEquipment.TrainerStationaryBike.TrainerChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
