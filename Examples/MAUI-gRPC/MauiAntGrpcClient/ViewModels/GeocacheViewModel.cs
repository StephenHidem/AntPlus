using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles;
using System.ComponentModel;

namespace MauiAntGrpcClient.ViewModels
{
    public partial class GeocacheViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RequestPINCommand))]
        [NotifyCanExecuteChangedFor(nameof(LogVisitCommand))]
        [NotifyCanExecuteChangedFor(nameof(RequestAuthenticationCommand))]
        [NotifyCanExecuteChangedFor(nameof(ProgramGeocacheCommand))]
        [NotifyCanExecuteChangedFor(nameof(EraseGeocacheCommand))]
        private bool isBusy;

        [ObservableProperty]
        private Geocache? geocache;

        [ObservableProperty]
        private uint pin;
        [ObservableProperty]
        private string trackableId = string.Empty;
        [ObservableProperty]
        private double? latitude;
        [ObservableProperty]
        private double? longitude;
        [ObservableProperty]
        private string hint = string.Empty;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Geocache = (Geocache)query["Sensor"];
            Geocache.PropertyChanged += Geocache_PropertyChanged;
        }

        private void Geocache_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NumberOfVisits")
            {
                MainThread.BeginInvokeOnMainThread(() => { LogVisitCommand.NotifyCanExecuteChanged(); });
            }
        }

        [RelayCommand(CanExecute = nameof(CanRequestPin))]
        private async Task RequestPIN()
        {
            IsBusy = true;
            _ = await Geocache!.RequestPinPage();
            IsBusy = false;
        }
        private bool CanRequestPin() => Geocache != null && !Geocache.Offline && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanLogVisit))]
        private async Task LogVisit()
        {
            IsBusy = true;
            _ = await Geocache!.UpdateLoggedVisits();
            IsBusy = false;
        }
        private bool CanLogVisit() => Geocache != null && !Geocache.Offline && !IsBusy && Geocache.NumberOfVisits != null;

        [RelayCommand(CanExecute = nameof(CanRequestAuthentication))]
        private async Task RequestAuthentication()
        {
            IsBusy = true;
            Random rnd = new();
            _ = await Geocache!.RequestAuthentication((uint)rnd.Next());
            IsBusy = false;
        }
        private bool CanRequestAuthentication() => Geocache != null && !Geocache.Offline && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanProgramGeocache))]
        private async Task ProgramGeocache()
        {
            // we want to capture the updated logged visits
            IsBusy = true;
            _ = await Geocache!.ProgramGeocache(TrackableId, Pin, Latitude, Longitude, Hint);
            IsBusy = false;
        }
        private bool CanProgramGeocache() => Geocache != null && !Geocache.Offline && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanEraseGeocache))]
        private async Task EraseGeocache()
        {
            IsBusy = true;
            _ = await Geocache!.EraseGeocache();
            IsBusy = false;
        }
        private bool CanEraseGeocache() => Geocache != null && !Geocache.Offline && !IsBusy;
    }
}
