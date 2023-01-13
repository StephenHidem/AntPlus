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
        public BicyclePowerCalibrationControl()
        {
            InitializeComponent();
        }

        public BicyclePowerCalibrationControl(BicyclePowerViewModel vm)
        {
            BindingOperations.EnableCollectionSynchronization(vm.BicyclePower.PowerOnlySensor.Calibration.Measurements, vm.BicyclePower.PowerOnlySensor.Calibration.Measurements.collectionLock);
            InitializeComponent();
            DataContext = vm;
        }
    }
}
