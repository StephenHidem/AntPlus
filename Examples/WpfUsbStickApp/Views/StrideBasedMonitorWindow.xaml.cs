using SmallEarthTech.AntPlus.DeviceProfiles;
using System.Windows;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for StrideBasedMonitorWindow.xaml
    /// </summary>
    public partial class StrideBasedMonitorWindow : Window
    {
        public StrideBasedMonitorWindow(StrideBasedSpeedAndDistance sdm)
        {
            InitializeComponent();
            DataContext = new SDMViewModel(sdm);
        }
    }
}
