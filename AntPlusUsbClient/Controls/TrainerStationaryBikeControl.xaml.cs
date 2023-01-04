using AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for TrainerStationaryBikeControl.xaml
    /// </summary>
    public partial class TrainerStationaryBikeControl : UserControl
    {
        public TrainerStationaryBikeControl(FitnessEquipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = fitnessEquipment.TrainerStationaryBike;
        }
    }
}
