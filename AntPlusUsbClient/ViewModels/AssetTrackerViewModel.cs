using AntPlus.DeviceProfiles.AssetTracker;
using System.ComponentModel;

namespace AntPlusUsbClient.ViewModels
{
    internal class AssetTrackerViewModel : INotifyPropertyChanged
    {
        private readonly AssetTracker tracker;

        public event PropertyChangedEventHandler PropertyChanged;

        public AssetTracker AssetTracker => tracker;

        public AssetTrackerViewModel(AssetTracker tracker)
        {
            this.tracker = tracker;
        }
    }
}
