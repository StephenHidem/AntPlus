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
        public HeartRate.HeartbeatEventType HeartbeatEventType { get; private set; }
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
            HeartRate.HeartRateChanged += (s, e) => { HeartRateData = e; RaisePropertyChange("HeartRateData"); };
            HeartRate.CumulativeOperatingTimePageChanged += (s, e) => { CumulativeOperatingTime = e; RaisePropertyChange("CumulativeOperatingTime"); };
            HeartRate.ManufacturerInfoPageChanged += (s, e) => { ManufacturerInfo = e; RaisePropertyChange("ManufacturerInfo"); };
            HeartRate.ProductInfoPageChanged += (s, e) => { ProductInfo = e; RaisePropertyChange("ProductInfo"); };
            HeartRate.PreviousHeartBeatPageChanged += (s, e) => { PreviousHeartBeat = e; RaisePropertyChange("PreviousHeartBeat"); };
            HeartRate.SwimIntervalPageChanged += (s, e) => { SwimInterval = e; RaisePropertyChange("SwimInterval"); };
            HeartRate.CapabilitiesPageChanged += (s, e) => { Capabilities = e; RaisePropertyChange("Capabilities"); };
            HeartRate.BatteryStatusPageChanged += (s, e) => { BatteryStatus = e; RaisePropertyChange("BatteryStatus"); };
            HeartRate.HeartbeatEventTypeChanged += (s, e) => { HeartbeatEventType = e; RaisePropertyChange("HeartbeatEventType"); };
            HeartRate.ManufacturerSpecificPageChanged += (s, e) => { ManufacturerSpecific = e; RaisePropertyChange("ManufacturerSpecific"); };

            PageRequestBinding = new CommandBinding(PageRequest, PageRequestExecuted, PageRequestCanExecute);
            SetSportModeBinding = new CommandBinding(SetSportMode, SetSportModeExecuted, SetSportModeCanExecute);
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PageRequestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            HeartRate.RequestDataPage((HeartRate.DataPage)e.Parameter);
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
