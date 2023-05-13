using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for BicyclePowerOnlyControl.xaml
    /// </summary>
    public partial class BicyclePowerOnlyControl : UserControl
    {
        public BicyclePowerOnlyControl(StandardPowerSensor bicyclePower)
        {
            InitializeComponent();
            DataContext = bicyclePower;
        }
    }
}
