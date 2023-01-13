using System.Windows.Controls;
using System.Windows.Input;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for BicyclePowerParametersControl.xaml
    /// </summary>
    public partial class BicyclePowerParametersControl : UserControl
    {
        public RoutedCommand GetParmsRoutedCommand;
        public BicyclePowerParametersControl()
        {
            InitializeComponent();
        }
    }
}
