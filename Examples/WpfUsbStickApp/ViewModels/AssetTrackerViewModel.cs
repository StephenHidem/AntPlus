using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;

namespace WpfUsbStickApp.ViewModels
{
    internal class AssetTrackerViewModel
    {
        private readonly Tracker tracker;

        public Tracker AssetTracker => tracker;

        public AssetTrackerViewModel(Tracker tracker)
        {
            this.tracker = tracker;
        }
    }
}
