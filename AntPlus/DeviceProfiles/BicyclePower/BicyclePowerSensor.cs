using AntPlus;
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
        public CommonDataPages CommonDataPages { get; private set; }

        public BicyclePowerSensor(BicyclePower bp)
        {
            bicyclePower = bp;
            PowerOnlySensor = new StandardPowerOnly();
            CommonDataPages = new CommonDataPages();
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

        public void ParseCommonDataPage(byte[] dataPage)
        {
            CommonDataPages.ParseCommonDataPage(dataPage);
        }
    }
}
