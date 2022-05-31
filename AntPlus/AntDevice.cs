using System;
using System.Linq;

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

    public enum MessageId

    {
        BroadcastData = 0x4E,
        AcknowledgedData = 0x4F,
        BurstData = 0x50,
        ExtBroadcastData = 0x5D,
        ExtAcknowledgedData = 0x5E,
        ExtBurstData = 0x5F
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
            Parse(payload);
        }

        public virtual void Parse(byte[] payload)
        {
        }

        /// <summary>
        /// Calculates the delta of the current and previous values. Rollover is accounted for and a positive integer is always returned.
        /// Add the returned value to the accumulated value in the derived class. The last value is updated with the current value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="lastValue">The last value.</param>
        /// <returns>Positive delta of the current and previous values.</returns>
        protected int CalculateDelta(byte currentValue, ref byte lastValue)
        {
            if (isFirstDataMessage)
            {
                lastValue = currentValue;
                return 0;
            }

            int delta = currentValue - lastValue;
            if (lastValue > currentValue)
            {
                // rollover
                delta += 256;
            }

            lastValue = currentValue;
            return delta;
        }

        /// <summary>
        /// Calculates the delta of the current and previous values. Rollover is accounted for and a positive integer is always returned.
        /// Add the returned value to the accumulated value in the derived class. The last value is updated with the current value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="lastValue">The last value.</param>
        /// <returns>Positive delta of the current and previous values.</returns>
        protected int CalculateDelta(ushort currentValue, ref ushort lastValue)
        {
            if (isFirstDataMessage)
            {
                lastValue = currentValue;
                return 0;
            }

            int delta = currentValue - lastValue;
            if (lastValue > currentValue)
            {
                // rollover
                delta += 0x10000;
            }

            lastValue = currentValue;
            return delta;
        }

        // TODO: FIX CHANNEL NUMBER
        public void RequestDataPage(byte pageNumber, byte transmissionResponse = 0x04, CommandType commandType = CommandType.DataPage, ushort slaveSerialNumber = 0xFFFF, byte decriptor1 = 0xFF, byte descriptor2 = 0xFF)
        {
            byte[] msg = new byte[] { (byte)CommonDataPageType.RequestDataPage, 0, 0, decriptor1, descriptor2, transmissionResponse, pageNumber, (byte)commandType };
            BitConverter.GetBytes(slaveSerialNumber).CopyTo(msg, 1);
            SendExtendedAcknowledgedMessage(0, msg);
        }

        public void SendExtendedAcknowledgedMessage(byte channelNumber, byte[] message)
        {
            byte[] msg = new byte[] { 13, (byte)MessageId.ExtAcknowledgedData, channelNumber };
            msg = msg.Concat(BitConverter.GetBytes(ChannelId)).Concat(message).ToArray();
            SendMessage(msg);
        }

        public byte[] Message { get; set; }
        private void SendMessage(byte[] msg)
        {
            Message = msg;
        }
    }
}
