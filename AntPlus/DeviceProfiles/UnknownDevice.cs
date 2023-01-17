using AntRadioInterface;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AntPlus.DeviceProfiles
{
    /// <summary>
    /// This class supports unknown devices.
    /// </summary>
    /// <seealso cref="AntPlus.AntDevice" />
    public class UnknownDevice : AntDevice
    {
        /// <summary>
        /// Gets the collection of data pages received from the unknown device.
        /// </summary>
        /// <value>
        /// The data pages.
        /// </value>
        public UnknownDataPages DataPages { get; private set; } = new UnknownDataPages();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownDevice"/> class.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="antChannel">Channel to send messages to.</param>
        public UnknownDevice(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        /// <summary>
        /// Parses/updates the specified data page.
        /// </summary>
        /// <param name="dataPage"></param>
        public override void Parse(byte[] dataPage)
        {
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
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class DataPage : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the data page.
        /// </summary>
        /// <value>
        /// 8 byte array of the page.
        /// </value>
        public byte[] Page { get; private set; }
        /// <summary>
        /// Gets the data page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
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
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection&lt;AntPlus.DeviceProfiles.DataPage&gt;" />
    public class UnknownDataPages : ObservableCollection<DataPage>
    {
        /// <summary>
        /// The collection lock.
        /// </summary>
        /// <remarks>
        /// An application should use the collection lock to ensure thread safe access to the
        /// collection. For example, the code behind for a WPF window should include -
        /// <code>BindingOperations.EnableCollectionSynchronization(unknownDevice.DataPages, unknownDevice.DataPages.collectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
        public object collectionLock = new object();

        protected override void InsertItem(int index, DataPage item)
        {
            lock (collectionLock)
            {
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (collectionLock)
            {
                base.RemoveItem(index);
            }
        }

        protected override void ClearItems()
        {
            lock (collectionLock)
            {
                base.ClearItems();
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            lock (collectionLock)
            {
                base.MoveItem(oldIndex, newIndex);
            }
        }

        protected override void SetItem(int index, DataPage item)
        {
            lock (collectionLock)
            {
                base.SetItem(index, item);
            }
        }
    }
}
