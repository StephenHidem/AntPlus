using DeviceProfiles;
using DeviceProfiles.BicyclePower;
using System.ComponentModel;
using System.Windows.Input;
using static AntPlus.CommonDataPages;

namespace AntPlusUsbClient.ViewModels
{
    internal class BicyclePowerViewModel : INotifyPropertyChanged
    {
        private readonly BicyclePower BicyclePower;

        public event PropertyChangedEventHandler PropertyChanged;

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
        public ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public ProductInfoPage ProductInfo { get; private set; }
        public BatteryStatusPage BatteryStatus { get; private set; }

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
            BicyclePower = bicyclePower;

            // hook up events
            if (PowerOnly != null)
            {
                Parameters = PowerOnly.Parameters;
                PowerOnly.ParametersChanged += (s, e) => RaisePropertyChange("Parameters");
                PowerOnly.PowerOnlyChanged += (s, e) => RaisePropertyChange("PowerOnly");
                PowerOnly.TEPSPageChanged += (s, e) => { TEPS = e; RaisePropertyChange("TEPS"); };
                PowerOnly.MeasurementOutputDataChanged += (s, e) => { MeasurementOutput = e; RaisePropertyChange("MeasurementOutput"); };
                PowerOnly.CommonDataPages.ManufacturerInfoPageChanged += (s, e) => { ManufacturerInfo = e; RaisePropertyChange("ManufacturerInfo"); };
                PowerOnly.CommonDataPages.ProductInfoPageChanged += (s, e) => { ProductInfo = e; RaisePropertyChange("ProductInfo"); };
                PowerOnly.CommonDataPages.BatteryStatusPageChanged += (s, e) => { BatteryStatus = e; RaisePropertyChange("BatteryStatus"); };
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
                new CommandBinding(ManualCalRequest, ManualCalRequestExecuted, RequestManualCalCanExecute),
                new CommandBinding(SetAutoZeroConfig, SetAutoZeroExecuted, SetAutoZeroCanExecute),
                new CommandBinding(GetCustomCalibrationParameters, GetCustomCalibrationParametersExecuted, GetCustomCalibrationParametersCanExecute),
                new CommandBinding(SetCustomCalibrationParameters, SetCustomCalibrationParametersExecuted, SetCustomCalibrationParametersCanExecute),
                new CommandBinding(SaveSlope, SaveSlopeExecuted, SaveSlopeCanExecute),
                new CommandBinding(SaveSerialNumber, SaveSerialNumberExecuted, SaveSerialNumberCanExecute),
                new CommandBinding(GetParameters, GetParametersExecuted, GetParametersCanExecute),
                new CommandBinding(SetCrankLength, SetCrankLengthExecuted, SetCrankLengthCanExecute)
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

        private void GetCustomCalibrationParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Calibration.RequestCustomParameters();
        }

        private void GetCustomCalibrationParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SetCustomCalibrationParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Calibration.SetCustomParameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 });
        }

        private void SetCustomCalibrationParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ManualCalRequestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Calibration.RequestManualCalibration();
        }

        private void RequestManualCalCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SetAutoZeroExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Calibration.SetAutoZeroConfiguration(Calibration.AutoZero.On);
        }

        private void SetAutoZeroCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void GetParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Parameters.GetParameters((Subpage)e.Parameter);
        }

        private void GetParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SetCrankLengthExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Parameters.SetCrankLength(double.Parse(e.Parameter.ToString()));
        }

        private void SetCrankLengthCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
