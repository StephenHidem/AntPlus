using AntPlusUsbClient.ViewModels;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for BicyclePowerOnlyControl.xaml
    /// </summary>
    public partial class BicyclePowerOnlyControl : UserControl
    {
        public BicyclePowerOnlyControl(BicyclePowerViewModel bp)
        {
            InitializeComponent();
            DataContext = bp.BicyclePower.PowerOnlySensor;
        }
    }
}
