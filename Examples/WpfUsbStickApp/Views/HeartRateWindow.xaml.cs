using SmallEarthTech.AntPlus.DeviceProfiles;
using System.Windows;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for HeartRateWindow.xaml
    /// </summary>
    public partial class HeartRateWindow : Window
    {
        public HeartRateWindow(HeartRate heartRate)
        {
            InitializeComponent();
            DataContext = new HeartRateViewModel(heartRate);
        }
    }
}
