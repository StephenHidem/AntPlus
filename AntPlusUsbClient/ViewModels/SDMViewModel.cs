using SmallEarthTech.AntPlus.DeviceProfiles.StrideBasedSpeedAndDistance;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    internal class SDMViewModel
    {
        private readonly StrideBasedSpeedAndDistance sdm;

        public StrideBasedSpeedAndDistance SDM => sdm;
        public RoutedCommand RequestSummary { get; private set; } = new RoutedCommand();
        public RoutedCommand RequestCapabilities { get; private set; } = new RoutedCommand();

        public CommandBinding[] CommandBindings { get; private set; }

        public SDMViewModel(StrideBasedSpeedAndDistance sdm)
        {
            this.sdm = sdm;

            CommandBindings = new CommandBinding[] {
                new CommandBinding(RequestSummary, (s, e) => SDM.RequestSummaryPage()),
                new CommandBinding(RequestCapabilities, (s, e) => SDM.RequestBroadcastCapabilities())
            };
        }
    }
}
