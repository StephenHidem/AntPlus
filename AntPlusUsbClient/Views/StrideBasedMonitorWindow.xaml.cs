using AntPlusUsbClient.ViewModels;
using SmallEarthTech.AntPlus.DeviceProfiles.StrideBasedSpeedAndDistance;
using System.Windows;

namespace AntPlusUsbClient.Views
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
