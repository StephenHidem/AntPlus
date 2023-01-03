using AntPlus.DeviceProfiles.BikeSpeedAndCadence;

namespace AntPlusUsbClient.ViewModels
{
    internal class BikeCadenceViewModel
    {
        private readonly BikeCadenceSensor cadenceSensor;
        public BikeCadenceSensor BikeCadenceSensor => cadenceSensor;

        public BikeCadenceViewModel(BikeCadenceSensor bikeCadence)
        {
            cadenceSensor = bikeCadence;
        }
    }
}
