using SmallEarthTech.AntRadioInterface;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.UnknownDevice
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
        public UnknownDevice(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        /// <inheritdoc/>
        public override void Parse(byte[] dataPage)
        {
            base.Parse(dataPage);

            DataPage page = DataPages.FirstOrDefault(p => p.PageNumber == dataPage[0]);
            if (page == null)
            {
                DataPages.Add(new DataPage(dataPage));
            }
            else { page.Update(dataPage); }
        }
    }

    /// <summary>
    /// This class encapsulates an unknown data page.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class DataPage : INotifyPropertyChanged
    {
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the data page received from the unknown sensor.
        /// </summary>
        /// <value>
        /// 8 byte array of the page received from the unknown sensor.
        /// </value>
        public byte[] Page { get; private set; }
        /// <summary>Gets the data page number.</summary>
        public byte PageNumber => Page[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPage"/> class.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public DataPage(byte[] dataPage)
        {
            Page = dataPage;
        }

        /// <summary>
        /// Updates the specified data page. The property changed notification is
        /// raised if the data page has changed.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
        public void Update(byte[] dataPage)
        {
            if (!Page.SequenceEqual(dataPage))
            {
                Page = dataPage;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Page)));
            }
        }
    }

    /// <summary>
    /// A thread safe collection of unknown data pages.
    /// </summary>
    public class UnknownDataPages : ObservableCollection<DataPage>
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
        protected override void InsertItem(int index, DataPage item)
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
        protected override void SetItem(int index, DataPage item)
        {
            lock (CollectionLock)
            {
                base.SetItem(index, item);
            }
        }
    }
}
