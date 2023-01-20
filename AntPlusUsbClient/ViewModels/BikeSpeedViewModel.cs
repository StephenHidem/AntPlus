using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;

namespace AntPlusUsbClient.ViewModels
{
    internal class BikeSpeedViewModel
    {
        private readonly BikeSpeedSensor speedSensor;

        public BikeSpeedSensor BikeSpeedSensor => speedSensor;

        public BikeSpeedViewModel(BikeSpeedSensor bikeSpeed)
        {
            speedSensor = bikeSpeed;
        }
    }
}
