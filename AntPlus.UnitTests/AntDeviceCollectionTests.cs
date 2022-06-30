using DeviceProfiles;
using System.Linq;

namespace AntPlus.UnitTests
{
    [TestClass]
    public class AntDeviceCollectionTests
    {
        [TestMethod]
        public void HandleDataMessage_CollectionContainsDevices_ExpectedBehavior()
        {
            // Arrange
            var antDeviceCollection = new AntDeviceCollection();
            uint hrmCid = 0x01782211;
            uint bpCid = 0x050B4433;
            uint unkCid = 0x05016655;
            byte[] dataPage = { 0, 0, 0, 0, 0, 0, 0, 0 };

            // Act
            antDeviceCollection.HandleDataMessage(hrmCid, dataPage);
            antDeviceCollection.HandleDataMessage(bpCid, dataPage);
            antDeviceCollection.HandleDataMessage(unkCid, dataPage);
            antDeviceCollection.HandleDataMessage(hrmCid, dataPage);
            antDeviceCollection.HandleDataMessage(bpCid, dataPage);
            antDeviceCollection.HandleDataMessage(unkCid, dataPage);

            // Assert
            Assert.AreEqual(3, antDeviceCollection.Count, "Count");
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId == hrmCid), "Has HRM");
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId == bpCid), "Has Bike Power");
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId == unkCid), "Has Unknown");
        }

        [TestMethod]
        public void Collection_Clear_IsEmpty()
        {
            // Arrange
            var antDeviceCollection = new AntDeviceCollection();
            uint hrmCid = 0x01782211;
            uint bpCid = 0x050B4433;
            uint unkCid = 0x05016655;
            byte[] dataPage = { 0, 0, 0, 0, 0, 0, 0, 0 };
            antDeviceCollection.HandleDataMessage(hrmCid, dataPage);
            antDeviceCollection.HandleDataMessage(bpCid, dataPage);
            antDeviceCollection.HandleDataMessage(unkCid, dataPage);

            // Act
            antDeviceCollection.Clear();

            // Assert
            Assert.AreEqual(0, antDeviceCollection.Count, "Count");
        }

        [TestMethod]
        public void Collection_RemoveItem_DoesNotContainItem()
        {
            // Arrange
            var antDeviceCollection = new AntDeviceCollection();
            BicyclePower? bp = null;
            antDeviceCollection.CollectionChanged += (sender, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    if (e.NewItems[0] is BicyclePower power)
                    {
                        bp = power;
                    }
                }
            };
            uint hrmCid = 0x01782211;
            uint bpCid = 0x050B4433;
            uint unkCid = 0x05016655;
            byte[] dataPage = { 0, 0, 0, 0, 0, 0, 0, 0 };
            antDeviceCollection.HandleDataMessage(hrmCid, dataPage);
            antDeviceCollection.HandleDataMessage(bpCid, dataPage);
            antDeviceCollection.HandleDataMessage(unkCid, dataPage);

            // Act
            bool result = antDeviceCollection.Remove(bp);

            // Assert
            Assert.IsTrue(result, "Remove Result");
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId == hrmCid), "Has HRM");
            Assert.IsFalse(antDeviceCollection.Any(d => d.ChannelId == bpCid), "Has Bike Power");
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId == unkCid), "Has Unknown");
        }
    }
}
