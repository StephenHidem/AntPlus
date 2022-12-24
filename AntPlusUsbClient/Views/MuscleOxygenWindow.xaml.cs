using AntPlus.DeviceProfiles;
using AntPlusUsbClient.ViewModels;
using System.Windows;

namespace AntPlusUsbClient.Views
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
