using AntPlusUsbClient.ViewModels;
using System.Windows.Controls;

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
            InitializeComponent();
            DataContext = vm;
        }
    }
}
