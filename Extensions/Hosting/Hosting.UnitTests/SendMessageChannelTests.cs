using Microsoft.Extensions.Logging;
using Moq;
using SmallEarthTech.AntPlus.Extensions.Hosting;
using SmallEarthTech.AntRadioInterface;
using System.Diagnostics;
using System.Reflection;

namespace Hosting.UnitTests
{
    public class SendMessageChannelTests
    {
        private readonly Mock<IAntChannel> _mockChannel;
        private readonly object _sendMessageChannel;

        public class DebugLogger<T> : ILogger<T>
        {
            IDisposable? ILogger.BeginScope<TState>(TState state) => null;
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                Debug.WriteLine(formatter(state, exception));
            }
        }

        public SendMessageChannelTests()
        {
            _mockChannel = new Mock<IAntChannel>();
            Type sendMessageChannelType = typeof(AntCollection).GetNestedType("SendMessageChannel", BindingFlags.NonPublic)!;
            _sendMessageChannel = Activator.CreateInstance(sendMessageChannelType, new[] { _mockChannel.Object, _mockChannel.Object, _mockChannel.Object }, new DebugLogger<AntCollection>())!;
        }

        [Fact]
        public void ChannelNumber_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetProperty("ChannelNumber")!
                .GetGetMethod()!
                .Invoke(_sendMessageChannel, null)
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void AssignChannel_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("AssignChannel")!
                .Invoke(_sendMessageChannel, [ChannelType.BaseSlaveReceive, (byte)0, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void AssignChannelExt_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("AssignChannelExt")!
                .Invoke(_sendMessageChannel, [ChannelType.BaseSlaveReceive, (byte)0, ChannelTypeExtended.AdvFastStart, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void CloseChannel_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("CloseChannel")!
                .Invoke(_sendMessageChannel, [(uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void ConfigFrequencyAgility_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("ConfigFrequencyAgility")!
                .Invoke(_sendMessageChannel, [(byte)0, (byte)0, (byte)0, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void Dispose_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("Dispose")!
                .Invoke(_sendMessageChannel, null)
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void IncludeExcludeListAddChannel_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("IncludeExcludeListAddChannel")!
                .Invoke(_sendMessageChannel, [new ChannelId(0), (byte)0, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void IncludeExcludeListConfigure_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("IncludeExcludeListConfigure")!
                .Invoke(_sendMessageChannel, [(byte)0, true, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void OpenChannel_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("OpenChannel")!
                .Invoke(_sendMessageChannel, [(uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void RequestChannelID_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("RequestChannelID")!
                .Invoke(_sendMessageChannel, [(uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void RequestStatus_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("RequestStatus")!
                .Invoke(_sendMessageChannel, [(uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SendAcknowledgedData_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SendAcknowledgedData")!
                .Invoke(_sendMessageChannel, [new byte[0], (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SendAcknowledgedDataAsync_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SendAcknowledgedDataAsync")!
                .Invoke(_sendMessageChannel, [new byte[0], (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SendBroadcastData_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SendBroadcastData")!
                .Invoke(_sendMessageChannel, [new byte[0]])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SendBurstTransfer_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SendBurstTransfer")!
                .Invoke(_sendMessageChannel, [new byte[0], (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SendBurstTransferAsync_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SendBurstTransferAsync")!
                .Invoke(_sendMessageChannel, [new byte[0], (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SendExtAcknowledgedData_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SendExtAcknowledgedData")!
                .Invoke(_sendMessageChannel, [new ChannelId(0), new byte[0], (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public async Task SendExtAcknowledgedDataAsync_InvokesMethodMultipleTimesAndReturnsPass()
        {
            var channelId = new ChannelId(0);
            var data = new byte[0];
            var ackWaitTime = 500U;
            var messagingReturnCode = MessagingReturnCode.Pass;

            _mockChannel.Setup(c => c.SendExtAcknowledgedDataAsync(channelId, data, ackWaitTime))
                        .ReturnsAsync(messagingReturnCode);

            var methodInfo = _sendMessageChannel.GetType().GetMethod("SendExtAcknowledgedDataAsync");
            Assert.NotNull(methodInfo);

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
                Assert.Equal(messagingReturnCode, result);
            }
            _mockChannel.Verify(c => c.SendExtAcknowledgedDataAsync(channelId, data, ackWaitTime), Times.Exactly(tasks.Count));
        }

        [Fact]
        public void SendExtBroadcastData_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SendExtBroadcastData")!
                .Invoke(_sendMessageChannel, [new ChannelId(0), new byte[0]])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SendExtBurstTransfer_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SendExtBurstTransfer")!
                .Invoke(_sendMessageChannel, [new ChannelId(0), new byte[0], (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SendExtBurstTransferAsync_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SendExtBurstTransferAsync")!
                .Invoke(_sendMessageChannel, [new ChannelId(0), new byte[0], (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SetChannelFreq_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SetChannelFreq")!
                .Invoke(_sendMessageChannel, [(byte)0, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SetChannelID_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SetChannelID")!
                .Invoke(_sendMessageChannel, [new ChannelId(0), (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SetChannelID_UsingSerial_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SetChannelID_UsingSerial")!
                .Invoke(_sendMessageChannel, [new ChannelId(0), (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SetChannelPeriod_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SetChannelPeriod")!
                .Invoke(_sendMessageChannel, [(ushort)0, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SetChannelSearchTimeout_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SetChannelSearchTimeout")!
                .Invoke(_sendMessageChannel, [(byte)0, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SetChannelTransmitPower_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SetChannelTransmitPower")!
                .Invoke(_sendMessageChannel, [TransmitPower.Tx0DB, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SetLowPrioritySearchTimeout_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SetLowPrioritySearchTimeout")!
                .Invoke(_sendMessageChannel, [(byte)0, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SetProximitySearch_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SetProximitySearch")!
                .Invoke(_sendMessageChannel, [(byte)0, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void SetSearchThresholdRSSI_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("SetSearchThresholdRSSI")!
                .Invoke(_sendMessageChannel, [(byte)0, (uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }

        [Fact]
        public void UnassignChannel_ThrowsNotImplementedException()
        {
            var exception = Assert.Throws<TargetInvocationException>(() =>
                _sendMessageChannel.GetType()
                .GetMethod("UnassignChannel")!
                .Invoke(_sendMessageChannel, [(uint)500])
            );

            Assert.IsType<NotImplementedException>(exception.InnerException);
        }
    }
}
