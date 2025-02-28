using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Runtime.CompilerServices;

namespace SmallEarthTech.AntPlus.Extensions.Logging
{
    /// <summary>
    /// Provides extension methods for logging.
    /// </summary>
    /// <remarks>
    /// LogTraceDataPage is used to log a trace message indicating a data page.
    /// LogDebugDataPage is used to log a debug message indicating a data page.
    /// LogInfoDataPage is used to log an information message indicating a data page.
    /// LogUnknownDataPage is used to log a warning message indicating an unknown data page.
    /// LogErrorDataPage is used to log an error message indicating a data page.
    /// LogCriticalDataPage is used to log a critical message indicating a data page.
    /// 
    /// Event IDs follow the log level and the names match the log level.
    /// </remarks>
    public static class LoggerExtensions
    {
        private static readonly Action<ILogger, byte, string, Exception?> s_unknownDataPage =
            LoggerMessage.Define<byte, string>(
                LogLevel.Warning,
                new(3000, nameof(LogUnknownDataPage)),
                "Unknown data page# 0x{Page:X2}, Data page = {DataPage}");

        private static readonly Action<ILogger, byte, byte, string, string, Exception?> s_unknownDataPageEnum =
            LoggerMessage.Define<byte, byte, string, string>(
                LogLevel.Warning,
                new(3001, nameof(LogUnknownDataPage)),
                "Unknown data page# 0x{Page:X2}, 0x{Value:X2} is not defined in {Enum}, Data page = {DataPage}");

        private static readonly Action<ILogger, int, uint, byte, string, Exception?> s_sendAckMsg =
            LoggerMessage.Define<int, uint, byte, string>(
                LogLevel.Debug,
                new(1000, nameof(LogSendAcknowledgedMessage)),
                "SendExtAcknowledgedDataAsync: Channel index = {ChannelIndex}, Channel ID = 0x{ChannelId:X8}, Page# 0x{Page:X2}, Data page = {DataPage}");

        private static readonly Action<ILogger, int, uint, byte, MessagingReturnCode, Exception?> s_sendAckMsgResult =
            LoggerMessage.Define<int, uint, byte, MessagingReturnCode>(
                LogLevel.Debug,
                new(1001, nameof(LogSendAcknowledgedMessage)),
                "SendExtAcknowledgedDataAsync: Channel index = {ChannelIndex}, Channel ID = 0x{ChannelId:X8}, Page# 0x{Page:X2}, Result = {Result}");

        private static readonly Action<ILogger, string, Exception?> s_logMethodEntry =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new(1002, nameof(LogMethodEntry)),
                "Entering {MethodName}");

        private static readonly Action<ILogger, string, string, uint, bool, int, Exception?> s_logAntDeviceState =
            LoggerMessage.Define<string, string, uint, bool, int>(
                LogLevel.Debug,
                new(1003, nameof(LogAntDeviceState)),
                "{MethodName} {AntDevice} Device# {DeviceNumber}, Offline = {Offline}, Timeout = {Timeout}ms");

        private static readonly Action<ILogger, string, string, uint, Exception?> s_logAntCollectionChange =
            LoggerMessage.Define<string, string, uint>(
                LogLevel.Debug,
                new(1004, nameof(LogAntCollectionChange)),
                "{MethodName} {AntDevice}, Device# {DeviceNumber}");

        /// <summary>
        /// Logs a warning message indicating an unknown data page.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="dataPage">The data page as a byte array.</param>
        public static void LogUnknownDataPage(this ILogger logger, byte[] dataPage)
        {
            s_unknownDataPage(logger, dataPage[0], BitConverter.ToString(dataPage), default);
        }

        /// <summary>
        /// Logs a warning message indicating an unknown data page deeper in the device parsing hierarchy.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration that was being parsed.</typeparam>
        /// <param name="logger">The logger instance.</param>
        /// <param name="value">The value in the data page that is not defined in the enumeration.</param>
        /// <param name="dataPage">The data page as a byte array.</param>
        public static void LogUnknownDataPage<TEnum>(this ILogger logger, byte value, byte[] dataPage) where TEnum : Enum
        {
            s_unknownDataPageEnum(logger, dataPage[0], value, typeof(TEnum).Name, BitConverter.ToString(dataPage), default);
        }

        /// <summary>
        /// Logs a debug message about sending an acknowledged message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="channelIndex">The channel index.</param>
        /// <param name="channelId">The channel ID of the ANT device.</param>
        /// <param name="result">A <see cref="MessagingReturnCode"/>. Set to null if not available.</param>
        public static void LogSendAcknowledgedMessage(this ILogger logger, int channelIndex, uint channelId, byte[] dataPage, MessagingReturnCode? result)
        {
            if (result.HasValue)
            {
                s_sendAckMsgResult(logger, channelIndex, channelId, dataPage[0], result.Value, default);
            }
            else
            {
                s_sendAckMsg(logger, channelIndex, channelId, dataPage[0], BitConverter.ToString(dataPage), default);
            }
        }

        /// <summary>
        /// Logs a method entry. The log level is Debug.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogMethodEntry(this ILogger logger, [CallerMemberName] string methodName = "")
        {
            s_logMethodEntry(logger, methodName, default);
        }

        /// <summary>
        /// Logs the state of an ANT device. The log level is Debug.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="antDevice">The ANT device instance.</param>
        /// <param name="timeout">The timeout value in milliseconds.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogAntDeviceState(this ILogger logger, AntDevice antDevice, int timeout, [CallerMemberName] string methodName = "")
        {
            s_logAntDeviceState(logger, methodName, antDevice.ToString(), antDevice.ChannelId.DeviceNumber, antDevice.Offline, timeout, default);
        }

        /// <summary>
        /// Logs a change in the ANT device collection. The log level is Debug.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="antDevice">The ANT device instance.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogAntCollectionChange(this ILogger logger, AntDevice antDevice, [CallerMemberName] string methodName = "")
        {
            s_logAntCollectionChange(logger, methodName, antDevice.ToString(), antDevice.ChannelId.DeviceNumber, default);
        }
    }
}
