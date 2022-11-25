using AntPlus;
using AntPlusUsbClient.Views;
using DeviceProfiles;
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
            AntDevice antDevice = ((ListView)sender).SelectedItem as AntDevice;
            switch (antDevice.ChannelId.DeviceType)
            {
                case HeartRate.DeviceClass:
                    HeartRateWindow heartRateWindow = new HeartRateWindow((HeartRate)antDevice);
                    heartRateWindow.Show();
                    break;
                case BicyclePower.DeviceClass:
                    BicyclePowerWindow bpWindow = new BicyclePowerWindow((BicyclePower)antDevice);
                    bpWindow.Show();
                    break;
                default:
                    break;
            }
        }
    }
}
