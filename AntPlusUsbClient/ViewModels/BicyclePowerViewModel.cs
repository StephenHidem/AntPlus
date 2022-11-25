using AntPlus.DeviceProfiles;
using DeviceProfiles;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class BicyclePowerViewModel : INotifyPropertyChanged
    {
        private readonly BicyclePower BicyclePower;

        public event PropertyChangedEventHandler PropertyChanged;

        public StandardPowerOnly PowerOnly => BicyclePower.PowerOnlySensor;
        public StandardWheelTorqueSensor WheelTorque => BicyclePower.WheelTorqueSensor;
        public StandardCrankTorqueSensor CrankTorque => BicyclePower.CrankTorqueSensor;

        public BicyclePowerViewModel(BicyclePower bicyclePower)
        {
            BicyclePower = bicyclePower;

            // hook up events
            BicyclePower.PowerOnlyChanged += BicyclePower_PowerOnlyChanged;
            BicyclePower.CrankTorquePageChanged += BicyclePower_CrankTorquePageChanged;
            BicyclePower.WheelTorquePageChanged += BicyclePower_WheelTorquePageChanged;
        }

        private void BicyclePower_WheelTorquePageChanged(object sender, StandardWheelTorqueSensor e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WheelTorque"));
        }

        private void BicyclePower_CrankTorquePageChanged(object sender, StandardCrankTorqueSensor e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CrankTorque"));
        }

        private void BicyclePower_PowerOnlyChanged(object sender, StandardPowerOnly e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PowerOnly"));
        }
    }
}
