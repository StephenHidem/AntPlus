using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles
{
    /// <summary>
    /// This class supports unknown devices.
    /// </summary>
    /// <seealso cref="AntDevice" />
    public class UnknownDevice : AntDevice
    {
        /// <inheritdoc/>
        public override int ChannelCount => 8192;

        /// <summary>
        /// The collection lock.
        /// </summary>
        /// <remarks>
        /// An application should use the collection lock to ensure thread safe access to the
        /// collection. For example, the code behind for a WPF window should include -
        /// <code>BindingOperations.EnableCollectionSynchronization(unknownDevice.DataPages, unknownDevice.CollectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
        public object CollectionLock = new object();

        /// <summary>
        /// Gets the collection of data pages received from the unknown device.
        /// </summary>
        /// <value>
        /// The data pages.
        /// </value>
        public ObservableCollection<byte[]> DataPages { get; private set; } = new ObservableCollection<byte[]>();
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(UnknownDevice).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.Unknown.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownDevice"/> class.
        /// </summary>
        /// <inheritdoc cref="AntDevice(ChannelId, IAntChannel, ILogger, int?, byte?)"/>
        public UnknownDevice(ChannelId channelId, IAntChannel antChannel, ILogger<UnknownDevice> logger, int? timeout = default, byte? missedMessages = default)
            : base(channelId, antChannel, logger, timeout, missedMessages)
        {
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            _logger.LogDebug("Data page = {0}", BitConverter.ToString(dataPage));

            byte[] page = DataPages.FirstOrDefault(p => p[0] == dataPage[0]);
            if (page == null)
            {
                lock (CollectionLock)
                {
                    DataPages.Add(dataPage);
                }
            }
            else
            {
                if (!page.SequenceEqual(dataPage))
                {
                    DataPages[DataPages.IndexOf(page)] = dataPage;
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("Unknown Device Type: {0}", ChannelId.DeviceType);
        }
    }
}
