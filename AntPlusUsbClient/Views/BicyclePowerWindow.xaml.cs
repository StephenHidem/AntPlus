using AntPlusUsbClient.ViewModels;
using DeviceProfiles;
using System.Windows;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for BicyclePowerWindow.xaml
    /// </summary>
    public partial class BicyclePowerWindow : Window
    {
        public BicyclePowerWindow(BicyclePower bicyclePower)
        {
            InitializeComponent();
            BicyclePowerViewModel bicyclePowerViewModel = new BicyclePowerViewModel(bicyclePower);
            //CommandBindings.Add(bicyclePowerViewModel.ManualCalRequestBinding);
            //CommandBindings.Add(bicyclePowerViewModel.SetAutoZeroConfigBinding);
            //CommandBindings.Add(bicyclePowerViewModel.GetCustomParametersBinding);
            //CommandBindings.Add(bicyclePowerViewModel.SetCustomParametersBinding);
            //CommandBindings.Add(bicyclePowerViewModel.SaveSerialNumberBinding);
            //CommandBindings.Add(bicyclePowerViewModel.SaveSlopeBinding);
            CommandBindings.AddRange(bicyclePowerViewModel.CommandBindings);
            DataContext = bicyclePowerViewModel;
        }
    }
}
