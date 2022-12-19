using AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class BikeSpeedAndCadenceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CombinedSpeedAndCadenceSensor CombinedSpeedAndCadenceSensor { get; private set; }

        public BikeSpeedAndCadenceViewModel(CombinedSpeedAndCadenceSensor combinedSpeedAndCadence)
        {
            CombinedSpeedAndCadenceSensor = combinedSpeedAndCadence;
            combinedSpeedAndCadence.CombinedSpeedAndCadenceSensorChanged += (s, e) => RaisePropertyChange("CombinedSpeedAndCadenceSensor");
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
