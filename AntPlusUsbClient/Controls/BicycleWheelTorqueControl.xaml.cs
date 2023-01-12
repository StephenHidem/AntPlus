using AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for BicycleWheelTorqueControl.xaml
    /// </summary>
    public partial class BicycleWheelTorqueControl : UserControl
    {
        public BicycleWheelTorqueControl(StandardWheelTorqueSensor wts)
        {
            InitializeComponent();
            DataContext = wts;
        }
    }
}
