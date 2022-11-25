using AntPlusUsbClient.ViewModels;
using DeviceProfiles;
using System.Windows;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for HeartRateWindow.xaml
    /// </summary>
    public partial class HeartRateWindow : Window
    {
        public HeartRateWindow(HeartRate heartRate)
        {
            InitializeComponent();
            HeartRateViewModel hrvm = new HeartRateViewModel(heartRate);
            CommandBindings.Add(hrvm.PageRequestBinding);
            CommandBindings.Add(hrvm.SetSportModeBinding);
            DataContext = hrvm;
        }
    }
}
