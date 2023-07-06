using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for TrainerStationaryBikeControl.xaml
    /// </summary>
    public partial class TrainerStationaryBikeControl : UserControl
    {
        public TrainerStationaryBikeControl(Equipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = fitnessEquipment;
        }
    }
}
