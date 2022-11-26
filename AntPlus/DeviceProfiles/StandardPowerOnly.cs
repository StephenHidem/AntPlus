using System;

namespace AntPlus.DeviceProfiles
{
    public class StandardPowerOnly
    {
        protected bool isFirstDataMessage = true;     // used for accumulated values
        private byte lastEventCount;
        private int previousAccumulatedEventCount;

        private ushort lastPower;
        private int previousAccumulatedPower;

        public int AccumulatedEventCount { get; private set; }
        public double AveragePower { get; private set; }
        public byte PedalPower { get; private set; }
        public byte InstantaneousCadence { get; private set; }
        public int AccumulatedPower { get; private set; }
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
                AccumulatedEventCount += Utils.CalculateDelta(dataPage[1], ref lastEventCount);
                AccumulatedPower += Utils.CalculateDelta(BitConverter.ToUInt16(dataPage, 4), ref lastPower);
                AveragePower = (AccumulatedPower - previousAccumulatedPower) / (AccumulatedEventCount - previousAccumulatedEventCount);
                previousAccumulatedEventCount = AccumulatedEventCount;
                previousAccumulatedPower = AccumulatedPower;
            }
        }
    }
}
