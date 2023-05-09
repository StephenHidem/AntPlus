using SmallEarthTech.AntPlus.DeviceProfiles.FitnessEquipment;
using System.Windows.Input;

namespace AntPlusUsbClient.ViewModels
{
    internal class FitnessEquipmentViewModel
    {
        private readonly FitnessEquipment fitnessEquipment;

        public FitnessEquipment FitnessEquipment => fitnessEquipment;

        public RoutedCommand FECapabilitiesRequest { get; private set; } = new RoutedCommand();
        public RoutedCommand SetUserConfig { get; private set; } = new RoutedCommand();
        public RoutedCommand SetBasicResistance { get; private set; } = new RoutedCommand();
        public RoutedCommand SetTargetPower { get; private set; } = new RoutedCommand();
        public RoutedCommand SetWindResistance { get; private set; } = new RoutedCommand();
        public RoutedCommand SetTrackResistance { get; private set; } = new RoutedCommand();
        public CommandBinding[] CommandBindings { get; private set; }

        public FitnessEquipmentViewModel(FitnessEquipment fitnessEquipment)
        {
            this.fitnessEquipment = fitnessEquipment;
            CommandBindings = new CommandBinding[] {
                new CommandBinding(FECapabilitiesRequest, (s, e) => FitnessEquipment.RequestFECapabilities()),
                new CommandBinding(SetUserConfig, (s, e) => FitnessEquipment.SetUserConfiguration(95, 10, 15, 2.2, 4)),
                new CommandBinding(SetBasicResistance, (s, e) => FitnessEquipment.SetBasicResistance(50)),
                new CommandBinding(SetTargetPower, (s, e) => FitnessEquipment.SetTargetPower(200)),
                new CommandBinding(SetWindResistance, (s, e) => FitnessEquipment.SetWindResistance(0.51, -30, 0.9)),
                new CommandBinding(SetTrackResistance, (s, e) => FitnessEquipment.SetTrackResistance(-15)),
            };
        }
    }
}
