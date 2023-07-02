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
        /// <summary>
        /// Gets the collection of data pages received from the unknown device.
        /// </summary>
        /// <value>
        /// The data pages.
        /// </value>
        public UnknownDataPages DataPages { get; private set; } = new UnknownDataPages();
        /// <inheritdoc/>
        public override Stream DeviceImageStream => typeof(UnknownDevice).Assembly.GetManifestResourceStream("SmallEarthTech.AntPlus.Images.Unknown.png");

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownDevice"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        /// <param name="logger">Logger to use.</param>
        /// <param name="timeout">Time in milliseconds before firing <see cref="AntDevice.DeviceWentOffline"/>.</param>
        public UnknownDevice(ChannelId channelId, IAntChannel antChannel, ILogger<UnknownDevice> logger, int timeout = 2000) : base(channelId, antChannel, logger, timeout)
        {
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);
            logger.LogDebug("Data page = {0}", BitConverter.ToString(dataPage));

            byte[] page = DataPages.FirstOrDefault(p => p[0] == dataPage[0]);
            if (page == null)
            {
                DataPages.Add(dataPage);
            }
            else
            {
                if (!page.SequenceEqual(dataPage))
                {
                    DataPages[DataPages.IndexOf(page)] = dataPage;
                }
            }
        }
    }

    /// <summary>
    /// A thread safe collection of unknown data pages.
    /// </summary>
    public class UnknownDataPages : ObservableCollection<byte[]>
    {
        /// <summary>
        /// The collection lock.
        /// </summary>
        /// <remarks>
        /// An application should use the collection lock to ensure thread safe access to the
        /// collection. For example, the code behind for a WPF window should include -
        /// <code>BindingOperations.EnableCollectionSynchronization(unknownDevice.DataPages, unknownDevice.DataPages.CollectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
        public object CollectionLock = new object();

        /// <inheritdoc/>
        protected override void InsertItem(int index, byte[] item)
        {
            lock (CollectionLock)
            {
                base.InsertItem(index, item);
            }
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            lock (CollectionLock)
            {
                base.RemoveItem(index);
            }
        }

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            lock (CollectionLock)
            {
                base.ClearItems();
            }
        }

        /// <inheritdoc/>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            lock (CollectionLock)
            {
                base.MoveItem(oldIndex, newIndex);
            }
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, byte[] item)
        {
            lock (CollectionLock)
            {
                base.SetItem(index, item);
            }
        }
    }
}
