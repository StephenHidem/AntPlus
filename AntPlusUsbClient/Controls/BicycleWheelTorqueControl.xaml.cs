using AntPlusUsbClient.ViewModels;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for BicycleWheelTorqueControl.xaml
    /// </summary>
    public partial class BicycleWheelTorqueControl : UserControl
    {
        public BicycleWheelTorqueControl(BicyclePowerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm.BicyclePower.WheelTorqueSensor;
        }
    }
}
