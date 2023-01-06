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
                new CommandBinding(RequestPIN, (s, e) => geocache.RequestPinPage()),
                new CommandBinding(RequestAuthentication, (s, e) => geocache.RequestAuthentication(0x11223344)),
                new CommandBinding(LogVisit, (s, e) => geocache.UpdateLoggedVisits()),
                new CommandBinding(ProgramGeocache, (s, e) => geocache.ProgramGeocache("junk-abcd", 100, 0xC0000000, 0x40000000, "abcde")),
            };
        }
    }
}
