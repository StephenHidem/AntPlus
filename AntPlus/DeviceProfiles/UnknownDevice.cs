using AntRadioInterface;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AntPlus.DeviceProfiles
{
    /// <summary>
    /// This class supports unknown devices.
    /// 
    /// © 2022 Stephen Hidem.
    /// </summary>
    /// <seealso cref="AntPlus.AntDevice" />
    public class UnknownDevice : AntDevice
    {
        /// <summary>
        /// Gets the data pages received from the unknown device.
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
        public event PropertyChangedEventHandler PropertyChanged;

        public byte[] Page { get; private set; }
        public byte PageNumber => Page[0];
        public DataPage(byte[] dataPage)
        {
            Page = dataPage;
        }
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
    /// A collection of unknown data pages.
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection&lt;AntPlus.DeviceProfiles.DataPage&gt;" />
    public class UnknownDataPages : ObservableCollection<DataPage>
    {
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
