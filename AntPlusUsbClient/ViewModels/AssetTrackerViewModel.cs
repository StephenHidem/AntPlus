using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;

namespace AntPlusUsbClient.ViewModels
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
