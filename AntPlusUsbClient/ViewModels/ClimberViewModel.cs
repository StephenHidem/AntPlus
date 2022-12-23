using AntPlus.DeviceProfiles.FitnessEquipment;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class ClimberViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly FitnessEquipment fitnessEquipment;

        public int StrideCycles => fitnessEquipment.Climber.StrideCycles;
        public byte Cadence => fitnessEquipment.Climber.Cadence;
        public int InstantaneousPower => fitnessEquipment.Climber.InstantaneousPower;

        public ClimberViewModel(FitnessEquipment fitnessEquipment)
        {
            this.fitnessEquipment = fitnessEquipment;
            fitnessEquipment.Climber.ClimberChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
