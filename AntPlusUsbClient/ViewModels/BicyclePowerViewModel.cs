using AntPlus.DeviceProfiles;
using DeviceProfiles;
using System.ComponentModel;
using System.Windows.Input;
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
        public MeasurementOutputData MeasurementOutput { get; private set; }
        public CrankTorqueFrequencySensor CrankTorqueFrequency => BicyclePower.CrankTorqueFrequency;

        public RoutedCommand ManualCalRequest { get; private set; } = new RoutedCommand();
        public RoutedCommand SetAutoZeroConfig { get; private set; } = new RoutedCommand();
        public RoutedCommand GetCustomParameters { get; private set; } = new RoutedCommand();
        public RoutedCommand SetCustomParameters { get; private set; } = new RoutedCommand();
        public RoutedCommand SaveSlope { get; private set; } = new RoutedCommand();
        public RoutedCommand SaveSerialNumber { get; private set; } = new RoutedCommand();

        public CommandBinding[] CommandBindings { get; private set; }

        public BicyclePowerViewModel(BicyclePower bicyclePower)
        {
            BicyclePower = bicyclePower;

            // hook up events
            BicyclePower.PowerOnlyChanged += (s, e) => RaisePropertyChange("PowerOnly");
            BicyclePower.CrankTorquePageChanged += (s, e) => RaisePropertyChange("CrankTorque");
            BicyclePower.WheelTorquePageChanged += (s, e) => RaisePropertyChange("WheelTorque");
            BicyclePower.TEPSPageChanged += (s, e) => RaisePropertyChange("TEPS");
            BicyclePower.BicycleCalibrationPageChanged += (s, e) => RaisePropertyChange("CalibrationData");
            BicyclePower.MeasurementOutputDataChanged += (s, e) => { MeasurementOutput = e; RaisePropertyChange("MeasurementOutput"); };
            BicyclePower.CrankTorqueFrequencyPageChanged += (s, e) => RaisePropertyChange("CrankTorqueFrequency");

            CommandBindings = new CommandBinding[] {
                new CommandBinding(ManualCalRequest, ManualCalRequestExecuted, RequestManualCalCanExecute),
                new CommandBinding(SetAutoZeroConfig, SetAutoZeroExecuted, SetAutoZeroCanExecute),
                new CommandBinding(GetCustomParameters, GetCustomParametersExecuted, GetCustomParametersCanExecute),
                new CommandBinding(SetCustomParameters, SetCustomParametersExecuted, SetCustomParametersCanExecute),
                new CommandBinding(SaveSlope, SaveSlopeExecuted, SaveSlopeCanExecute),
                new CommandBinding(SaveSerialNumber, SaveSerialNumberExecuted, SaveSerialNumberCanExecute)
            };
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveSlopeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CrankTorqueFrequency.SaveSlopeToFlash(300);
        }

        private void SaveSlopeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveSerialNumberExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CrankTorqueFrequency.SaveSerialNumberToFlash(1122);
        }

        private void SaveSerialNumberCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void GetCustomParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CalibrationData.CustomParametersRequest();
        }

        private void GetCustomParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SetCustomParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CalibrationData.SetCustomParameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 });
        }

        private void SetCustomParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ManualCalRequestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CalibrationData.ManualCalibrationRequest();
        }

        private void RequestManualCalCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SetAutoZeroExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CalibrationData.SetAutoZeroConfiguration(BicycleCalibrationData.AutoZero.On);
        }

        private void SetAutoZeroCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
