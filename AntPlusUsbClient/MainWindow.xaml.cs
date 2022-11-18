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
    }
}
