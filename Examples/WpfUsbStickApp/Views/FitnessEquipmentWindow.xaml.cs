using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for FitnessEquipmentWindow.xaml
    /// </summary>
    public partial class FitnessEquipmentWindow : Window
    {
        public FitnessEquipmentWindow(FitnessEquipment fitnessEquipment)
        {
            InitializeComponent();
            DataContext = new FitnessEquipmentViewModel(fitnessEquipment);
        }
    }
}
