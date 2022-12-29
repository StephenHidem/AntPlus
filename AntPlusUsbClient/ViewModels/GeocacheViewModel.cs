using AntPlus;
using AntPlus.DeviceProfiles;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    internal class GeocacheViewModel : INotifyPropertyChanged
    {
        private readonly Geocache Geocache;

        public event PropertyChangedEventHandler PropertyChanged;

        public string TrackableId => Geocache.TrackableId;
        public uint ProgrammingPIN => Geocache.ProgrammingPIN;
        public byte TotalPagesProgrammed => Geocache.TotalPagesProgrammed;
        public uint NextStageLatitude => Geocache.NextStageLatitude;
        public uint NextStageLongitude => Geocache.NextStageLongitude;
        public string Hint => Geocache.Hint;
        public ushort NumberOfVisits => Geocache.NumberOfVisits;
        public DateTime LastVisitTimestamp => Geocache.LastVisitTimestamp;
        public byte[] AuthenticationToken => Geocache.AuthenticationToken;
        public CommonDataPages CommonDataPages { get; private set; }

        public RoutedCommand RequestPIN { get; private set; } = new RoutedCommand();
        public RoutedCommand RequestAuthentication { get; private set; } = new RoutedCommand();
        public RoutedCommand LogVisit { get; private set; } = new RoutedCommand();
        public RoutedCommand ProgramGeocache { get; private set; } = new RoutedCommand();

        public CommandBinding[] CommandBindings { get; private set; }


        public GeocacheViewModel(Geocache geocache)
        {
            Geocache = geocache;
            geocache.PinPageChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgrammingPIN"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TotalPagesProgrammed"));
            };
            geocache.HintChanged += (s, e) => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Hint")); };
            geocache.AuthenticationPageChanged += (s, e) => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AuthenticationToken")); };
            geocache.LatitudePageChanged += (s, e) => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NextStageLatitude")); };
            geocache.LongitudePageChanged += (s, e) => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NextStageLongitude")); };
            geocache.LoggedVisitsChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumberOfVisits"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LastVisitTimestamp"));
            };

            CommandBindings = new CommandBinding[] {
                new CommandBinding(RequestPIN, RequestPINExecuted, RequestPINCanExecute),
                new CommandBinding(RequestAuthentication, RequestAuthenticationExecuted, RequestAuthenticationCanExecute),
                new CommandBinding(LogVisit, LogVisitExecuted, LogVisitCanExecute),
                new CommandBinding(ProgramGeocache, ProramExecuted, ProgramCanExecute),
            };

        }

        private void RequestPINExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Geocache.RequestPinPage();
        }

        private void RequestPINCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RequestAuthenticationExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Geocache.RequestAuthentication(0x11223344);
        }

        private void RequestAuthenticationCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LogVisitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Geocache.UpdateLoggedVisits();
        }

        private void LogVisitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ProramExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Geocache.ProgramGeocache("junk-abcd", 100, 0xC0000000, 0x40000000, "abcde");
        }

        private void ProgramCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
