using AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for CTFControl.xaml
    /// </summary>
    public partial class CTFControl : UserControl
    {
        private CrankTorqueFrequencySensor sensor;
        public CTFControl(CrankTorqueFrequencySensor ctf)
        {
            InitializeComponent();
            DataContext = sensor = ctf;
        }

        private void Click_SaveSlope(object sender, System.Windows.RoutedEventArgs e)
        {
            sensor.SaveSlopeToFlash(300);
        }

        private void Click_SaveSerialNumber(object sender, System.Windows.RoutedEventArgs e)
        {
            sensor.SaveSerialNumberToFlash(0x1234);
        }

        private void Click_ManCalReq(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
