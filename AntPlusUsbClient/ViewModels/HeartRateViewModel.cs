using AntPlus;
using AntPlus.DeviceProfiles.HeartRate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    internal class HeartRateViewModel : INotifyPropertyChanged
    {
        private readonly HeartRate heartRate;

        public HeartRate HeartRate => heartRate;

        public bool ApplyFeature { get; set; }
        public bool EnableGymMode { get; set; }

        public RoutedCommand PageRequest { get; private set; } = new RoutedCommand();
        public RoutedCommand SetSportMode { get; private set; } = new RoutedCommand();
        public RoutedCommand SetHRFeature { get; private set; } = new RoutedCommand();
        public CommandBinding PageRequestBinding { get; private set; }
        public CommandBinding SetSportModeBinding { get; private set; }
        public CommandBinding SetHRFeatureBinding { get; private set; }
        public IEnumerable<HeartRate.DataPage> DataPageValues => Enum.GetValues(typeof(HeartRate.DataPage)).Cast<HeartRate.DataPage>();
        public IEnumerable<SportMode> SportModeValues => Enum.GetValues(typeof(SportMode)).Cast<SportMode>();

        public HeartRateViewModel(HeartRate heartRate)
        {
            this.heartRate = heartRate;

            PageRequestBinding = new CommandBinding(PageRequest, PageRequestExecuted, PageRequestCanExecute);
            SetSportModeBinding = new CommandBinding(SetSportMode, SetSportModeExecuted, SetSportModeCanExecute);
            SetHRFeatureBinding = new CommandBinding(SetHRFeature, SetHRFeatureExecuted, SetHRFeatureCanExecute);
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PageRequestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            heartRate.RequestDataPage((HeartRate.DataPage)e.Parameter);
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
            heartRate.SetSportMode((SportMode)e.Parameter);
        }

        private void SetSportModeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter == null)
            {
                return;
            }
            switch ((SportMode)e.Parameter)
            {
                case SportMode.Generic:
                    e.CanExecute = !HeartRate.Capabilities.Enabled.Equals(HeartRate.Features.Generic);
                    break;
                case SportMode.Running:
                    e.CanExecute = HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Running);
                    break;
                case SportMode.Cycling:
                    e.CanExecute = HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Cycling);
                    break;
                case SportMode.Swimming:
                    e.CanExecute = HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Swimming);
                    break;
                default:
                    break;
            }
        }

        private void SetHRFeatureExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            heartRate.SetHRFeature(ApplyFeature, EnableGymMode);
        }

        private void SetHRFeatureCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
