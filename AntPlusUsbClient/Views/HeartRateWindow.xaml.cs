using AntPlusUsbClient.ViewModels;
using DeviceProfiles.HeartRate;
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
            CommandBindings.Add(hrvm.SetHRFeatureBinding);
            DataContext = hrvm;
        }
    }
}
