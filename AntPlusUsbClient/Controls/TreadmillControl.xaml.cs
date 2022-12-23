using AntPlus.DeviceProfiles.FitnessEquipment;
using AntPlusUsbClient.ViewModels;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for TreadmillControl.xaml
    /// </summary>
    public partial class TreadmillControl : UserControl
    {
        public TreadmillControl(FitnessEquipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = new TreadmillViewModel(fitnessEquipment);
        }
    }
}
