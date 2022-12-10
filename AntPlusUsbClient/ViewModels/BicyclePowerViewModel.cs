﻿using AntPlus.DeviceProfiles;
using DeviceProfiles;
using System.ComponentModel;
using System.Windows.Input;
using static DeviceProfiles.BicyclePower;
using static DeviceProfiles.BicyclePower.BicyclePowerParameters;

namespace AntPlusUsbClient.ViewModels
{
    internal class BicyclePowerViewModel : INotifyPropertyChanged
    {
        private readonly BicyclePower BicyclePower;

        public event PropertyChangedEventHandler PropertyChanged;

        public SensorType SensorType => BicyclePower.Sensor;
        public StandardPowerOnly PowerOnly => BicyclePower.PowerOnlySensor;
        public StandardWheelTorqueSensor WheelTorque => BicyclePower.WheelTorqueSensor;
        public StandardCrankTorqueSensor CrankTorque => BicyclePower.CrankTorqueSensor;
        public TorqueEffectivenessAndPedalSmoothness TEPS => BicyclePower.TEPS;
        public BicycleCalibrationData CalibrationData => BicyclePower.CalibrationData;
        public MeasurementOutputData MeasurementOutput { get; private set; }
        public CrankTorqueFrequencySensor CrankTorqueFrequency => BicyclePower.CTFSensor;
        public BicyclePowerParameters Parameters => BicyclePower.Parameters;

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
            BicyclePower.PowerOnlyChanged += (s, e) => RaisePropertyChange("PowerOnly");
            BicyclePower.CrankTorquePageChanged += (s, e) => RaisePropertyChange("CrankTorque");
            BicyclePower.WheelTorquePageChanged += (s, e) => RaisePropertyChange("WheelTorque");
            BicyclePower.TEPSPageChanged += (s, e) => RaisePropertyChange("TEPS");
            BicyclePower.BicycleCalibrationPageChanged += (s, e) => RaisePropertyChange("CalibrationData");
            BicyclePower.MeasurementOutputDataChanged += (s, e) => { MeasurementOutput = e; RaisePropertyChange("MeasurementOutput"); };
            BicyclePower.CrankTorqueFrequencyPageChanged += (s, e) => RaisePropertyChange("CrankTorqueFrequency");
            BicyclePower.ParametersChanged += (s, e) => RaisePropertyChange("Parameters");

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
            CalibrationData.CustomParametersRequest();
        }

        private void GetCustomCalibrationParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SetCustomCalibrationParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CalibrationData.SetCustomParameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 });
        }

        private void SetCustomCalibrationParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ManualCalRequestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CalibrationData.RequestManualCalibration();
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

        private void GetParametersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            BicyclePower.Parameters.GetParameters(ParameterSubpage.CrankParameters);
            BicyclePower.Parameters.GetParameters(ParameterSubpage.PowerPhaseConfiguration);
            BicyclePower.Parameters.GetParameters(ParameterSubpage.RiderPositionConfiguration);
            BicyclePower.Parameters.GetParameters(ParameterSubpage.AdvancedCapabilities1);
            BicyclePower.Parameters.GetParameters(ParameterSubpage.AdvancedCapabilities2);
        }

        private void GetParametersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SetCrankLengthExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            BicyclePower.Parameters.SetCrankLength(double.Parse(e.Parameter.ToString()));
        }

        private void SetCrankLengthCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
