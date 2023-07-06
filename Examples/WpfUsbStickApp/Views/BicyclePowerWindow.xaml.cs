using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows;
using System.Windows.Data;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for BicyclePowerWindow.xaml
    /// </summary>
    public partial class BicyclePowerWindow : Window
    {
        public BicyclePowerWindow(Bicycle bicyclePower)
        {
            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(bicyclePower.Calibration.Measurements, bicyclePower.Calibration.Measurements.CollectionLock);
            DataContext = new BicyclePowerViewModel(bicyclePower);
        }
    }
}
