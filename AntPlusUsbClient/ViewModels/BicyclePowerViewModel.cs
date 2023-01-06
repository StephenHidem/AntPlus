using AntPlus.DeviceProfiles.BicyclePower;
using System.ComponentModel;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    internal class BicyclePowerViewModel : INotifyPropertyChanged
    {
        private readonly BicyclePower bicyclePower;

        public event PropertyChangedEventHandler PropertyChanged;

        public BicyclePower BicyclePower => bicyclePower;
        public bool EnableCrankTorque => CrankTorque != null;
        public bool EnableWheelTorque => WheelTorque != null;
        public bool EnableCTFSensor => CrankTorqueFrequency != null;
        public bool EnablePowerSensor => CrankTorqueFrequency == null;

        public SensorType SensorType => BicyclePower.Sensor;
        public StandardPowerSensor PowerOnly => BicyclePower.BicyclePowerSensor;
        public StandardWheelTorqueSensor WheelTorque => BicyclePower.BicyclePowerSensor as StandardWheelTorqueSensor;
        public StandardCrankTorqueSensor CrankTorque => BicyclePower.BicyclePowerSensor as StandardCrankTorqueSensor;
        public CrankTorqueFrequencySensor CrankTorqueFrequency => BicyclePower.CTFSensor;

        public TorqueEffectivenessAndPedalSmoothness TEPS { get; private set; }
        public MeasurementOutputData MeasurementOutput { get; private set; }

        public Calibration Calibration => BicyclePower.Calibration;
        public Parameters Parameters { get; private set; }

        public RoutedCommand ManualCalRequest { get; private set; } = new RoutedCommand();
        public RoutedCommand SetAutoZeroConfig { get; private set; } = new RoutedCommand();
        public RoutedCommand GetCustomCalibrationParameters { get; private set; } = new RoutedCommand();
        public RoutedCommand SetCustomCalibrationParameters { get; private set; } = new RoutedCommand();
        public RoutedCommand SaveSlope { get; private set; } = new RoutedCommand();
        public RoutedCommand SaveSerialNumber { get; private set; } = new RoutedCommand();
        public RoutedCommand GetParameters { get; private set; } = new RoutedCommand();
        public RoutedCommand SetCrankLength { get; private set; } = new RoutedCommand();

        public CommandBinding[] CommandBindings { get; private set; }

        public BicyclePowerViewModel(BicyclePower bicyclePower)
        {
            this.bicyclePower = bicyclePower;

            // hook up events
            if (PowerOnly != null)
            {
                Parameters = PowerOnly.Parameters;
                PowerOnly.ParametersChanged += (s, e) => RaisePropertyChange("Parameters");
                PowerOnly.PowerOnlyChanged += (s, e) => RaisePropertyChange("PowerOnly");
                PowerOnly.TEPSPageChanged += (s, e) => { TEPS = e; RaisePropertyChange("TEPS"); };
                PowerOnly.MeasurementOutputDataChanged += (s, e) => { MeasurementOutput = e; RaisePropertyChange("MeasurementOutput"); };
                if (CrankTorque != null)
                {
                    CrankTorque.CrankTorquePageChanged += (s, e) => RaisePropertyChange("CrankTorque");
                }
                if (WheelTorque != null)
                {
                    WheelTorque.WheelTorquePageChanged += (s, e) => RaisePropertyChange("WheelTorque");
                }
            }
            BicyclePower.BicycleCalibrationPageChanged += (s, e) => RaisePropertyChange("Calibration");
            BicyclePower.CrankTorqueFrequencyPageChanged += (s, e) => RaisePropertyChange("CrankTorqueFrequency");

            CommandBindings = new CommandBinding[] {
                new CommandBinding(ManualCalRequest, (s, e) => Calibration.RequestManualCalibration()),
                new CommandBinding(SetAutoZeroConfig, (s, e) => Calibration.SetAutoZeroConfiguration(Calibration.AutoZero.On)),
                new CommandBinding(GetCustomCalibrationParameters, (s, e) => Calibration.RequestCustomParameters()),
                new CommandBinding(SetCustomCalibrationParameters, (s, e) => Calibration.SetCustomParameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 })),
                new CommandBinding(SaveSlope, (s, e) => CrankTorqueFrequency.SaveSlopeToFlash(300)),
                new CommandBinding(SaveSerialNumber, (s, e) => CrankTorqueFrequency.SaveSerialNumberToFlash(1122)),
                new CommandBinding(GetParameters, (s, e) => Parameters.GetParameters((Subpage)e.Parameter)),
                new CommandBinding(SetCrankLength, (s, e) => Parameters.SetCrankLength(double.Parse(e.Parameter.ToString())))
            };
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
