using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows;
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
            DataContext = new BicyclePowerViewModel(sensor);
        }
    }
}
