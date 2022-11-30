using AntPlus.DeviceProfiles;
using DeviceProfiles;
using System.ComponentModel;
using static DeviceProfiles.BicyclePower;

namespace AntPlusUsbClient.ViewModels
{
    internal class BicyclePowerViewModel : INotifyPropertyChanged
    {
        private readonly BicyclePower BicyclePower;

        public event PropertyChangedEventHandler PropertyChanged;

        public StandardPowerOnly PowerOnly => BicyclePower.PowerOnlySensor;
        public StandardWheelTorqueSensor WheelTorque => BicyclePower.WheelTorqueSensor;
        public StandardCrankTorqueSensor CrankTorque => BicyclePower.CrankTorqueSensor;
        public TorqueEffectivenessAndPedalSmoothness TEPS => BicyclePower.TEPS;
        public BicycleCalibrationData CalibrationData => BicyclePower.CalibrationData;

        public BicyclePowerViewModel(BicyclePower bicyclePower)
        {
            BicyclePower = bicyclePower;

            // hook up events
            BicyclePower.PowerOnlyChanged += BicyclePower_PowerOnlyChanged;
            BicyclePower.CrankTorquePageChanged += BicyclePower_CrankTorquePageChanged;
            BicyclePower.WheelTorquePageChanged += BicyclePower_WheelTorquePageChanged;
            BicyclePower.TEPSPageChanged += BicyclePower_TEPSPageChanged;
            BicyclePower.BicycleCalibrationPageChanged += BicyclePower_BicycleCalibrationPageChanged;
        }

        private void BicyclePower_BicycleCalibrationPageChanged(object sender, BicyclePower.BicycleCalibrationData e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CalibrationData"));
        }

        private void BicyclePower_TEPSPageChanged(object sender, TorqueEffectivenessAndPedalSmoothness e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TorqueEffectivenessPedalSmootness"));
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
