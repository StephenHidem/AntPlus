using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace AntPlus.UnitTests
{
    public class SendMessageChannelTests
    {
        private readonly Mock<IAntChannel> _mockChannel;
        private readonly object _sendMessageChannel;

        public SendMessageChannelTests()
        {
            _mockChannel = new Mock<IAntChannel>();
            Type sendMessageChannelType = typeof(AntDeviceCollection).GetNestedType("SendMessageChannel", BindingFlags.NonPublic)!;
            _sendMessageChannel = Activator.CreateInstance(sendMessageChannelType,
                new[] { _mockChannel.Object },
                Mock.Of<ILogger<AntDeviceCollection>>())!;
        }

        [Fact]
        public void AllMethods_ThrowNotImplementedException()
        {
            var methods = new (string MethodName, object[] Parameters)[]
            {
                ("AssignChannel", new object[] { ChannelType.BaseSlaveReceive, (byte)0, (uint)500 }),
                ("AssignChannelExt", new object[] { ChannelType.BaseSlaveReceive, (byte)0, ChannelTypeExtended.AdvFastStart, (uint)500 }),
                ("CloseChannel", new object[] { (uint)500 }),
                ("ConfigFrequencyAgility", new object[] { (byte)0, (byte)0, (byte)0, (uint)500 }),
                ("Dispose", Array.Empty<object>()),
                ("IncludeExcludeListAddChannel", new object[] { new ChannelId(0), (byte)0, (uint)500 }),
                ("IncludeExcludeListConfigure", new object[] { (byte)0, true, (uint)500 }),
                ("OpenChannel", new object[] { (uint)500 }),
                ("RequestChannelID", new object[] { (uint)500 }),
                ("RequestStatus", new object[] { (uint)500 }),
                ("SendAcknowledgedData", new object[] { Array.Empty<byte>(), (uint)500 }),
                ("SendAcknowledgedDataAsync", new object[] { Array.Empty<byte>(), (uint)500 }),
                ("SendBroadcastData", new object[] { Array.Empty<byte>() }),
                ("SendBurstTransfer", new object[] { Array.Empty<byte>(), (uint)500 }),
                ("SendBurstTransferAsync", new object[] { Array.Empty<byte>(), (uint)500 }),
                ("SendExtAcknowledgedData", new object[] { new ChannelId(0), Array.Empty<byte>(), (uint)500 }),
                ("SendExtBroadcastData", new object[] { new ChannelId(0), Array.Empty<byte>() }),
                ("SendExtBurstTransfer", new object[] { new ChannelId(0), Array.Empty<byte>(), (uint)500 }),
                ("SendExtBurstTransferAsync", new object[] { new ChannelId(0), Array.Empty<byte>(), (uint)500 }),
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

            foreach (var (MethodName, Parameters) in methods)
            {
                var methodInfo = _sendMessageChannel.GetType().GetMethod(MethodName);
                Assert.NotNull(methodInfo);

                var exception = Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(_sendMessageChannel, Parameters));
                Assert.IsType<NotImplementedException>(exception.InnerException);
            }

            var propertyInfo = _sendMessageChannel.GetType().GetProperty("ChannelNumber");
            Assert.NotNull(propertyInfo);

            var propertyException = Assert.Throws<TargetInvocationException>(() => propertyInfo.GetGetMethod()!.Invoke(_sendMessageChannel, null));
            Assert.IsType<NotImplementedException>(propertyException.InnerException);
        }

        [Fact]
        public async Task SendExtAcknowledgedDataAsync_InvokesMethodMultipleTimesAndReturnsPass()
        {
            var channelId = new ChannelId(0);
            var data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var messagingReturnCode = MessagingReturnCode.Pass;

            _mockChannel.Setup(c => c.SendExtAcknowledgedDataAsync(channelId, data, It.IsAny<uint>()))
                        .ReturnsAsync(messagingReturnCode);
            _mockChannel.Setup(c => c.UnassignChannel(It.IsAny<uint>())).Returns(true);
            _mockChannel.Setup(c => c.AssignChannel(ChannelType.BaseSlaveReceive, 0, It.IsAny<uint>())).Returns(true);

            var methodInfo = _sendMessageChannel.GetType().GetMethod("SendExtAcknowledgedDataAsync");
            Assert.NotNull(methodInfo);

            // Act
            var tasks = new List<Task<MessagingReturnCode>>();
            for (int i = 0; i < 16; i++)
            {
                tasks.Add((Task<MessagingReturnCode>)methodInfo.Invoke(_sendMessageChannel, [channelId, data, It.IsAny<uint>()])!);
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            foreach (var result in results)
            {
                Assert.Equal(messagingReturnCode, result);
            }
            _mockChannel.Verify(c => c.SendExtAcknowledgedDataAsync(channelId, data, It.IsAny<uint>()), Times.Exactly(tasks.Count));
            _mockChannel.Verify(c => c.UnassignChannel(It.IsAny<uint>()), Times.Never);
            _mockChannel.Verify(c => c.AssignChannel(ChannelType.BaseSlaveReceive, 0, It.IsAny<uint>()), Times.Never);
        }

        [Fact]
        public async Task SendExtAcknowledgedDataAsync_InvokesMethodAndReturnsTimeout()
        {
            var channelId = new ChannelId(0);
            var data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var messagingReturnCode = MessagingReturnCode.Timeout;
            _mockChannel.Setup(c => c.SendExtAcknowledgedDataAsync(channelId, data, It.IsAny<uint>()))
                        .ReturnsAsync(messagingReturnCode);
            _mockChannel.Setup(c => c.UnassignChannel(It.IsAny<uint>())).Returns(true);
            _mockChannel.Setup(c => c.AssignChannel(ChannelType.BaseSlaveReceive, 0, It.IsAny<uint>())).Returns(true);

            var methodInfo = _sendMessageChannel.GetType().GetMethod("SendExtAcknowledgedDataAsync");
            Assert.NotNull(methodInfo);

            // Act
            var result = await (Task<MessagingReturnCode>)methodInfo.Invoke(_sendMessageChannel, [channelId, data, It.IsAny<uint>()])!;

            // Assert
            Assert.Equal(messagingReturnCode, result);
            _mockChannel.Verify(c => c.UnassignChannel(It.IsAny<uint>()), Times.Once);
            _mockChannel.Verify(c => c.AssignChannel(ChannelType.BaseSlaveReceive, 0, It.IsAny<uint>()), Times.Once);
        }
    }
}
