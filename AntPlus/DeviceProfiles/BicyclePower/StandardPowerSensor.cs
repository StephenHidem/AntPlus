using AntPlus;
using DeviceProfiles.BicyclePower;
using System;

namespace DeviceProfiles
{
    public class StandardPowerSensor
    {
        private bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastEventCount;
        private int deltaEventCount;
        private ushort lastPower;
        private int deltaPower;

        public Parameters Parameters { get; private set; }
        public CommonDataPages CommonDataPages { get; private set; }

        public event EventHandler<StandardPowerSensor> PowerOnlyChanged;
        public event EventHandler<MeasurementOutputData> MeasurementOutputDataChanged;
        public event EventHandler<TorqueEffectivenessAndPedalSmoothness> TEPSPageChanged;
        public event EventHandler<Parameters> ParametersChanged;


        public StandardPowerSensor(BicyclePower.BicyclePower bp)
        {
            Parameters = new Parameters(bp);
            CommonDataPages = new CommonDataPages();
        }

        public double AveragePower { get; private set; }
        public byte PedalPower { get; private set; }
        public byte InstantaneousCadence { get; private set; }
        public ushort InstantaneousPower { get; private set; }

        public void Parse(byte[] dataPage)
        {
            PedalPower = dataPage[2];
            InstantaneousCadence = dataPage[3];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 6);

            if (isFirstDataMessage)
            {
                // initialize if first data message
                isFirstDataMessage = false;
                lastEventCount = dataPage[1];
                lastPower = BitConverter.ToUInt16(dataPage, 4);
                return;
            }

            if (dataPage[1] != lastEventCount)
            {
                // handle new events
                deltaEventCount = Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                deltaPower = Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastPower);
                AveragePower = deltaPower / deltaEventCount;
            }
            PowerOnlyChanged?.Invoke(this, null);
        }

        public void ParseParameters(byte[] dataPage)
        {
            Parameters.Parse(dataPage);
            ParametersChanged?.Invoke(this, Parameters);
        }

        public void ParseMeasurementOutputData(byte[] dataPage)
        {
            MeasurementOutputDataChanged?.Invoke(this, new MeasurementOutputData(dataPage));
        }

        public void ParseTEPS(byte[] dataPage)
        {
            TEPSPageChanged?.Invoke(this, new TorqueEffectivenessAndPedalSmoothness(dataPage));
        }

        public void ParseCommonDataPage(byte[] dataPage)
        {
            CommonDataPages.ParseCommonDataPage(dataPage);
        }
    }
}
