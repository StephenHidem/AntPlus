using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for RowerControl.xaml
    /// </summary>
    public partial class RowerControl : UserControl
    {
        public RowerControl(Equipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = fitnessEquipment;
        }
    }
}
