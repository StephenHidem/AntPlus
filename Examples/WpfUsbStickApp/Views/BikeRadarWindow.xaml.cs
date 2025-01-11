using SmallEarthTech.AntPlus.DeviceProfiles.BikeRadar;
using System.Windows;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for BikeRadarWindow.xaml
    /// </summary>
    public partial class BikeRadarWindow : Window
    {
        public BikeRadarWindow(BikeRadar radar)
        {
            InitializeComponent();
            DataContext = new BikeRadarViewModel(radar);
        }
    }
}
