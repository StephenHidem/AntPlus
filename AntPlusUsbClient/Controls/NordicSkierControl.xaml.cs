using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for NordicSkierControl.xaml
    /// </summary>
    public partial class NordicSkierControl : UserControl
    {
        public NordicSkierControl(FitnessEquipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = fitnessEquipment.NordicSkier;
        }
    }
}
