using AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using System;
using System.ComponentModel;
using static AntPlus.DeviceProfiles.BikeSpeedAndCadence.CommonSpeedCadence;

namespace AntPlusUsbClient.ViewModels
{
    internal class BikeSpeedViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public TimeSpan CumulativeOperatingTime { get; private set; }
        public ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public ProductInfoPage ProductInfo { get; private set; }
        public BatteryStatusPage BatteryStatus { get; private set; }
        public bool Stopped { get; private set; }

        public BikeSpeedSensor BikeSpeedSensor { get; private set; }

        public BikeSpeedViewModel(BikeSpeedSensor bikeSpeed)
        {
            BikeSpeedSensor = bikeSpeed;
            bikeSpeed.BikeSpeedSensorChanged += (s, e) => RaisePropertyChange("BikeSpeedSensor");
            bikeSpeed.CumulativeOperatingTimePageChanged += (s, e) => { CumulativeOperatingTime = e; RaisePropertyChange("CumulativeOperatingTime"); };
            bikeSpeed.ManufacturerInfoPageChanged += (s, e) => { ManufacturerInfo = e; RaisePropertyChange("ManufacturerInfo"); };
            bikeSpeed.ProductInfoPageChanged += (s, e) => { ProductInfo = e; RaisePropertyChange("ProductInfo"); };
            bikeSpeed.BatteryStatusPageChanged += (s, e) => { BatteryStatus = e; RaisePropertyChange("BatteryStatus"); };
            bikeSpeed.StopIndicatorChanged += (s, e) => { Stopped = e; RaisePropertyChange("Stopped"); };
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
