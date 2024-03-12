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

        private async void Click_SaveSlope(object sender, System.Windows.RoutedEventArgs e)
        {
            _ = await sensor.SaveSlopeToFlash(30.0);
        }

        private async void Click_SaveSerialNumber(object sender, System.Windows.RoutedEventArgs e)
        {
            _ = await sensor.SaveSerialNumberToFlash(0x1234);
        }
    }
}
