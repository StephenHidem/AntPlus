﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntPlus.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
    /// 2 seconds. Consult the device profile documentation at https://www.thisisant.com for a device and consult the channel
    /// period defined for master devices.
    /// </remarks>
    public abstract partial class AntDevice : ObservableObject, IDisposable
    {
        private readonly IAntChannel _antChannel;
        private Timer? _timeoutTimer;
        private readonly int _deviceTimeout;
        private const double _baseTransmissionFrequency = 32768;  // base ANT device data transmission frequency in Hz

        /// <summary>This is a multiple of the base transmission frequency. All derived classes must implement this property.
        /// See the master Channel Period specified in the specific ANT device specification for the count value.</summary>
        public abstract int ChannelCount { get; }

        /// <summary>The logger for derived classes to use.</summary>
        protected readonly ILogger _logger;

        /// <summary>Occurs when no messages have been received from the device within the specified timeout duration.</summary>
        /// <remarks>
        /// Consumers can also use the <see cref="Offline"/> property instead of this event for databinding
        /// purposes.
        /// </remarks>
        public event EventHandler? DeviceWentOffline;

        /// <summary>Gets a value indicating whether this <see cref="AntDevice" /> is offline.</summary>
        /// <value>
        ///   <c>true</c> if offline; otherwise, <c>false</c>.</value>
        /// <remarks>The <see cref="DeviceWentOffline" /> event is also invoked when the device has not received a message within the timeout specified for this device.</remarks>
        [ObservableProperty]
        private bool offline;

        /// <summary>Gets the channel identifier.</summary>
        /// <value>The channel identifier.</value>
        public ChannelId ChannelId { get; private set; }

        /// <summary>Gets the device image stream from the embedded resource image associated with the derived device class.</summary>
        /// <value>The device image stream.</value>
        public abstract Stream? DeviceImageStream { get; }

        /// <summary>Initializes a new instance of the <see cref="AntDevice" /> class.</summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">
        /// Time in milliseconds before firing <see cref="DeviceWentOffline"/>.
        /// To disable the device timeout, set the timeout argument to -1 and skip 
        /// setting the missedMessages argument.
        /// </param>
        protected AntDevice(ChannelId channelId, IAntChannel antChannel, ILogger logger, int timeout)
        {
            ChannelId = channelId;
            _antChannel = antChannel;
            _logger = logger;
            _deviceTimeout = timeout;
            _timeoutTimer = new Timer(TimeoutCallback, null, _deviceTimeout, Timeout.Infinite);
            _logger.LogAntDeviceState(this, _deviceTimeout);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AntDevice"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeoutOptions">Timeout options.</param>
        protected AntDevice(ChannelId channelId, IAntChannel antChannel, ILogger logger, TimeoutOptions? timeoutOptions)
        {
            ChannelId = channelId;
            _antChannel = antChannel;
            _logger = logger;
            if (timeoutOptions?.Timeout == null)
            {
                _deviceTimeout = (int)Math.Ceiling(((timeoutOptions?.MissedMessages ?? 8) / (_baseTransmissionFrequency / ChannelCount)) * 1000);
            }
            else
            {
                _deviceTimeout = (int)timeoutOptions.Timeout;
            }
            _timeoutTimer = new Timer(TimeoutCallback, null, _deviceTimeout, Timeout.Infinite);
            _logger.LogAntDeviceState(this, _deviceTimeout);
        }

        private void TimeoutCallback(object state)
        {
            Offline = true;
            Dispose();
            DeviceWentOffline?.Invoke(this, new EventArgs());
        }

        /// <summary>Parses the specified data page.</summary>
        /// <param name="dataPage">The received data page.</param>
        public virtual void Parse(byte[] dataPage)
        {
            _logger.LogDataPage(LogLevel.Trace, dataPage);
            _ = _timeoutTimer?.Change(_deviceTimeout, Timeout.Infinite);
        }

        /// <inheritdoc/>
        /// <remarks>Overridden to provide the short class name versus the full namespace name.</remarks>
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>Requests the data page.</summary>
        /// <typeparam name="T">The data page enumeration of the derived ANT device class.</typeparam>
        /// <param name="page">The requested page.</param>
        /// <param name="descriptor1">The descriptor1. The default is 0xFF.</param>
        /// <param name="descriptor2">The descriptor2. The default is 0xFF.</param>
        /// <param name="transmissionResponse">The transmission response. The default is to send 4 messages.</param>
        /// <param name="commandType">Type of the command. The default is <see cref="CommandType.DataPage"/>.</param>
        /// <param name="slaveSerialNumber">The slave serial number. The default is 0xFFFF.</param>
        /// <returns>Status of the request.</returns>
        /// <exception cref="System.ArgumentException">Invalid data page requested.</exception>
        public async Task<MessagingReturnCode> RequestDataPage<T>(T page, byte descriptor1 = 0xFF, byte descriptor2 = 0xFF, byte transmissionResponse = 4, CommandType commandType = CommandType.DataPage, ushort slaveSerialNumber = 0xFFFF) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), page))
            {
                byte[] msg = new byte[] { (byte)CommonDataPage.RequestDataPage, 0, 0, descriptor1, descriptor2, transmissionResponse, Convert.ToByte(page), (byte)commandType };
                BitConverter.GetBytes(slaveSerialNumber).CopyTo(msg, 1);
                return await _antChannel.SendExtAcknowledgedDataAsync(ChannelId, msg, (uint)_deviceTimeout);
            }
            else
            {
                throw new ArgumentException("Invalid data page requested.", nameof(page));
            }
        }

        /// <summary>Sends an acknowledged message to the ANT device.</summary>
        /// <param name="message">The message.</param>
        /// <returns>Status of the request.</returns>
        public async Task<MessagingReturnCode> SendExtAcknowledgedMessage(byte[] message)
        {
            MessagingReturnCode ret = await _antChannel.SendExtAcknowledgedDataAsync(ChannelId, message, (uint)_deviceTimeout);
            return ret;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _logger.LogAntDeviceState(this, _deviceTimeout);
            _timeoutTimer?.Dispose();
            _timeoutTimer = null;
        }
    }
}
