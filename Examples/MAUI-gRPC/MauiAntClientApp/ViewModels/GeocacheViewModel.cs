using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System.ComponentModel;

namespace MauiAntClientApp.ViewModels
{
    public partial class GeocacheViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ILogger<GeocacheViewModel> _logger;
        private bool pinReq, authReq, programming, logVisit;

        [ObservableProperty]
        private Geocache geocache = null!;
        [ObservableProperty]
        private CommonDataPages commonDataPages = null!;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ProgramGeocacheCommand))]
        private uint? pin;
        [ObservableProperty]
        private string trackableId = string.Empty;
        [ObservableProperty]
        private double? latitude;
        [ObservableProperty]
        private double? longitude;
        [ObservableProperty]
        private string hint = string.Empty;

        public GeocacheViewModel(ILogger<GeocacheViewModel> logger)
        {
            _logger = logger;
            _logger.LogInformation("Created Geocache");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _logger.LogInformation($"{nameof(ApplyQueryAttributes)}");
            Geocache = (Geocache)query["Sensor"];
            CommonDataPages = Geocache.CommonDataPages;
            Geocache.PropertyChanged += Geocache_PropertyChanged;
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
                case nameof(Geocache.NumberOfVisits):
                    if (logVisit)
                    {
                        logVisit = false;
                    }
                    CheckCanExecutes();
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
        private async Task<MessagingReturnCode> RequestPIN()
        {
            pinReq = true;
            CheckCanExecutes();
            return await Geocache.RequestPinPage();
        }
        private bool CanRequestPin()
        {
            if (Geocache != null)
            {
                return !Geocache.Offline && !programming && !pinReq && !authReq;
            }
            return false;
        }

        [RelayCommand(CanExecute = nameof(CanLogVisit))]
        private async Task<MessagingReturnCode> LogVisit()
        {
            logVisit = true;
            CheckCanExecutes();
            return await Geocache.UpdateLoggedVisits();
        }
        private bool CanLogVisit()
        {
            if (Geocache != null)
            {
                return !Geocache.Offline && !programming && !logVisit && !pinReq && !authReq && Geocache.NumberOfVisits != null;
            }
            return false;
        }

        [RelayCommand(CanExecute = nameof(CanRequestAuthentication))]
        private async Task<MessagingReturnCode> RequestAuthentication()
        {
            authReq = true;
            CheckCanExecutes();
            Random rnd = new();
            return await Geocache.RequestAuthentication((uint)rnd.Next());
        }
        private bool CanRequestAuthentication()
        {
            if (Geocache != null)
            {
                return !Geocache.Offline && !programming && !pinReq && !logVisit && !authReq;
            }
            return false;
        }

        [RelayCommand(CanExecute = nameof(CanProgramGeocache))]
        private async Task<MessagingReturnCode> ProgramGeocache()
        {
            // we want to capture the updated logged visits
            programming = logVisit = true;
            CheckCanExecutes();
            MessagingReturnCode result = await Geocache.ProgramGeocache(TrackableId, Pin, Latitude, Longitude, Hint);
            programming = false;
            CheckCanExecutes();
            return result;
        }
        private bool CanProgramGeocache()
        {
            if (Geocache != null)
            {
                return !Geocache.Offline && !programming && !pinReq && !logVisit && !authReq;
            }
            return false;
        }

        private void CheckCanExecutes()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                RequestPINCommand.NotifyCanExecuteChanged();
                LogVisitCommand.NotifyCanExecuteChanged();
                RequestAuthenticationCommand.NotifyCanExecuteChanged();
                ProgramGeocacheCommand.NotifyCanExecuteChanged();
            });
        }
    }
}
