using SmallEarthTech.AntRadioInterface;
using System;
using System.ComponentModel;
using System.IO;

namespace SmallEarthTech.AntPlus
{
    /// <summary>
    /// Base class for all ANT devices.
    /// </summary>
    public abstract class AntDevice : INotifyPropertyChanged
    {
        /// <summary>The last data page received.</summary>
        protected byte[] lastDataPage = new byte[8];
        private readonly IAntChannel antChannel;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises the property change event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


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

        /// <summary>Gets or sets the device image stream.</summary>
        /// <value>The device image stream.</value>
        public abstract Stream DeviceImageStream { get; }

        /// <summary>Parses the specified data page.</summary>
        /// <param name="dataPage">The received data page.</param>
        public abstract void Parse(byte[] dataPage);
        //public abstract void ChannelEventHandler(EventMsgId eventMsgId);
        //public abstract void ChannelResponseHandler(byte messageId, ResponseMsgId responseMsgId);

        /// <inheritdoc/>
        /// <remarks>Overridden to provide the short class name versus the full namepace name.</remarks>
        public override string ToString()
        {
            return base.GetType().Name;
        }

        /// <summary>Requests the data page.</summary>
        /// <typeparam name="T">The data page enumeration of the derived ANT device class.</typeparam>
        /// <param name="page">The requested page.</param>
        /// <param name="decriptor1">The decriptor1.</param>
        /// <param name="descriptor2">The descriptor2.</param>
        /// <param name="transmissionResponse">The transmission response. The default is to send 4 messages.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="slaveSerialNumber">The slave serial number.</param>
        /// <exception cref="System.ArgumentException">Invalid data page requested.</exception>
        public void RequestDataPage<T>(T page, byte decriptor1 = 0xFF, byte descriptor2 = 0xFF, byte transmissionResponse = 4, CommandType commandType = CommandType.DataPage, ushort slaveSerialNumber = 0xFFFF) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), page))
            {
                byte[] msg = new byte[] { (byte)CommonDataPage.RequestDataPage, 0, 0, decriptor1, descriptor2, transmissionResponse, Convert.ToByte(page), (byte)commandType };
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
