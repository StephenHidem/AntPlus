using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;
using System.Windows;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for BikeCadenceWindow.xaml
    /// </summary>
    public partial class BikeCadenceWindow : Window
    {
        public BikeCadenceWindow(BikeCadenceSensor bikeCadence)
        {
            InitializeComponent();
            DataContext = new BikeCadenceViewModel(bikeCadence);
        }
    }
}
