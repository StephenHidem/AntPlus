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
        internal BicyclePowerCalibrationControl(BicyclePowerViewModel vm)
        {
            BindingOperations.EnableCollectionSynchronization(vm.BicyclePower.Calibration.Measurements, vm.BicyclePower.Calibration.Measurements.collectionLock);
            InitializeComponent();
            DataContext = vm;
        }
    }
}
