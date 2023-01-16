using AntRadioInterface;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AntPlus.DeviceProfiles
{
    public class UnknownDevice : AntDevice
    {
        public UnknownDataPages DataPages { get; private set; } = new UnknownDataPages();

        public UnknownDevice(ChannelId channelId, IAntChannel antChannel) : base(channelId, antChannel)
        {
        }

        public override void Parse(byte[] dataPage)
        {
            DataPage page = DataPages.FirstOrDefault(p => p.PageNumber == dataPage[0]);
            if (page == null)
            {
                DataPages.Add(new DataPage(dataPage));
            }
            else { page.Update(dataPage); }
        }

        public override void ChannelEventHandler(EventMsgId eventMsgId)
        {
            throw new NotImplementedException();
        }

        public override void ChannelResponseHandler(byte messageId, ResponseMsgId responseMsgId)
        {
            throw new NotImplementedException();
        }
    }

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
