using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for TEPSControl.xaml
    /// </summary>
    public partial class TEPSControl : UserControl
    {
        public TEPSControl()
        {
            InitializeComponent();
        }

        public TEPSControl(TorqueEffectivenessAndPedalSmoothness teps)
        {
            InitializeComponent();
            DataContext = teps;
        }
    }
}
