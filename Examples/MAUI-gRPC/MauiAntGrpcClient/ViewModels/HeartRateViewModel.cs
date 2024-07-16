using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;

namespace MauiAntGrpcClient.ViewModels
{
    [QueryProperty(nameof(HeartRate), "Sensor")]
    public partial class HeartRateViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private HeartRate? heartRate;

        [ObservableProperty]
        private SportMode modeRequested;
        [ObservableProperty]
        private bool applyFeature;
        [ObservableProperty]
        private bool enableGymMode;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor("SetGymModeCommand")]
        private bool isGymModeSupported;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor("SetSportModeCommand")]
        private bool isRunningSupported;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor("SetSportModeCommand")]
        private bool isCyclingSupported;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor("SetSportModeCommand")]
        private bool isSwimmingSupported;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            ModeRequested = SportMode.Generic;
            ApplyFeature = true;
            HeartRate = (HeartRate)query["Sensor"];
            HeartRate.PropertyChanged += HeartRate_PropertyChanged;
            _ = HeartRate.RequestDataPage(HeartRate.DataPage.Capabilities);
        }

        private void HeartRate_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Capabilities")
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsCyclingSupported = HeartRate!.Capabilities.Supported.HasFlag(HeartRate.Features.Cycling);
                    IsRunningSupported = HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Running);
                    IsSwimmingSupported = HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.Swimming);
                    IsGymModeSupported = HeartRate.Capabilities.Supported.HasFlag(HeartRate.Features.GymMode);
                    SetGymModeCommand.NotifyCanExecuteChanged();
                    SetSportModeCommand.NotifyCanExecuteChanged();
                });
            }
        }

        [RelayCommand]
        private void GetCapabilities() => _ = HeartRate!.RequestDataPage(HeartRate.DataPage.Capabilities);

        [RelayCommand(CanExecute = nameof(CanSetGymMode))]
        private async Task<MessagingReturnCode> SetGymMode()
        {
            MessagingReturnCode result = await HeartRate!.SetHRFeature(ApplyFeature, EnableGymMode);
            _ = HeartRate.RequestDataPage(HeartRate.DataPage.Capabilities);
            return result;
        }
        private bool CanSetGymMode() => IsGymModeSupported;

        [RelayCommand(CanExecute = nameof(CanSetSportMode))]
        private async Task<MessagingReturnCode> SetSportMode()
        {
            MessagingReturnCode result = await HeartRate!.SetSportMode(ModeRequested);
            _ = HeartRate.RequestDataPage(HeartRate.DataPage.Capabilities);
            return result;
        }
        private bool CanSetSportMode() => IsRunningSupported || IsCyclingSupported || IsSwimmingSupported;
    }
}
