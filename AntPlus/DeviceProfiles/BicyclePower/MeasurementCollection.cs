using System.Collections.ObjectModel;
using System.Linq;

namespace AntPlus.DeviceProfiles.BicyclePower
{
    public class MeasurementCollection : ObservableCollection<MeasurementOutputData>
    {
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
