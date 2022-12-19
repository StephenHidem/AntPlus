using AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using System;
using System.ComponentModel;
using static AntPlus.DeviceProfiles.BikeSpeedAndCadence.CommonSpeedCadence;

namespace AntPlusUsbClient.ViewModels
{
    internal class BikeCadenceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BikeCadenceSensor BikeCadenceSensor { get; private set; }
        public TimeSpan CumulativeOperatingTime { get; private set; }
        public ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public ProductInfoPage ProductInfo { get; private set; }
        public BatteryStatusPage BatteryStatus { get; private set; }
        public bool Stopped { get; private set; }

        public BikeCadenceViewModel(BikeCadenceSensor bikeCadence)
        {
            BikeCadenceSensor = bikeCadence;
            bikeCadence.BikeCadenceSensorChanged += (s, e) => RaisePropertyChange("BikeCadenceSensor");
            bikeCadence.CumulativeOperatingTimePageChanged += (s, e) => { CumulativeOperatingTime = e; RaisePropertyChange("CumulativeOperatingTime"); };
            bikeCadence.ManufacturerInfoPageChanged += (s, e) => { ManufacturerInfo = e; RaisePropertyChange("ManufacturerInfo"); };
            bikeCadence.ProductInfoPageChanged += (s, e) => { ProductInfo = e; RaisePropertyChange("ProductInfo"); };
            bikeCadence.BatteryStatusPageChanged += (s, e) => { BatteryStatus = e; RaisePropertyChange("BatteryStatus"); };
            bikeCadence.StopIndicatorChanged += (s, e) => { Stopped = e; RaisePropertyChange("Stopped"); };
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
