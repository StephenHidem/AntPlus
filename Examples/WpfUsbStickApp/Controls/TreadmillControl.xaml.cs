using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for TreadmillControl.xaml
    /// </summary>
    public partial class TreadmillControl : UserControl
    {
        public TreadmillControl(Treadmill treadmill)
        {
            InitializeComponent();
            DataContext = treadmill;
        }
    }
}
