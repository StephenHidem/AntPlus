using AntPlus.DeviceProfiles.FitnessEquipment;
using AntPlusUsbClient.ViewModels;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for RowerControl.xaml
    /// </summary>
    public partial class RowerControl : UserControl
    {
        public RowerControl(FitnessEquipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = new RowerViewModel(fitnessEquipment);
        }
    }
}
