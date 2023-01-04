using AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for EllipticalControl.xaml
    /// </summary>
    public partial class EllipticalControl : UserControl
    {
        public EllipticalControl(FitnessEquipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = fitnessEquipment.Elliptical;
        }
    }
}
