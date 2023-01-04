using AntPlus.DeviceProfiles.FitnessEquipment;

namespace AntPlusUsbClient.ViewModels
{
    internal class FitnessEquipmentViewModel
    {
        private readonly FitnessEquipment fitnessEquipment;

        public FitnessEquipment FitnessEquipment => fitnessEquipment;
        public Treadmill Treadmill { get; private set; }
        public Elliptical Elliptical { get; private set; }
        public Rower Rower { get; private set; }
        public Climber Climber { get; private set; }
        public NordicSkier NordicSkier { get; private set; }
        public TrainerStationaryBike TrainerStationaryBike { get; private set; }

        public FitnessEquipmentViewModel(FitnessEquipment fitnessEquipment)
        {
            this.fitnessEquipment = fitnessEquipment;
        }
    }
}
