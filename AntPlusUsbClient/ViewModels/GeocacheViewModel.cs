using AntPlus.DeviceProfiles;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    internal class GeocacheViewModel
    {
        private readonly Geocache geocache;

        public Geocache Geocache => geocache;

        public RoutedCommand RequestPIN { get; private set; } = new RoutedCommand();
        public RoutedCommand RequestAuthentication { get; private set; } = new RoutedCommand();
        public RoutedCommand LogVisit { get; private set; } = new RoutedCommand();
        public RoutedCommand ProgramGeocache { get; private set; } = new RoutedCommand();

        public CommandBinding[] CommandBindings { get; private set; }


        public GeocacheViewModel(Geocache geocache)
        {
            this.geocache = geocache;

            CommandBindings = new CommandBinding[] {
                new CommandBinding(RequestPIN, RequestPINExecuted, RequestPINCanExecute),
                new CommandBinding(RequestAuthentication, RequestAuthenticationExecuted, RequestAuthenticationCanExecute),
                new CommandBinding(LogVisit, LogVisitExecuted, LogVisitCanExecute),
                new CommandBinding(ProgramGeocache, ProramExecuted, ProgramCanExecute),
            };

        }

        private void RequestPINExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            geocache.RequestPinPage();
        }

        private void RequestPINCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RequestAuthenticationExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            geocache.RequestAuthentication(0x11223344);
        }

        private void RequestAuthenticationCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LogVisitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            geocache.UpdateLoggedVisits();
        }

        private void LogVisitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ProramExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            geocache.ProgramGeocache("junk-abcd", 100, 0xC0000000, 0x40000000, "abcde");
        }

        private void ProgramCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
