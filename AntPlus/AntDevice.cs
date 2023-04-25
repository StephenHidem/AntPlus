using SmallEarthTech.AntPlus.DeviceProfiles.Geocache;
using SmallEarthTech.AntRadioInterface;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace SmallEarthTech.AntPlus
{
    /// <summary>
    /// Base class for all ANT devices.
    /// </summary>
    /// <remarks>
    /// An important consideration is an appropriate timeout to determine if a device has gone offline - battery 
    /// has died, device has been turned off, device is out of range of receiver, ane/or the RF environment the
    /// device and receiver are operating in. Most devices broadcast at 4Hz, with the notable exception of
    /// <see cref="Geocache"/>.
    /// 
    /// A reasonable rule of thumb is to set the timeout at 8 messages times the channel period per second; typically
    /// 2 seconds. Consult the device profile documentation at https://www.thisisant.com for a device and review the channel
    /// period defined for master devices.
    /// </remarks>
    public abstract class AntDevice : INotifyPropertyChanged, IDisposable
    {
        private readonly IAntChannel antChannel;
        private Timer timeoutTimer;
        private readonly int deviceTimeout;

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises the property change event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Occurs when no messages have been received from the device within the timeout duration.</summary>
        public event EventHandler DeviceWentOffline;

        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        public ChannelId ChannelId { get; private set; }

        /// <summary>Gets the device image stream from the embedded resource image associated with the derived device class.</summary>
        /// <value>The device image stream.</value>
        public abstract Stream DeviceImageStream { get; }

        /// <summary>Initializes a new instance of the <see cref="AntDevice" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="DeviceWentOffline"/>.
        /// The default is 2000 milliseconds.</param>
        protected AntDevice(ChannelId channelId, IAntChannel antChannel, int timeout)
        {
            ChannelId = channelId;
            this.antChannel = antChannel;
            deviceTimeout = timeout;
            timeoutTimer = new Timer(TimeoutCallback);
            timeoutTimer.Change(deviceTimeout, Timeout.Infinite);
        }

        private void TimeoutCallback(object state)
        {
            timeoutTimer?.Dispose();
            timeoutTimer = null;
            DeviceWentOffline?.Invoke(this, new EventArgs());
        }

        /// <summary>Parses the specified data page.</summary>
        /// <param name="dataPage">The received data page.</param>
        public virtual void Parse(byte[] dataPage)
        {
            _ = timeoutTimer?.Change(deviceTimeout, Timeout.Infinite);
        }

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

        /// <summary>Sends an acknowledged message to the ANT device.</summary>
        /// <param name="message">The message.</param>
        public void SendExtAcknowledgedMessage(byte[] message)
        {
            antChannel.SendExtAcknowledgedData(ChannelId, message, 500);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            timeoutTimer?.Dispose();
            timeoutTimer = null;
        }
    }
}
