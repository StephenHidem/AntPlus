using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AntPlus.UnitTests
{
    [TestClass]
    public class SendMessageChannelTests
    {
        private readonly Mock<IAntChannel> _mockChannel;
        private readonly object _sendMessageChannel;

        public SendMessageChannelTests()
        {
            _mockChannel = new Mock<IAntChannel>();
            Type sendMessageChannelType = typeof(AntDeviceCollection).GetNestedType("SendMessageChannel", BindingFlags.NonPublic)!;
            _sendMessageChannel = Activator.CreateInstance(sendMessageChannelType, new[] { _mockChannel.Object, _mockChannel.Object, _mockChannel.Object }, Mock.Of<ILogger<AntDeviceCollection>>())!;
        }

        [TestMethod]
        public void AllMethods_ThrowNotImplementedException()
        {
            var methods = new (string MethodName, object[] Parameters)[]
            {
                ("AssignChannel", new object[] { ChannelType.BaseSlaveReceive, (byte)0, (uint)500 }),
                ("AssignChannelExt", new object[] { ChannelType.BaseSlaveReceive, (byte)0, ChannelTypeExtended.AdvFastStart, (uint)500 }),
                ("CloseChannel", new object[] { (uint)500 }),
                ("ConfigFrequencyAgility", new object[] { (byte)0, (byte)0, (byte)0, (uint)500 }),
                ("Dispose", new object[] { }),
                ("IncludeExcludeListAddChannel", new object[] { new ChannelId(0), (byte)0, (uint)500 }),
                ("IncludeExcludeListConfigure", new object[] { (byte)0, true, (uint)500 }),
                ("OpenChannel", new object[] { (uint)500 }),
                ("RequestChannelID", new object[] { (uint)500 }),
                ("RequestStatus", new object[] { (uint)500 }),
                ("SendAcknowledgedData", new object[] { new byte[0], (uint)500 }),
                ("SendAcknowledgedDataAsync", new object[] { new byte[0], (uint)500 }),
                ("SendBroadcastData", new object[] { new byte[0] }),
                ("SendBurstTransfer", new object[] { new byte[0], (uint)500 }),
                ("SendBurstTransferAsync", new object[] { new byte[0], (uint)500 }),
                ("SendExtAcknowledgedData", new object[] { new ChannelId(0), new byte[0], (uint)500 }),
                ("SendExtBroadcastData", new object[] { new ChannelId(0), new byte[0] }),
                ("SendExtBurstTransfer", new object[] { new ChannelId(0), new byte[0], (uint)500 }),
                ("SendExtBurstTransferAsync", new object[] { new ChannelId(0), new byte[0], (uint)500 }),
                ("SetChannelFreq", new object[] { (byte)0, (uint)500 }),
                ("SetChannelID", new object[] { new ChannelId(0), (uint)500 }),
                ("SetChannelID_UsingSerial", new object[] { new ChannelId(0), (uint)500 }),
                ("SetChannelPeriod", new object[] { (ushort)0, (uint)500 }),
                ("SetChannelSearchTimeout", new object[] { (byte)0, (uint)500 }),
                ("SetChannelTransmitPower", new object[] { TransmitPower.Tx0DB, (uint)500 }),
                ("SetLowPrioritySearchTimeout", new object[] { (byte)0, (uint)500 }),
                ("SetProximitySearch", new object[] { (byte)0, (uint)500 }),
                ("SetSearchThresholdRSSI", new object[] { (byte)0, (uint)500 }),
                ("UnassignChannel", new object[] { (uint)500 })
            };

            foreach (var method in methods)
            {
                var methodInfo = _sendMessageChannel.GetType().GetMethod(method.MethodName);
                Assert.IsNotNull(methodInfo);

                var exception = Assert.ThrowsException<TargetInvocationException>(() => methodInfo.Invoke(_sendMessageChannel, method.Parameters));
                Assert.IsInstanceOfType(exception.InnerException, typeof(NotImplementedException));
            }

            var propertyInfo = _sendMessageChannel.GetType().GetProperty("ChannelNumber");
            Assert.IsNotNull(propertyInfo);

            var propertyException = Assert.ThrowsException<TargetInvocationException>(() => propertyInfo.GetGetMethod()!.Invoke(_sendMessageChannel, null));
            Assert.IsInstanceOfType(propertyException.InnerException, typeof(NotImplementedException));
        }

        [TestMethod]
        public async Task SendExtAcknowledgedDataAsync_InvokesMethodMultipleTimesAndReturnsPass()
        {
            var channelId = new ChannelId(0);
            var data = new byte[0];
            var ackWaitTime = 500U;
            var messagingReturnCode = MessagingReturnCode.Pass;

            _mockChannel.Setup(c => c.SendExtAcknowledgedDataAsync(channelId, data, ackWaitTime))
                        .ReturnsAsync(messagingReturnCode);

            var methodInfo = _sendMessageChannel.GetType().GetMethod("SendExtAcknowledgedDataAsync");
            Assert.IsNotNull(methodInfo);

            // Act
            var tasks = new List<Task<MessagingReturnCode>>();
            for (int i = 0; i < 16; i++)
            {
                tasks.Add((Task<MessagingReturnCode>)methodInfo.Invoke(_sendMessageChannel, new object[] { channelId, data, ackWaitTime })!);
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            foreach (var result in results)
            {
                Assert.AreEqual(messagingReturnCode, result);
            }
            _mockChannel.Verify(c => c.SendExtAcknowledgedDataAsync(channelId, data, ackWaitTime), Times.Exactly(tasks.Count));
        }

    }
}
