using System.Windows.Controls;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for BicycleWheelTorqueControl.xaml
    /// </summary>
    public partial class BicycleWheelTorqueControl : UserControl
    {
        public BicycleWheelTorqueControl(BicyclePowerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
