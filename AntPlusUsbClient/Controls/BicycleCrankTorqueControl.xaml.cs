using AntPlusUsbClient.ViewModels;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for BicycleCrankTorqueControl.xaml
    /// </summary>
    public partial class BicycleCrankTorqueControl : UserControl
    {
        public BicycleCrankTorqueControl(BicyclePowerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm.BicyclePower.CrankTorqueSensor;
        }
    }
}
