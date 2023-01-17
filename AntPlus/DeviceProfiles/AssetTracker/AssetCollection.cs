using System.Collections.ObjectModel;

namespace AntPlus.DeviceProfiles.AssetTracker
{
    /// <summary>
    /// A collection of assets.
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection&lt;AntPlus.DeviceProfiles.AssetTracker.Asset&gt;" />
    public class AssetCollection : ObservableCollection<Asset>
    {
        public object collectionLock = new object();

        protected override void ClearItems()
        {
            lock (collectionLock)
            {
                base.ClearItems();
            }
        }

        protected override void InsertItem(int index, Asset item)
        {
            lock (collectionLock)
            {
                base.InsertItem(index, item);
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            lock (collectionLock)
            {
                base.MoveItem(oldIndex, newIndex);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (collectionLock)
            {
                base.RemoveItem(index);
            }
        }

        protected override void SetItem(int index, Asset item)
        {
            lock (collectionLock)
            {
                base.SetItem(index, item);
            }
        }
    }
}
