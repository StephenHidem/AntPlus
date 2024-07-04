using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for NordicSkierControl.xaml
    /// </summary>
    public partial class NordicSkierControl : UserControl
    {
        public NordicSkierControl(NordicSkier fitnessEquipment)
        {
            InitializeComponent();
            DataContext = fitnessEquipment;
        }
    }
}
