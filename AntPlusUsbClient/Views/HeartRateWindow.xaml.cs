using AntPlusUsbClient.ViewModels;
using DeviceProfiles;
using System.Windows;
using System.Windows.Controls;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for HeartRateWindow.xaml
    /// </summary>
    public partial class HeartRateWindow : Window
    {
        public HeartRateWindow(ListView listView)
        {
            InitializeComponent();
            HeartRate item = (HeartRate)listView.SelectedItem;
            DataContext = new HeartRateViewModel(item);
        }
    }
}
