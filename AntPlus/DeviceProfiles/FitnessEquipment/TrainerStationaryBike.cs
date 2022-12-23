using System;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    public class TrainerStationaryBike
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        public int StrideCount { get; private set; }
        public byte Cadence { get; private set; }
        public int InstantaneousPower { get; private set; }

        public event EventHandler TrainerChanged;

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
            Cadence = dataPage[2];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 5) & 0x0FFF;
            TrainerChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ParseTorque(byte[] dataPage)
        {

        }
    }
}
