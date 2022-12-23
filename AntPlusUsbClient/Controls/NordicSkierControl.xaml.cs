using AntPlus.DeviceProfiles.FitnessEquipment;
using AntPlusUsbClient.ViewModels;
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
            DataContext = new NordicSkierViewModel(fitnessEquipment);
        }
    }
}
