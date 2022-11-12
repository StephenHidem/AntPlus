using DeviceProfiles;
using System;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class HeartRateViewModel : INotifyPropertyChanged
    {
        private HeartRate HeartRate;

        public HeartRate.CommonHeartRateData HeartRateData { get; private set; }
        public TimeSpan CumulativeOperatingTime { get; private set; }
        public HeartRate.SwimIntervalPage SwimInterval { get; private set; }
        public HeartRate.CapabilitiesPage Capabilities { get; private set; }
        public HeartRate.ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public HeartRate.ProductInfoPage ProductInfo { get; private set; }
        public HeartRate.PreviousHeartBeatPage PreviousHeartBeat { get; private set; }
        public HeartRate.BatteryStatusPage BatteryStatus { get; private set; }
        public HeartRate.ManufacturerSpecificPage ManufacturerSpecific { get; private set; }

        public HeartRateViewModel(HeartRate heartRate)
        {
            HeartRate = heartRate;

            // hook up events
            HeartRate.HeartRateChanged += HeartRate_HeartRateChanged;
            HeartRate.CumulativeOperatingTimePageChanged += HeartRate_CumulativeOperatingTimePageChanged;
            HeartRate.ManufacturerInfoPageChanged += HeartRate_ManufacturerInfoPageChanged;
            HeartRate.ProductInfoPageChanged += HeartRate_ProductInfoPageChanged;
            HeartRate.PreviousHeartBeatPageChanged += HeartRate_PreviousHeartBeatPageChanged;
            HeartRate.SwimIntervalPageChanged += HeartRate_SwimIntervalPageChanged;
            HeartRate.CapabilitiesPageChanged += HeartRate_CapabilitiesPageChanged;
            HeartRate.BatteryStatusPageChanged += HeartRate_BatteryStatusPageChanged;
            HeartRate.ManufacturerSpecificPageChanged += HeartRate_ManufacturerSpecificPageChanged;
        }

        private void HeartRate_HeartRateChanged(object sender, HeartRate.CommonHeartRateData e)
        {
            HeartRateData = e;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HeartRateData"));
        }

        private void HeartRate_CumulativeOperatingTimePageChanged(object sender, TimeSpan e)
        {
            CumulativeOperatingTime = e;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CumulativeOperatingTime"));
        }

        private void HeartRate_ManufacturerInfoPageChanged(object sender, HeartRate.ManufacturerInfoPage e)
        {
            ManufacturerInfo = e;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ManufacturerInfo"));
        }

        private void HeartRate_ProductInfoPageChanged(object sender, HeartRate.ProductInfoPage e)
        {
            ProductInfo = e;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductInfo"));
        }

        private void HeartRate_PreviousHeartBeatPageChanged(object sender, HeartRate.PreviousHeartBeatPage e)
        {
            PreviousHeartBeat = e;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreviousHeartBeat"));
        }

        private void HeartRate_SwimIntervalPageChanged(object sender, HeartRate.SwimIntervalPage e)
        {
            SwimInterval = e;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SwimInterval"));
        }

        private void HeartRate_CapabilitiesPageChanged(object sender, HeartRate.CapabilitiesPage e)
        {
            Capabilities = e;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Capabilities"));
        }

        private void HeartRate_BatteryStatusPageChanged(object sender, HeartRate.BatteryStatusPage e)
        {
            BatteryStatus = e;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BatteryStatus"));
        }

        private void HeartRate_ManufacturerSpecificPageChanged(object sender, HeartRate.ManufacturerSpecificPage e)
        {
            ManufacturerSpecific = e;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ManufacturerSpecific"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
