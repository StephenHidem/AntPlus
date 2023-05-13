using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for CTFControl.xaml
    /// </summary>
    public partial class CTFControl : UserControl
    {
        private readonly CrankTorqueFrequencySensor sensor;
        public CTFControl(CrankTorqueFrequencySensor ctf)
        {
            InitializeComponent();
            sensor = ctf;
            DataContext = ctf;
        }

        private void Click_SaveSlope(object sender, System.Windows.RoutedEventArgs e)
        {
            sensor.SaveSlopeToFlash(300);
        }

        private void Click_SaveSerialNumber(object sender, System.Windows.RoutedEventArgs e)
        {
            sensor.SaveSerialNumberToFlash(0x1234);
        }
    }
}
