using System.Collections.ObjectModel;

namespace AntPlus.DeviceProfiles.AssetTracker
{
    /// <summary>
    /// A thread safe collection of assets.
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection&lt;AntPlus.DeviceProfiles.AssetTracker.Asset&gt;" />
    public class AssetCollection : ObservableCollection<Asset>
    {
        /// <summary>
        /// The collection lock.
        /// </summary>
        /// <remarks>
        /// An application should use the collection lock to ensure thread safe access to the
        /// collection. For example, the code behind for a WPF window should include -
        /// <code>BindingOperations.EnableCollectionSynchronization(assetTrack.Assets, assetTracker.Assets.collectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
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
