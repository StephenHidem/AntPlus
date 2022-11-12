using AntPlusUsbClient.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AntPlusUsbClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(App.AntDevices, App.AntDevices.collectionLock);
            DataContext = App.AntDevices;
            antDevices.MouseDoubleClick += AntDevices_MouseDoubleClick;
        }

        private void AntDevices_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HeartRateWindow heartRateWindow = new HeartRateWindow((ListView)sender);
            heartRateWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.AntRadio.GetChannel(1).SetChannelID(101, false, 120, 1, 500);
            var status = App.AntRadio.GetChannel(1).SendExtAcknowledgedData(101, 120, 0x01, new byte[] { 0x46, 0xFF, 0xFF, 0xFF, 0xFF, 0x80, 6, 1 }, 100);
        }
    }
}
