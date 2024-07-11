using SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower;
using System.Windows;
using WpfUsbStickApp.ViewModels;
namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for CTFWindow.xaml
    /// </summary>
    public partial class CTFWindow : Window
    {
        public CTFWindow(CrankTorqueFrequencySensor ctf)
        {
            InitializeComponent();
            DataContext = new CTFViewModel(ctf);
        }
    }
}
