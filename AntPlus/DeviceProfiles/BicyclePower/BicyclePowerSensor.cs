using System;
namespace DeviceProfiles.BicyclePower
{
    public class BicyclePowerSensor
    {
        private readonly BicyclePower bicyclePower;

        public event EventHandler<StandardPowerOnly> PowerOnlyChanged;
        public event EventHandler<TorqueEffectivenessAndPedalSmoothness> TEPSPageChanged;
        public event EventHandler<MeasurementOutputData> MeasurementOutputDataChanged;

        public StandardPowerOnly PowerOnlySensor { get; private set; }

        public BicyclePowerSensor(BicyclePower bp)
        {
            bicyclePower = bp;
            PowerOnlySensor = new StandardPowerOnly();
        }

        public virtual void Parse(byte[] dataPage)
        {
            PowerOnlySensor.Parse(dataPage);
            PowerOnlyChanged?.Invoke(this, PowerOnlySensor);
        }

        public void ParseTEPS(byte[] dataPage)
        {
            TEPSPageChanged?.Invoke(this, new TorqueEffectivenessAndPedalSmoothness(dataPage));
        }

        public void ParseMeasurementOutputData(byte[] dataPage)
        {
            MeasurementOutputDataChanged?.Invoke(this, new MeasurementOutputData(dataPage));
        }
    }
}
