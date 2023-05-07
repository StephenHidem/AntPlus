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
        public AssetTrackerWindow(AssetTracker assetTracker)
        {
            InitializeComponent();
            AssetTrackerViewModel vm = new AssetTrackerViewModel(assetTracker);
            BindingOperations.EnableCollectionSynchronization(assetTracker.Assets, assetTracker.Assets.CollectionLock);
            DataContext = vm;
        }
    }
}
