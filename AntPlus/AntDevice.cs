using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.ComponentModel;
using System.Diagnostics;
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

        /// <summary>This field supplies the generic ANT+ image
        /// from the manifest resource stream.</summary>
        public readonly static Stream AntImage = typeof(AntDevice).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.AntPlus.png");

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

        /// <summary>Occurs when no messages have been received from the device within the specified timeout duration.</summary>
        /// <remarks>
        /// Consumers can also use the <see cref="Offline"/> property instead of this event for databinding
        /// purposes.
        /// </remarks>
        public event EventHandler DeviceWentOffline;

        /// <summary>Gets a value indicating whether this <see cref="AntDevice" /> is offline.</summary>
        /// <value>
        ///   <c>true</c> if offline; otherwise, <c>false</c>.</value>
        /// <remarks>The <see cref="DeviceWentOffline" /> event is also invoked when the device has not received a message within the timeout specified for this device.</remarks>
        public bool Offline { get; private set; }

        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        public ChannelId ChannelId { get; private set; }

        /// <summary>Gets the device image stream from the embedded resource image associated with the derived device class.</summary>
        /// <value>The device image stream.</value>
        public abstract Stream DeviceImageStream { get; }

        /// <summary>Initializes a new instance of the <see cref="AntDevice" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="DeviceWentOffline"/>.</param>
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
            Offline = true;
            RaisePropertyChange(nameof(Offline));
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
            return GetType().Name;
        }

        /// <summary>Requests the data page.</summary>
        /// <typeparam name="T">The data page enumeration of the derived ANT device class.</typeparam>
        /// <param name="page">The requested page.</param>
        /// <param name="ackWaitTime">Time in milliseconds to wait for the device acknowledgment. The default is 500ms.</param>
        /// <param name="decriptor1">The decriptor1. The default is 0xFF.</param>
        /// <param name="descriptor2">The descriptor2. The default is 0xFF.</param>
        /// <param name="transmissionResponse">The transmission response. The default is to send 4 messages.</param>
        /// <param name="commandType">Type of the command. The default is <see cref="CommandType.DataPage"/>.</param>
        /// <param name="slaveSerialNumber">The slave serial number. The default is 0xFFFF.</param>
        /// <returns>Returns the <see cref="MessagingReturnCode"/>.</returns>
        /// <exception cref="System.ArgumentException">Invalid data page requested.</exception>
        public MessagingReturnCode RequestDataPage<T>(T page, uint ackWaitTime = 500, byte decriptor1 = 0xFF, byte descriptor2 = 0xFF, byte transmissionResponse = 4, CommandType commandType = CommandType.DataPage, ushort slaveSerialNumber = 0xFFFF) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), page))
            {
                byte[] msg = new byte[] { (byte)CommonDataPage.RequestDataPage, 0, 0, decriptor1, descriptor2, transmissionResponse, Convert.ToByte(page), (byte)commandType };
                BitConverter.GetBytes(slaveSerialNumber).CopyTo(msg, 1);
                return antChannel.SendExtAcknowledgedData(ChannelId, msg, ackWaitTime);
            }
            else
            {
                throw new ArgumentException("Invalid data page requested.");
            }
        }

        /// <summary>Sends an acknowledged message to the ANT device.</summary>
        /// <param name="message">The message.</param>
        /// <param name="ackWaitTime">Time in milliseconds to wait for the device acknowledgment. The default is 500ms.</param>
        /// <returns>Returns the <see cref="MessagingReturnCode"/>.</returns>
        public MessagingReturnCode SendExtAcknowledgedMessage(byte[] message, uint ackWaitTime = 500)
        {
            int retries = 3;
            MessagingReturnCode ret;
            do
            {
                ret = antChannel.SendExtAcknowledgedData(ChannelId, message, ackWaitTime);
                Debug.WriteLineIf(ret != MessagingReturnCode.Pass, string.Format("SendExtAcknowledgedMessage error: Page = {0}, Code = {1}, Retries = {2}", message[0], ret, retries));
            } while (ret != MessagingReturnCode.Pass && --retries > 0);
            return ret;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            timeoutTimer?.Dispose();
            timeoutTimer = null;
        }
    }
}
