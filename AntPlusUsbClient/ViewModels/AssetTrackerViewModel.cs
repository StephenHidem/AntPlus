using AntPlus.DeviceProfiles.AssetTracker;
using System.ComponentModel;
using static AntPlus.CommonDataPages;

namespace AntPlusUsbClient.ViewModels
{
    internal class AssetTrackerViewModel : INotifyPropertyChanged
    {
        private readonly AssetTracker tracker;

        public event PropertyChangedEventHandler PropertyChanged;

        public AssetTracker AssetTracker => tracker;
        public ManufacturerInfoPage ManufacturerInfo { get; private set; }
        public ProductInfoPage ProductInfo { get; private set; }
        public BatteryStatusPage BatteryStatus { get; private set; }

        public AssetTrackerViewModel(AssetTracker tracker)
        {
            this.tracker = tracker;
            tracker.CommonDataPages.ManufacturerInfoPageChanged += (s, e) => { ManufacturerInfo = e; RaisePropertyChange("ManufacturerInfo"); };
            tracker.CommonDataPages.ProductInfoPageChanged += (s, e) => { ProductInfo = e; RaisePropertyChange("ProductInfo"); };
            tracker.CommonDataPages.BatteryStatusPageChanged += (s, e) => { BatteryStatus = e; RaisePropertyChange("BatteryStatus"); };
        }

        private void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
