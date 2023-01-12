using AntPlusUsbClient.ViewModels;
using System.Windows.Controls;
using System.Windows.Data;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for BicyclePowerCalibrationControl.xaml
    /// </summary>
    public partial class BicyclePowerCalibrationControl : UserControl
    {
        internal BicyclePowerCalibrationControl(BicyclePowerViewModel bp)
        {
            BindingOperations.EnableCollectionSynchronization(bp.BicyclePower.Calibration.Measurements, bp.BicyclePower.Calibration.Measurements.collectionLock);
            InitializeComponent();
            DataContext = bp;
        }
    }
}
