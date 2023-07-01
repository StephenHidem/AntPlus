using SmallEarthTech.AntPlus.DeviceProfiles.AssetTracker;
using System.Windows;
using System.Windows.Data;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for AssetTrackerWindow.xaml
    /// </summary>
    public partial class AssetTrackerWindow : Window
    {
        public AssetTrackerWindow(Tracker assetTracker)
        {
            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(assetTracker.Assets, assetTracker.Assets.CollectionLock);
            DataContext = new AssetTrackerViewModel(assetTracker);
        }
    }
}
