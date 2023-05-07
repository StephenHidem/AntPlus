using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;

namespace WpfUsbStickApp.ViewModels
{
    internal class AssetTrackerViewModel
    {
        private readonly AssetTracker tracker;

        public AssetTracker AssetTracker => tracker;

        public AssetTrackerViewModel(AssetTracker tracker)
        {
            this.tracker = tracker;
        }
    }
}
