using SmallEarthTech.AntPlus.DeviceProfiles.BikeSpeedAndCadence;

namespace WpfUsbStickApp.ViewModels
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
