using SmallEarthTech.AntPlus.DeviceProfiles.Geocache;
using System.Windows;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for GeocacheWindow.xaml
    /// </summary>
    public partial class GeocacheWindow : Window
    {
        public GeocacheWindow(Geocache geocache)
        {
            InitializeComponent();
            DataContext = new GeocacheViewModel(geocache);
        }
    }
}
