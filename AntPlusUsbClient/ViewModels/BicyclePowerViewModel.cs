﻿using AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    public class BicyclePowerViewModel
    {
        private readonly BicyclePower bicyclePower;

        public BicyclePower BicyclePower => bicyclePower;
        public SensorType SensorType => BicyclePower.Sensor;

        public RoutedCommand ManualCalRequest { get; private set; } = new RoutedCommand();
        public RoutedCommand SetAutoZeroConfig { get; private set; } = new RoutedCommand();
        public RoutedCommand GetCustomCalibrationParameters { get; private set; } = new RoutedCommand();
        public RoutedCommand SetCustomCalibrationParameters { get; private set; } = new RoutedCommand();
        public RoutedCommand GetParameters { get; private set; } = new RoutedCommand();
        public RoutedCommand SetCrankLength { get; private set; } = new RoutedCommand();

        public CommandBinding[] CommandBindings { get; private set; }

        public BicyclePowerViewModel(BicyclePower bicyclePower)
        {
            this.bicyclePower = bicyclePower;

            CommandBindings = new CommandBinding[] {
                new CommandBinding(ManualCalRequest, (s, e) => BicyclePower.PowerOnlySensor.Calibration.RequestManualCalibration()),
                new CommandBinding(SetAutoZeroConfig, (s, e) => BicyclePower.PowerOnlySensor.Calibration.SetAutoZeroConfiguration(Calibration.AutoZero.On)),
                new CommandBinding(GetCustomCalibrationParameters, (s, e) => BicyclePower.PowerOnlySensor.Calibration.RequestCustomParameters()),
                new CommandBinding(SetCustomCalibrationParameters, (s, e) => BicyclePower.PowerOnlySensor.Calibration.SetCustomParameters(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66 })),
                new CommandBinding(GetParameters, (s, e) => BicyclePower.PowerOnlySensor.Parameters.GetParameters((Subpage)e.Parameter)),
                new CommandBinding(SetCrankLength, (s, e) => BicyclePower.PowerOnlySensor.Parameters.SetCrankLength(double.Parse(e.Parameter.ToString())))
            };
        }
    }
}
