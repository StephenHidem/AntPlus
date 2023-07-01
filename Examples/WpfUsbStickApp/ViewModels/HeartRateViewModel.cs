using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class HeartRateViewModel : ObservableObject
    {
        private readonly HeartRate heartRate;
        public HeartRate HeartRate => heartRate;

        public static IEnumerable<HeartRate.DataPage> DataPageValues => Enum.GetValues(typeof(HeartRate.DataPage)).Cast<HeartRate.DataPage>();
        public static IEnumerable<SportMode> SportModeValues => Enum.GetValues(typeof(SportMode)).Cast<SportMode>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RequestPageCommand))]
        private HeartRate.DataPage pageRequested;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SetSportModeCommand))]
        private SportMode modeRequested;

        [ObservableProperty]
        private bool applyFeature;
        [ObservableProperty]
        private bool enableGymMode;

        public HeartRateViewModel(HeartRate heartRate)
        {
            this.heartRate = heartRate;
            heartRate.PropertyChanged += HeartRate_PropertyChanged;
            PageRequested = HeartRate.DataPage.Capabilities;
            ModeRequested = SportMode.Cycling;
        }

        private void HeartRate_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(HeartRate.Capabilities):
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        SetSportModeCommand.NotifyCanExecuteChanged();
                    });
                    break;
                default:
                    break;
            }
        }

        [RelayCommand(CanExecute = nameof(CanRequestPage))]
        private void RequestPage()
        {
            _ = heartRate.RequestDataPage(PageRequested);
        }
        private bool CanRequestPage()
        {
            switch (PageRequested)
            {
                case HeartRate.DataPage.Default:
                case HeartRate.DataPage.PreviousHeartBeat:
                    return false;
                case HeartRate.DataPage.CumulativeOperatingTime:
                case HeartRate.DataPage.ManufacturerInfo:
                case HeartRate.DataPage.ProductInfo:
                case HeartRate.DataPage.SwimInterval:
                case HeartRate.DataPage.Capabilities:
                case HeartRate.DataPage.BatteryStatus:
                    return true;
                default:
                    return false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanSetSportMode))]
        private void SetSportMode()
        {
            heartRate.SetSportMode(ModeRequested);
        }
        private bool CanSetSportMode()
        {
            switch (ModeRequested)
            {
                case SportMode.Generic:
                    return !HeartRate.Capabilities.Enabled.Equals(HeartRate.Features.Generic);
                case SportMode.Running:
                    return HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Running);
                case SportMode.Cycling:
                    return HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Cycling);
                case SportMode.Swimming:
                    return HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Swimming);
                default:
                    return false;
            }
        }

        [RelayCommand]
        private void SetHRFeature()
        {
            heartRate.SetHRFeature(ApplyFeature, EnableGymMode);
        }
    }
}
