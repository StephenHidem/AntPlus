using DeviceProfiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    internal class HeartRateViewModel : INotifyPropertyChanged
    {
        private readonly HeartRate HeartRate;

        public HeartRate.CommonHeartRateData HeartRateData { get; private set; }
        public TimeSpan CumulativeOperatingTime { get; private set; }
        public HeartRate.SwimIntervalPage SwimInterval { get; private set; }
        public HeartRate.CapabilitiesPage Capabilities { get; private set; }
        public HeartRate.ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public HeartRate.ProductInfoPage ProductInfo { get; private set; }
        public HeartRate.PreviousHeartBeatPage PreviousHeartBeat { get; private set; }
        public HeartRate.BatteryStatusPage BatteryStatus { get; private set; }
        public HeartRate.ManufacturerSpecificPage ManufacturerSpecific { get; private set; }

        public RoutedCommand PageRequest { get; private set; } = new RoutedCommand();
        public RoutedCommand SetSportMode { get; private set; } = new RoutedCommand();
        public CommandBinding PageRequestBinding { get; private set; }
        public CommandBinding SetSportModeBinding { get; private set; }
        public IEnumerable<HeartRate.DataPage> DataPageValues => Enum.GetValues(typeof(HeartRate.DataPage)).Cast<HeartRate.DataPage>();
        public IEnumerable<HeartRate.SportMode> SportModeValues => Enum.GetValues(typeof(HeartRate.SportMode)).Cast<HeartRate.SportMode>();

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

            PageRequestBinding = new CommandBinding(PageRequest, PageRequestExecuted, PageRequestCanExecute);
            SetSportModeBinding = new CommandBinding(SetSportMode, SetSportModeExecuted, SetSportModeCanExecute);
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

        private void PageRequestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HeartRate.RequestDataPage((HeartRate.DataPage)e.Parameter, 0x80);
        }

        private void PageRequestCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter == null)
            {
                return;
            }
            switch ((HeartRate.DataPage)e.Parameter)
            {
                case HeartRate.DataPage.Default:
                    e.CanExecute = false;
                    break;
                case HeartRate.DataPage.CumulativeOperatingTime:
                    e.CanExecute = true;
                    break;
                case HeartRate.DataPage.ManufacturerInfo:
                    e.CanExecute = true;
                    break;
                case HeartRate.DataPage.ProductInfo:
                    e.CanExecute = true;
                    break;
                case HeartRate.DataPage.PreviousHeartBeat:
                    e.CanExecute = false;
                    break;
                case HeartRate.DataPage.SwimInterval:
                    e.CanExecute = true;
                    break;
                case HeartRate.DataPage.Capabilities:
                    e.CanExecute = true;
                    break;
                case HeartRate.DataPage.BatteryStatus:
                    e.CanExecute = true;
                    break;
                default:
                    break;
            }
        }

        private void SetSportModeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HeartRate.SetSportMode((HeartRate.SportMode)e.Parameter);
        }

        private void SetSportModeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter == null)
            {
                return;
            }
            switch ((HeartRate.SportMode)e.Parameter)
            {
                case HeartRate.SportMode.None:
                    e.CanExecute = !Capabilities.Enabled.Equals(HeartRate.Features.None);
                    break;
                case HeartRate.SportMode.Running:
                    e.CanExecute = Capabilities.Supported.HasFlag(HeartRate.Features.Running);
                    break;
                case HeartRate.SportMode.Cycling:
                    e.CanExecute = Capabilities.Supported.HasFlag(HeartRate.Features.Cycling);
                    break;
                case HeartRate.SportMode.Swimming:
                    e.CanExecute = Capabilities.Supported.HasFlag(HeartRate.Features.Swimming);
                    break;
                default:
                    break;
            }
        }
    }
}
