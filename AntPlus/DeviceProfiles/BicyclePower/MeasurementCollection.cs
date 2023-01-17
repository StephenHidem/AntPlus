using System.Collections.ObjectModel;
using System.Linq;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// A thread safe collection of measurments reported during calibration.
    /// </summary>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection&lt;AntPlus.DeviceProfiles.BicyclePower.MeasurementOutputData&gt;" />
    public class MeasurementCollection : ObservableCollection<MeasurementOutputData>
    {
        /// <summary>
        /// The collection lock.
        /// </summary>
        /// <remarks>
        /// An application should use the collection lock to ensure thread safe access to the
        /// collection. For example, the code behind for a WPF window should include -
        /// <code>BindingOperations.EnableCollectionSynchronization(bicyclePowerViewModel.BicyclePower.Calibration.Measurements, bicyclePowerViewModel.BicyclePower.Calibration.Measurements.collectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
        public object collectionLock = new object();

        protected override void InsertItem(int index, MeasurementOutputData item)
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

        protected override void SetItem(int index, MeasurementOutputData item)
        {
            lock (collectionLock)
            {
                base.SetItem(index, item);
            }
        }

        internal void Parse(byte[] dataPage)
        {
            MeasurementOutputData measurement = this.FirstOrDefault(m => m.MeasurementType == (MeasurementOutputData.DataType)dataPage[2]);
            if (measurement == null)
            {
                measurement = new MeasurementOutputData(dataPage);
                Add(measurement);
            }
            measurement.Parse(dataPage);
        }
    }
}
