using System.Windows.Controls;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for BicycleCrankTorqueControl.xaml
    /// </summary>
    public partial class BicycleCrankTorqueControl : UserControl
    {
        public BicycleCrankTorqueControl(BicyclePowerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
