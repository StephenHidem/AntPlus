using System;
using System.ComponentModel;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    public class StandardPowerSensor : INotifyPropertyChanged
    {
        private bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastEventCount;
        private int deltaEventCount;
        private ushort lastPower;
        private int deltaPower;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double AveragePower { get; private set; }
        public byte PedalPower { get; private set; }
        public byte InstantaneousCadence { get; private set; }
        public ushort InstantaneousPower { get; private set; }
        public Parameters Parameters { get; private set; }
        public TorqueEffectivenessAndPedalSmoothness TorqueEffectiveness { get; private set; } = new TorqueEffectivenessAndPedalSmoothness();
        public CommonDataPages CommonDataPages { get; private set; } = new CommonDataPages();


        public StandardPowerSensor(BicyclePower bp)
        {
            Parameters = new Parameters(bp);
        }


        public void Parse(byte[] dataPage)
        {
            PedalPower = dataPage[2];
            InstantaneousCadence = dataPage[3];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 6);
            RaisePropertyChange(nameof(PedalPower));
            RaisePropertyChange(nameof(InstantaneousCadence));
            RaisePropertyChange(nameof(InstantaneousPower));

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
                RaisePropertyChange(nameof(AveragePower));
            }
        }

        public void ParseParameters(byte[] dataPage)
        {
            Parameters.Parse(dataPage);
        }

        public void ParseTEPS(byte[] dataPage)
        {
            TorqueEffectiveness.Parse(dataPage);
        }

        public void ParseCommonDataPage(byte[] dataPage)
        {
            CommonDataPages.ParseCommonDataPage(dataPage);
        }
    }
}
