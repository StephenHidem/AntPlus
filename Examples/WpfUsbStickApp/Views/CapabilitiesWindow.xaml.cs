using SmallEarthTech.AntRadioInterface;
using System.Windows;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for CapabilitiesWindow.xaml
    /// </summary>
    public partial class CapabilitiesWindow : Window
    {
        public CapabilitiesWindow(DeviceCapabilities capabilities)
        {
            InitializeComponent();
            DataContext = new CapabilitiesViewModel(capabilities);
        }
    }
}
