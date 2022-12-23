using AntPlus.DeviceProfiles.FitnessEquipment;
using AntPlusUsbClient.ViewModels;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for ClimberControl.xaml
    /// </summary>
    public partial class ClimberControl : UserControl
    {
        public ClimberControl(FitnessEquipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = new ClimberViewModel(fitnessEquipment);
        }
    }
}
