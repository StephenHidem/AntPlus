using AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for BicyclePowerOnlyControl.xaml
    /// </summary>
    public partial class BicyclePowerOnlyControl : UserControl
    {
        public BicyclePowerOnlyControl(BicyclePower bp)
        {
            InitializeComponent();
            DataContext = bp.PowerOnlySensor;
        }
    }
}
