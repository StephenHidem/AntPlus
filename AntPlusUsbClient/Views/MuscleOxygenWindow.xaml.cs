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
            MuscleOxygenViewModel moxy = new MuscleOxygenViewModel(muscleOxygen);
            CommandBindings.AddRange(moxy.CommandBindings);
            DataContext = moxy;
        }
    }
}
