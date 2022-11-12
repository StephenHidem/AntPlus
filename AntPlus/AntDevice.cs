using AntRadioInterface;
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

    /// <summary>
    /// Base class for all ANT devices.
    /// </summary>
    public abstract class AntDevice
    {
        protected bool isFirstDataMessage = true;     // used for accumulated values
        private byte[] message;


        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        public ChannelId ChannelId { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="AntDevice" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        protected AntDevice(ChannelId channelId)
        {
            ChannelId = channelId;
        }

        /// <summary>Parses the specified data page.</summary>
        /// <param name="payload">The data page.</param>
        public abstract void Parse(byte[] dataPage);
        public abstract void ChannelEventHandler(EventMsgId eventMsgId);
        public abstract void ChannelResponseHandler(byte messageId, ResponseMsgId responseMsgId);

        protected void SendMessage(byte[] msg)
        {
            message = msg;
            // TODO: QUEUE UP ANT RADIO TO SEND MESSAGE
        }

        public byte[] GetMessage()
        {
            return message;
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

        /// <summary>Requests the data page.</summary>
        /// <param name="channelNumber">The channel number.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="transmissionResponse">The transmission response.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="slaveSerialNumber">The slave serial number.</param>
        /// <param name="decriptor1">The decriptor1.</param>
        /// <param name="descriptor2">The descriptor2.</param>
        public void RequestDataPage(byte channelNumber, byte pageNumber, byte transmissionResponse = 0x04, CommandType commandType = CommandType.DataPage, ushort slaveSerialNumber = 0xFFFF, byte decriptor1 = 0xFF, byte descriptor2 = 0xFF)
        {
            byte[] msg = new byte[] { (byte)CommonDataPageType.RequestDataPage, 0, 0, decriptor1, descriptor2, transmissionResponse, pageNumber, (byte)commandType };
            BitConverter.GetBytes(slaveSerialNumber).CopyTo(msg, 1);
            SendExtendedAcknowledgedMessage(channelNumber, msg);
        }

        /// <summary>Sends the extended acknowledged message.</summary>
        /// <param name="channelNumber">The channel number.</param>
        /// <param name="message">The message.</param>
        public void SendExtendedAcknowledgedMessage(byte channelNumber, byte[] message)
        {
            byte[] msg = new byte[] { 13, (byte)MessageId.ExtAcknowledgedData, channelNumber };
            msg = msg.Concat(BitConverter.GetBytes(ChannelId.Id)).Concat(message).ToArray();
            SendMessage(msg);
        }
    }
}
