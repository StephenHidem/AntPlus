using AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for TEPSControl.xaml
    /// </summary>
    public partial class TEPSControl : UserControl
    {
        public TEPSControl(StandardPowerSensor bp)
        {
            InitializeComponent();
            DataContext = bp.TorqueEffectiveness;
        }
    }
}
