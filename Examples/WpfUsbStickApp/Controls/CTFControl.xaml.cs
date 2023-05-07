using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for CTFControl.xaml
    /// </summary>
    public partial class CTFControl : UserControl
    {
        private readonly CrankTorqueFrequencySensor sensor;
        public CTFControl(BicyclePowerViewModel vm)
        {
            InitializeComponent();
            sensor = vm.BicyclePower.CTFSensor;
            DataContext = vm;
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
