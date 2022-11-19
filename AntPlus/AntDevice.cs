using AntRadioInterface;
using System;

namespace AntPlus
{
    /// <summary>
    /// Base class for all ANT devices.
    /// </summary>
    public abstract class AntDevice
    {
        protected bool isFirstDataMessage = true;     // used for accumulated values
        private byte[] message;
        private readonly IAntChannel antChannel;


        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        public ChannelId ChannelId { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="AntDevice" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        protected AntDevice(ChannelId channelId, IAntChannel antChannel)
        {
            ChannelId = channelId;
            this.antChannel = antChannel;
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
        /// <typeparam name="T">The data page enumeration of the derived ANT device class.</typeparam>
        /// <param name="page">The requested page.</param>
        /// <param name="transmissionResponse">The transmission response.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="slaveSerialNumber">The slave serial number.</param>
        /// <param name="decriptor1">The decriptor1.</param>
        /// <param name="descriptor2">The descriptor2.</param>
        /// <exception cref="System.ArgumentException">Invalid data page requested.</exception>
        public void RequestDataPage<T>(T page, byte transmissionResponse = 0x80, CommandType commandType = CommandType.DataPage, ushort slaveSerialNumber = 0xFFFF, byte decriptor1 = 0xFF, byte descriptor2 = 0xFF) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), page))
            {
                byte[] msg = new byte[] { (byte)CommonDataPageType.RequestDataPage, 0, 0, decriptor1, descriptor2, transmissionResponse, Convert.ToByte(page), (byte)commandType };
                BitConverter.GetBytes(slaveSerialNumber).CopyTo(msg, 1);
                antChannel.SendExtAcknowledgedData(ChannelId, msg, 500);
            }
            else
            {
                throw new ArgumentException("Invalid data page requested.");
            }
        }

        /// <summary>Sends the acknowledged message.</summary>
        /// <param name="message">The message.</param>
        public void SendExtAcknowledgedMessage(byte[] message)
        {
            antChannel.SendExtAcknowledgedData(ChannelId, message, 500);
        }
    }
}
