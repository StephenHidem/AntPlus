using AntPlus.DeviceProfiles.BicyclePower;
using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for MeasurementOutput.xaml
    /// </summary>
    public partial class MeasurementOutputControl : UserControl
    {
        public MeasurementOutputControl(BicyclePower bp)
        {
            InitializeComponent();
            DataContext = bp.PowerOnlySensor.MeasurementOutput;
        }
    }
}
