using AntPlus.DeviceProfiles.FitnessEquipment;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class RowerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly FitnessEquipment fitnessEquipment;

        public int StrokeCount => fitnessEquipment.Rower.StrokeCount;
        public byte Cadence => fitnessEquipment.Rower.Cadence;
        public int InstantaneousPower => fitnessEquipment.Rower.InstantaneousPower;

        public RowerViewModel(FitnessEquipment fitnessEquipment)
        {
            this.fitnessEquipment = fitnessEquipment;
            fitnessEquipment.Rower.RowerChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
