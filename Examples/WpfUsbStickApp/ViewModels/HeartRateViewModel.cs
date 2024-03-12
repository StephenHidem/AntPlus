using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private async Task RequestPage()
        {
            _ = await heartRate.RequestDataPage(PageRequested);
        }
        private bool CanRequestPage()
        {
            return PageRequested switch
            {
                HeartRate.DataPage.Default or HeartRate.DataPage.PreviousHeartBeat => false,
                HeartRate.DataPage.CumulativeOperatingTime or HeartRate.DataPage.ManufacturerInfo or HeartRate.DataPage.ProductInfo or HeartRate.DataPage.SwimInterval or HeartRate.DataPage.Capabilities or HeartRate.DataPage.BatteryStatus => true,
                _ => false,
            };
        }

        [RelayCommand(CanExecute = nameof(CanSetSportMode))]
        private async Task SetSportMode()
        {
            _ = await heartRate.SetSportMode(ModeRequested);
        }
        private bool CanSetSportMode()
        {
            return ModeRequested switch
            {
                SportMode.Generic => !HeartRate.Capabilities.Enabled.Equals(HeartRate.Features.Generic),
                SportMode.Running => HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Running),
                SportMode.Cycling => HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Cycling),
                SportMode.Swimming => HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Swimming),
                _ => false,
            };
        }

        [RelayCommand]
        private async Task SetHRFeature()
        {
            _ = await heartRate.SetHRFeature(ApplyFeature, EnableGymMode);
        }
    }
}
