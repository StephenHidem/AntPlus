using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmallEarthTech.AntPlus.DeviceProfiles.Geocache;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace WpfUsbStickApp.ViewModels
{
    internal partial class GeocacheViewModel : ObservableObject
    {
        private bool pinReq;
        private bool authReq, programming;
        private bool logVisit;
        private bool ignoreVists;
        private double lastMessageRate;

        private readonly Geocache geocache;
        public Geocache Geocache => geocache;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProgramGeocacheCommand))]
        private uint? pin;
        [ObservableProperty]
        private string? trackableId;
        [ObservableProperty]
        private double latitude;
        [ObservableProperty]
        private double longitude;
        [ObservableProperty]
        private string? hint;

        public GeocacheViewModel(Geocache geocache)
        {
            this.geocache = geocache;
            geocache.PropertyChanged += Geocache_PropertyChanged;
        }

        private void Geocache_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Geocache.ProgrammingPIN):
                    if (pinReq)
                    {
                        pinReq = false;
                        CheckCanExecutes();
                    }
                    break;
                case nameof(Geocache.MessageRate):
                    double deltaRate = Math.Abs(geocache.MessageRate - lastMessageRate);
                    if (deltaRate > 2)
                    {
                        lastMessageRate = geocache.MessageRate;
                        CheckCanExecutes();
                    }
                    break;
                case nameof(Geocache.NumberOfVisits):
                    if (logVisit && !ignoreVists)
                    {
                        logVisit = false;
                        CheckCanExecutes();
                    }
                    break;
                case nameof(Geocache.AuthenticationToken):
                    if (authReq)
                    {
                        authReq = false;
                        CheckCanExecutes();
                    }
                    break;
                default:
                    break;
            }
        }

        [RelayCommand(CanExecute = nameof(CanRequestPin))]
        private async void RequestPIN()
        {
            pinReq = true;
            RequestPINCommand.NotifyCanExecuteChanged();
            _ = await Task.Run(() => geocache.RequestPinPage());
        }
        private bool CanRequestPin()
        {
            return !geocache.Offline && !programming && !pinReq && geocache.MessageRate < 2;
        }

        [RelayCommand(CanExecute = nameof(CanLogVisit))]
        private void LogVisit()
        {
            logVisit = ignoreVists = true;
            _ = geocache.UpdateLoggedVisits();
            ignoreVists = false;
        }
        private bool CanLogVisit()
        {
            return !geocache.Offline && !programming && !logVisit && !pinReq && geocache.MessageRate > 2 && geocache.NumberOfVisits != null;
        }

        [RelayCommand(CanExecute = nameof(CanRequestAuthentication))]
        private async void RequestAuthentication()
        {
            authReq = true;
            Random rnd = new();
            _ = await Task.Run(() => geocache.RequestAuthentication((uint)rnd.Next()));
        }
        private bool CanRequestAuthentication()
        {
            return !geocache.Offline && !programming && !authReq && geocache.MessageRate > 2;
        }

        [RelayCommand(CanExecute = nameof(CanProgramGeocache))]
        private async void ProgramGeocache()
        {
            if (TrackableId != null && Pin != null)
            {
                // we want to capture the updated logged visits
                programming = logVisit = true;
                CheckCanExecutes();
                await Task.Run(() =>
                {
                    geocache.ProgramGeocache(TrackableId, (uint)Pin, Latitude, Longitude, Hint);
                    programming = false;
                });
            }
            else
            {
                _ = MessageBox.Show("Please enter a value for the ID and PIN.", "Program Geocache");
            }
        }
        private bool CanProgramGeocache()
        {
            return !geocache.Offline && !programming && geocache.MessageRate > 2 &&
                (geocache.ProgrammingPIN == null || geocache.ProgrammingPIN == Pin);
        }

        private void CheckCanExecutes()
        {
            _ = Application.Current.Dispatcher.BeginInvoke(() =>
            {
                RequestPINCommand.NotifyCanExecuteChanged();
                LogVisitCommand.NotifyCanExecuteChanged();
                RequestAuthenticationCommand.NotifyCanExecuteChanged();
                ProgramGeocacheCommand.NotifyCanExecuteChanged();
            });
        }
    }
}
