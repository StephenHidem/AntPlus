using System;

namespace DeviceProfiles
{
    public class BicyclePowerSensor
    {
        public event EventHandler<StandardPowerOnly> PowerOnlyChanged;

        public StandardPowerOnly PowerOnlySensor { get; private set; }
        public TorqueEffectivenessAndPedalSmoothness TEPS { get; private set; }

        public BicyclePowerSensor()
        {
            PowerOnlySensor = new StandardPowerOnly();
        }

        public virtual void Parse(byte[] dataPage)
        {
            PowerOnlySensor.Parse(dataPage);
            PowerOnlyChanged?.Invoke(this, PowerOnlySensor);
        }
    }
}
