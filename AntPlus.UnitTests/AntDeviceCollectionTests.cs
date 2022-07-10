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
            ChannelId hrmCid = new(0x01782211);
            ChannelId bpCid = new(0x050B4433);
            ChannelId unkCid = new(0x05016655);
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
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId.Id == hrmCid.Id), "Has HRM");
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId.Id == bpCid.Id), "Has Bike Power");
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId.Id == unkCid.Id), "Has Unknown");
        }

        [TestMethod]
        public void Collection_Clear_IsEmpty()
        {
            // Arrange
            var antDeviceCollection = new AntDeviceCollection();
            ChannelId hrmCid = new(0x01782211);
            ChannelId bpCid = new(0x050B4433);
            ChannelId unkCid = new(0x05016655);
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
            ChannelId hrmCid = new(0x01782211);
            ChannelId bpCid = new(0x050B4433);
            ChannelId unkCid = new(0x05016655);
            byte[] dataPage = { 0, 0, 0, 0, 0, 0, 0, 0 };
            antDeviceCollection.HandleDataMessage(hrmCid, dataPage);
            antDeviceCollection.HandleDataMessage(bpCid, dataPage);
            antDeviceCollection.HandleDataMessage(unkCid, dataPage);

            // Act
            bool result = antDeviceCollection.Remove(bp);

            // Assert
            Assert.IsTrue(result, "Remove Result");
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId.Id == hrmCid.Id), "Has HRM");
            Assert.IsFalse(antDeviceCollection.Any(d => d.ChannelId.Id == bpCid.Id), "Has Bike Power");
            Assert.IsTrue(antDeviceCollection.Any(d => d.ChannelId.Id == unkCid.Id), "Has Unknown");
        }
    }
}
