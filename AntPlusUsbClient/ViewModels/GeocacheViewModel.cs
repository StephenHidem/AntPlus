using SmallEarthTech.AntPlus.DeviceProfiles.Geocache;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    internal class GeocacheViewModel : INotifyPropertyChanged
    {
        private readonly Geocache geocache;
        private bool offline, pinReq, programming, logVisit, ignoreVists, authReq;
        private double lastMessageRate;

        public event PropertyChangedEventHandler PropertyChanged;

        public Geocache Geocache => geocache;

        public RoutedCommand RequestPIN { get; private set; } = new RoutedCommand();
        public RoutedCommand RequestAuthentication { get; private set; } = new RoutedCommand();
        public RoutedCommand LogVisit { get; private set; } = new RoutedCommand();
        public RoutedCommand ProgramGeocache { get; private set; } = new RoutedCommand();

        public CommandBinding[] CommandBindings { get; private set; }

        public uint? Pin { get; set; }
        public string TrackableId { get; set; }
        public int? Latitude { get; set; }
        public int? Longitude { get; set; }
        public string Hint { get; set; }
        public Visibility Offline { get; private set; } = Visibility.Hidden;

        public GeocacheViewModel(Geocache geocache)
        {
            this.geocache = geocache;

            CommandBindings = new CommandBinding[] {
                new CommandBinding(RequestPIN, ExecutedRequestPIN, CanExecuteRequestPIN),
                new CommandBinding(RequestAuthentication, ExecutedRequestAuthentication, CanExecuteRequestAuthentication),
                new CommandBinding(LogVisit, ExecutedUpdateLoggedVisits, CanExecuteUpdateLoggedVisit),
                new CommandBinding(ProgramGeocache, ExecutedProgramGeocache, CanExecuteProgramGeocache),
            };

            geocache.PropertyChanged += Geocache_PropertyChanged;
            geocache.DeviceWentOffline += Geocache_DeviceWentOffline;
        }

        private void Geocache_DeviceWentOffline(object sender, EventArgs e)
        {
            geocache.PropertyChanged -= Geocache_PropertyChanged;
            geocache.DeviceWentOffline -= Geocache_DeviceWentOffline;
            Offline = Visibility.Visible;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Offline)));
            offline = true;
            Application.Current.Dispatcher.BeginInvoke(new System.Action(() => CommandManager.InvalidateRequerySuggested()));
        }

        private void Geocache_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Geocache.ProgrammingPIN):
                    if (pinReq)
                    {
                        pinReq = false;
                        Application.Current.Dispatcher.BeginInvoke(new System.Action(() => CommandManager.InvalidateRequerySuggested()));
                    }
                    break;
                case nameof(Geocache.MessageRate):
                    double deltaRate = Math.Abs(geocache.MessageRate - lastMessageRate);
                    if (deltaRate > 2)
                    {
                        lastMessageRate = geocache.MessageRate;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => CommandManager.InvalidateRequerySuggested()));
                    }
                    break;
                case nameof(Geocache.NumberOfVisits):
                    if (logVisit && !ignoreVists)
                    {
                        logVisit = false;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => CommandManager.InvalidateRequerySuggested()));
                    }
                    break;
                case nameof(Geocache.AuthenticationToken):
                    if (authReq)
                    {
                        authReq = false;
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => CommandManager.InvalidateRequerySuggested()));
                    }
                    break;
                default:
                    break;
            }
        }

        private async void ExecutedRequestPIN(object sender, ExecutedRoutedEventArgs e)
        {
            pinReq = true;
            _ = await Task.Run(() => geocache.RequestPinPage());
        }

        private void CanExecuteRequestPIN(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !offline && !programming && !pinReq && geocache.MessageRate < 2;
        }

        private void ExecutedUpdateLoggedVisits(object sender, ExecutedRoutedEventArgs e)
        {
            logVisit = ignoreVists = true;
            _ = geocache.UpdateLoggedVisits();
            ignoreVists = false;
        }

        private void CanExecuteUpdateLoggedVisit(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !offline && !programming && !logVisit && !pinReq && geocache.MessageRate > 2 && geocache.NumberOfVisits != null;
        }

        private async void ExecutedRequestAuthentication(object sender, ExecutedRoutedEventArgs e)
        {
            authReq = true;
            Random rnd = new Random();
            _ = await Task.Run(() => geocache.RequestAuthentication((uint)rnd.Next()));
        }

        private void CanExecuteRequestAuthentication(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !offline && !programming && !authReq && geocache.MessageRate > 2;
        }

        private async void ExecutedProgramGeocache(object sender, ExecutedRoutedEventArgs e)
        {
            if (TrackableId != null && Pin != null)
            {
                // we want to capture the updated logged visits
                programming = logVisit = true;
                await Task.Run(() =>
                {
                    geocache.ProgramGeocache(TrackableId, (uint)Pin, Latitude, Longitude, Hint);
                    programming = false;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => CommandManager.InvalidateRequerySuggested()));
                });
            }
            else
            {
                _ = MessageBox.Show("Please enter a value for the ID and PIN.", "Program Geocache");
            }
        }

        private void CanExecuteProgramGeocache(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !offline && !programming && geocache.MessageRate > 2 &&
                (geocache.ProgrammingPIN == null || geocache.ProgrammingPIN == Pin);
        }
    }
}
