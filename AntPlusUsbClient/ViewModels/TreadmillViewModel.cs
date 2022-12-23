using AntPlus.DeviceProfiles.FitnessEquipment;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class TreadmillViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly FitnessEquipment fitnesssEquipment;

        public byte Cadence => fitnesssEquipment.Treadmill.Cadence;
        public double NegVerticalDistance => fitnesssEquipment.Treadmill.NegVerticalDistance;
        public double PosVerticalDistance => fitnesssEquipment.Treadmill.PosVerticalDistance;

        public TreadmillViewModel(FitnessEquipment fitnessEquipment)
        {
            fitnesssEquipment = fitnessEquipment;
            fitnessEquipment.Treadmill.TreadmillChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }
    }
}
