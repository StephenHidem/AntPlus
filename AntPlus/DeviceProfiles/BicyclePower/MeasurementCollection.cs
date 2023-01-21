using System.Collections.ObjectModel;
using System.Linq;

namespace SmallEarthTech.AntPlus.DeviceProfiles.BicyclePower
{
    /// <summary>
    /// A thread safe collection of measurments reported during calibration.
    /// </summary>
    public class MeasurementCollection : ObservableCollection<MeasurementOutputData>
    {
        /// <summary>
        /// The collection lock.
        /// </summary>
        /// <remarks>
        /// An application should use the collection lock to ensure thread safe access to the
        /// collection. For example, the code behind for a WPF window should include -
        /// <code>BindingOperations.EnableCollectionSynchronization(bicyclePowerViewModel.BicyclePower.Calibration.Measurements, bicyclePowerViewModel.BicyclePower.Calibration.Measurements.CollectionLock);</code>
        /// This ensures changes to the collection are thread safe and marshalled on the UI thread.
        /// </remarks>
        public object CollectionLock = new object();

        /// <inheritdoc/>
        protected override void InsertItem(int index, MeasurementOutputData item)
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
        protected override void SetItem(int index, MeasurementOutputData item)
        {
            lock (CollectionLock)
            {
                base.SetItem(index, item);
            }
        }

        /// <summary>
        /// Parses the specified data page.
        /// </summary>
        /// <param name="dataPage">The data page.</param>
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
