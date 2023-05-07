using System.Windows.Controls;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Controls
{
    /// <summary>
    /// Interaction logic for BicyclePowerOnlyControl.xaml
    /// </summary>
    public partial class BicyclePowerOnlyControl : UserControl
    {
        public BicyclePowerOnlyControl(BicyclePowerViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
