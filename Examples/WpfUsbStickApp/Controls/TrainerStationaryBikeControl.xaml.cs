using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for TrainerStationaryBikeControl.xaml
    /// </summary>
    public partial class TrainerStationaryBikeControl : UserControl
    {
        public TrainerStationaryBikeControl(TrainerStationaryBike trainer)
        {
            InitializeComponent();
            DataContext = new TrainerStationaryBikeViewModel(trainer);
        }
    }
}
