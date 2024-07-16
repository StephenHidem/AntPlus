using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class GeocacheViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RequestPINCommand))]
        [NotifyCanExecuteChangedFor(nameof(LogVisitCommand))]
        [NotifyCanExecuteChangedFor(nameof(RequestAuthenticationCommand))]
        [NotifyCanExecuteChangedFor(nameof(ProgramGeocacheCommand))]
        private bool isBusy;

        [ObservableProperty]
        private Geocache geocache;

        [ObservableProperty]
        private string? trackableId;
        [ObservableProperty]
        private uint? pin;
        [ObservableProperty]
        private double? latitude;
        [ObservableProperty]
        private double? longitude;
        [ObservableProperty]
        private string? hint;

        public GeocacheViewModel(Geocache geocache)
        {
            Geocache = geocache;
            Geocache.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NumberOfVisits")
            {
                _ = Application.Current.Dispatcher.BeginInvoke(() => { LogVisitCommand.NotifyCanExecuteChanged(); });
            }
        }

        [RelayCommand(CanExecute = nameof(CanRequestPin))]
        private async Task RequestPIN()
        {
            IsBusy = true;
            _ = await Geocache.RequestPinPage();
            IsBusy = false;
        }
        private bool CanRequestPin() => !Geocache.Offline && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanLogVisit))]
        private async Task LogVisit()
        {
            IsBusy = true;
            _ = await Geocache.UpdateLoggedVisits();
            IsBusy = false;
        }
        private bool CanLogVisit() => !Geocache.Offline && !IsBusy && Geocache.NumberOfVisits != null;

        [RelayCommand(CanExecute = nameof(CanRequestAuthentication))]
        private async Task RequestAuthentication()
        {
            IsBusy = true;
            Random rnd = new();
            _ = await Geocache.RequestAuthentication((uint)rnd.Next());
            IsBusy = false;
        }
        private bool CanRequestAuthentication() => !Geocache.Offline && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanProgramGeocache))]
        private async Task ProgramGeocache()
        {
            // we want to capture the updated logged visits
            IsBusy = true;
            _ = await Geocache.ProgramGeocache(TrackableId, Pin, Latitude, Longitude, Hint);
            IsBusy = false;
        }
        private bool CanProgramGeocache() => !Geocache.Offline && !IsBusy;
    }
}
