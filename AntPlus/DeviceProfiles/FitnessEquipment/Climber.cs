using System;

namespace AntPlus.DeviceProfiles.FitnessEquipment
{
    public class Climber
    {
        private bool isFirstDataMessage = true;
        private byte prevStride;

        public int StrideCycles { get; private set; }
        public byte Cadence { get; private set; }
        public int InstantaneousPower { get; private set; }

        public event EventHandler ClimberChanged;

        public void Parse(byte[] dataPage)
        {
            if (isFirstDataMessage)
            {
                isFirstDataMessage = false;
                prevStride = dataPage[3];
            }
            else
            {
                StrideCycles += Utils.CalculateDelta(dataPage[3], ref prevStride);
            }
            Cadence = dataPage[4];
            InstantaneousPower = BitConverter.ToUInt16(dataPage, 5);
            ClimberChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
