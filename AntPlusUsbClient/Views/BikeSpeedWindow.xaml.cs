using AntPlusUsbClient.ViewModels;
using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using System.Windows;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for BikeSpeedWindow.xaml
    /// </summary>
    public partial class BikeSpeedWindow : Window
    {
        public BikeSpeedWindow(BikeSpeedSensor bikeSpeed)
        {
            InitializeComponent();
            DataContext = new BikeSpeedViewModel(bikeSpeed);
        }
    }
}
