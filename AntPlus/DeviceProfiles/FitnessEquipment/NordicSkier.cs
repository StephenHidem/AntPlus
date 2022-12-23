using System;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    public class NordicSkier
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        public int StrideCount { get; private set; }
        public byte Cadence { get; private set; }
        public int InstantaneousPower { get; private set; }

        public event EventHandler NordicSkierChanged;

        public void Parse(byte[] dataPage)
        {
            if (isFirstDataMessage)
            {
                isFirstDataMessage = false;
                prevStride = dataPage[3];
            }
            else
            {
                StrideCount += Utils.CalculateDelta(dataPage[3], ref prevStride);
            }
            Cadence = dataPage[4];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
            NordicSkierChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
