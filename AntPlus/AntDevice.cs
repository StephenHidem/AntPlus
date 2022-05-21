using System;

namespace AntPlus
{
    /// <summary>
    /// ANT channel sharing enumeration. This is obtained from the transmission type in the channel ID.
    /// </summary>
    public enum ChannelSharing
    {
        /// <summary>The reserved</summary>
        Reserved = 0,
        /// <summary>
        /// The independent channel
        /// </summary>
        IndependentChannel = 1,
        /// <summary>
        /// The shared channel one byte address
        /// </summary>
        SharedChannelOneByteAddress = 2,
        /// <summary>
        /// The shared channel two byte address
        /// </summary>
        SharedChannelTwoByteAddress = 3,
    }

    /// <summary>
    /// Base class for all ANT devices.
    /// </summary>
    public class AntDevice
    {
        protected bool isFirstDataMessage = true;     // used for accumulated values


        /// <summary>
        /// Gets the channel identifier.
        /// </summary>
        /// <value>
        /// The channel identifier.
        /// </value>
        public uint ChannelId { get; private set; }
        public byte DeviceType => (byte)(ChannelId >> 16 & 0x0000007F);
        public uint DeviceNumber => (ChannelId & 0x0000FFFF) + (ChannelId >> 12 & 0x000F0000);
        public bool IsPairingBitSet => (ChannelId & 0x00800000) == 0x00800000;
        public ChannelSharing TransmissionType => (ChannelSharing)(ChannelId >> 24 & 0x00000003);
        public bool AreGlobalDataPagesUsed => (ChannelId & 0x04000000) == 0x04000000;

        public AntDevice(byte[] payload, uint channelId)
        {
            ChannelId = channelId;
        }

        protected int UpdateAccumulatedValue(byte value, ref byte lastValue, int accumulatedValue)
        {
            if (isFirstDataMessage)
            {
                lastValue = value;
                return 0;
            }

            accumulatedValue += value - lastValue;
            if (lastValue > value)
            {
                // rollover
                accumulatedValue += 256;
            }

            lastValue = value;
            return accumulatedValue;
        }

        protected int UpdateAccumulatedValue(ushort value, ref ushort lastValue, int accumulatedValue)
        {
            if (isFirstDataMessage)
            {
                lastValue = value;
                return 0;
            }

            accumulatedValue += value - lastValue;
            if (lastValue > value)
            {
                // rollover
                accumulatedValue += 0x10000;
            }

            lastValue = value;
            return accumulatedValue;
        }

        public void RequestDataPage(byte pageNumber, byte transmissionResponse = 0x04, CommandType commandType = CommandType.DataPage, ushort slaveSerialNumber = 0xFFFF, byte decriptor1 = 0xFF, byte descriptor2 = 0xFF)
        {
            byte[] msg = new byte[] { (byte)CommonDataPageType.RequestDataPage, 0, 0, decriptor1, descriptor2, transmissionResponse, pageNumber, (byte)commandType };
            BitConverter.GetBytes(slaveSerialNumber).CopyTo(msg, 1);
        }

        public void SendAcknowledgedMessage(byte[] message)
        {

        }
    }
}
