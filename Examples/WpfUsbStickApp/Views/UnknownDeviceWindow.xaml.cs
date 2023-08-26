using SmallEarthTech.AntPlus.DeviceProfiles;
using System.Windows;
using System.Windows.Data;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for UnknownDeviceWindow.xaml
    /// </summary>
    public partial class UnknownDeviceWindow : Window
    {
        public UnknownDeviceWindow(UnknownDevice unknownDevice)
        {
            BindingOperations.EnableCollectionSynchronization(unknownDevice.DataPages, unknownDevice.CollectionLock);
            InitializeComponent();
            DataContext = new UnknownDeviceViewModel(unknownDevice);
        }
    }
}
