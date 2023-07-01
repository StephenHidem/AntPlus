using SmallEarthTech.AntPlus.DeviceProfiles;
using System.Windows;
using WpfUsbStickApp.ViewModels;

namespace WpfUsbStickApp.Views
{
    /// <summary>
    /// Interaction logic for MuscleOxygenWindow.xaml
    /// </summary>
    public partial class MuscleOxygenWindow : Window
    {
        public MuscleOxygenWindow(MuscleOxygen muscleOxygen)
        {
            InitializeComponent();
            DataContext = new MuscleOxygenViewModel(muscleOxygen);
        }
    }
}
