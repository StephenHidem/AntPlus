using System.Windows.Controls;

namespace AntPlusUsbClient.Controls
{
    /// <summary>
    /// Interaction logic for CommonDataPagesControl.xaml
    /// </summary>
    public partial class CommonDataPagesControl : UserControl
    {
        public CommonDataPagesControl()
        {
            InitializeComponent();
        }

        public CommonDataPagesControl(object ctx)
        {
            InitializeComponent();
            DataContext = ctx;
        }
    }
}
