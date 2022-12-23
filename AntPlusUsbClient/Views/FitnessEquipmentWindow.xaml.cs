using AntPlus.DeviceProfiles.FitnessEquipment;
using AntPlusUsbClient.Controls;
using AntPlusUsbClient.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace AntPlusUsbClient.Views
{
    /// <summary>
    /// Interaction logic for FitnessEquipmentWindow.xaml
    /// </summary>
    public partial class FitnessEquipmentWindow : Window
    {
        public FitnessEquipmentWindow(FitnessEquipment fitnessEquipment)
        {
            UserControl control = null;

            InitializeComponent();
            FitnessEquipmentViewModel fevm = new FitnessEquipmentViewModel(fitnessEquipment);

            switch (fitnessEquipment.GeneralData.EquipmentType)
            {
                case FitnessEquipment.FitnessEquipmentType.Treadmill:
                    control = new TreadmillControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.Elliptical:
                    control = new EllipticalControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.Rower:
                    control = new RowerControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.Climber:
                    control = new ClimberControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.NordicSkier:
                    control = new NordicSkierControl(fitnessEquipment);
                    break;
                case FitnessEquipment.FitnessEquipmentType.TrainerStationaryBike:
                    control = new TrainerStationaryBikeControl(fitnessEquipment);
                    break;
                default:
                    break;
            }

            FESpecific.Children.Add(control);
            DataContext = fevm;
        }
    }
}
