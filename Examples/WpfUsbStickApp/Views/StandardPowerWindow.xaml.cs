using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows;
using System.Windows.Data;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for StandardPowerWindow.xaml
    /// </summary>
    public partial class StandardPowerWindow : Window
    {
        public StandardPowerWindow(StandardPowerSensor sensor)
        {
            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(sensor.Measurements, sensor.CollectionLock);
            DataContext = new BicyclePowerViewModel(sensor);
        }
    }
}
