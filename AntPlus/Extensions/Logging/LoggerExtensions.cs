using Microsoft.Extensions.Logging;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Runtime.CompilerServices;

namespace SmallEarthTech.AntPlus.Extensions.Logging
{
    /// <summary>
    /// Provides extension methods for logging used in the AntPlus class library and the hosting extensions library.
    /// The design goal is to provide a consistent logging experience across the libraries and good performance.
    /// </summary>
    /// <remarks>
    /// The public methods in this class perform the required parameter conversions and call the private methods
    /// decorated with the <see cref="LoggerMessageAttribute"/> attribute.
    /// <para>
    /// LogUnknownDataPage and LogUnknownDataPage&lt;TEnum&gt; are used to log unknown data pages from an ANT device.
    /// Use LogUnknownDataPage when the data page value at page index 0 is not supported by the derived AntDevice parser.
    /// Use LogUnknownDataPage&lt;TEnum&gt; when a data page value at some other index is not supported by the derived
    /// AntDevice parser.
    /// </para>
    /// </remarks>
    public static partial class LoggerExtensions
    {
        /// <summary>
        /// Logs a warning message indicating an unknown data page.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="dataPage">The data page as a byte array.</param>
        /// <param name="methodName">Optional: The caller member name.</param>
        public static void LogUnknownDataPage(this ILogger logger, byte[] dataPage, [CallerMemberName] string methodName = "")
        {
            s_unknownDataPage(logger, methodName, dataPage[0], BitConverter.ToString(dataPage));
        }

        [LoggerMessage(EventId = 1000, Level = LogLevel.Warning, Message = "{MethodName}: Unknown data page# 0x{Page:X2}, Data page = {DataPage}")]
        private static partial void s_unknownDataPage(ILogger logger, string methodName, byte page, string dataPage);

        /// <summary>
        /// Logs a warning message indicating an unknown data page deeper in the device parsing hierarchy.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration that was being parsed.</typeparam>
        /// <param name="logger">The logger instance.</param>
        /// <param name="value">The value in the data page that is not defined in the enumeration.</param>
        /// <param name="dataPage">The data page as a byte array.</param>
        /// <param name="methodName">Optional: The caller member name.</param>
        public static void LogUnknownDataPage<TEnum>(this ILogger logger, byte value, byte[] dataPage, [CallerMemberName] string methodName = "") where TEnum : Enum
        {
            s_unknownDataPageEnum(logger, methodName, dataPage[0], value, typeof(TEnum).Name, BitConverter.ToString(dataPage));
        }

        [LoggerMessage(EventId = 1001, Level = LogLevel.Warning, Message = "{MethodName}: Unknown data page# 0x{Page:X2}, 0x{Value:X2} is not defined in {Enum}, Data page = {DataPage}")]
        private static partial void s_unknownDataPageEnum(ILogger logger, string methodName, byte page, byte value, string @enum, string dataPage);

        /// <summary>
        /// Logs a debug message about sending an acknowledged message.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="channelIndex">The channel index.</param>
        /// <param name="channelId">The channel ID of the ANT device.</param>
        /// <param name="dataPage">The data page as a byte array.</param>
        /// <param name="result">A <see cref="MessagingReturnCode"/>. Set to null if not available.</param>
        /// <param name="methodName">Optional: The caller member name.</param>
        public static void LogSendAcknowledgedMessage(this ILogger logger, int channelIndex, uint channelId, byte[] dataPage, MessagingReturnCode? result, [CallerMemberName] string methodName = "")
        {
            if (result.HasValue)
            {
                s_sendAckMsgResult(logger, methodName, channelIndex, channelId, dataPage[0], result.Value);
            }
            else
            {
                s_sendAckMsg(logger, methodName, channelIndex, channelId, dataPage[0], BitConverter.ToString(dataPage));
            }
        }

        [LoggerMessage(EventId = 1002, Level = LogLevel.Debug, Message = "{MethodName}: Channel index = {ChannelIndex}, Channel ID = 0x{ChannelId:X8}, Page# 0x{Page:X2}, Data page = {DataPage}")]
        private static partial void s_sendAckMsg(ILogger logger, string methodName, int channelIndex, uint channelId, byte page, string dataPage);

        [LoggerMessage(EventId = 1003, Level = LogLevel.Debug, Message = "{MethodName}: Channel index = {ChannelIndex}, Channel ID = 0x{ChannelId:X8}, Page# 0x{Page:X2}, Result = {Result}")]
        private static partial void s_sendAckMsgResult(ILogger logger, string methodName, int channelIndex, uint channelId, byte page, MessagingReturnCode result);

        /// <summary>
        /// Logs a method entry. The log level is Debug.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogMethodEntry(this ILogger logger, [CallerMemberName] string methodName = "")
        {
            s_logMethodEntry(logger, methodName);
        }

        [LoggerMessage(EventId = 1004, Level = LogLevel.Debug, Message = "Entering {MethodName}")]
        private static partial void s_logMethodEntry(ILogger logger, string methodName);

        /// <summary>
        /// Logs the state of an ANT device. The log level is Debug.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="antDevice">The ANT device instance.</param>
        /// <param name="timeout">The timeout value in milliseconds.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogAntDeviceState(this ILogger logger, AntDevice antDevice, int timeout, [CallerMemberName] string methodName = "")
        {
            s_logAntDeviceState(logger, methodName, antDevice, antDevice.ChannelId.DeviceNumber, antDevice.Offline, timeout);
        }

        [LoggerMessage(EventId = 1005, Level = LogLevel.Debug, Message = "{MethodName} {AntDevice} Device# {DeviceNumber}, Offline = {Offline}, Timeout = {Timeout}ms")]
        private static partial void s_logAntDeviceState(ILogger logger, string methodName, AntDevice antDevice, uint deviceNumber, bool offline, int timeout);

        /// <summary>
        /// Logs a change in the ANT device collection. The log level is Debug.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="antDevice">The ANT device instance.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogAntCollectionChange(this ILogger logger, AntDevice antDevice, [CallerMemberName] string methodName = "")
        {
            s_logAntCollectionChange(logger, methodName, antDevice, antDevice.ChannelId.DeviceNumber);
        }

        [LoggerMessage(EventId = 1006, Level = LogLevel.Debug, Message = "{MethodName}: {AntDevice} Device# {DeviceNumber}")]
        private static partial void s_logAntCollectionChange(ILogger logger, string methodName, AntDevice antDevice, uint deviceNumber);

        /// <summary>
        /// Logs an ANT response. Null payloads are logged as "Null".
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="level">Log level.</param>
        /// <param name="antResponse">The AntResponse received.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogAntResponse(this ILogger logger, LogLevel level, AntResponse antResponse, [CallerMemberName] string methodName = "")
        {
            string payload = antResponse.Payload != null ? BitConverter.ToString(antResponse.Payload) : "Null";
            s_logAntResponse(logger, level, methodName, antResponse.ChannelNumber, antResponse.ResponseId, payload);
        }

        [LoggerMessage(EventId = 1007, Message = "{MethodName}: Channel# {Channel}, Response ID = {ResponseId}, Payload = {Payload}")]
        private static partial void s_logAntResponse(ILogger logger, LogLevel level, string methodName, byte channel, MessageId responseId, string payload);

        /// <summary>
        /// Logs an unhandled ANT response. Null payloads are logged as "Null".
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="antResponse">The AntResponse received.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogUnhandledAntResponse(this ILogger logger, AntResponse antResponse, [CallerMemberName] string methodName = "")
        {
            string payload = antResponse.Payload != null ? BitConverter.ToString(antResponse.Payload) : "Null";
            s_logUnhandledAntResponse(logger, methodName, antResponse.ChannelNumber, antResponse.ResponseId, payload);
        }

        [LoggerMessage(EventId = 1011, Level = LogLevel.Warning, Message = "{MethodName}: Unhandled ANT response. Channel# {Channel}, Response ID = {ResponseId}, Payload = {Payload}")]
        private static partial void s_logUnhandledAntResponse(ILogger logger, string methodName, byte channel, MessageId responseId, string payload);

        /// <summary>
        /// Logs a warning message indicating an ignored or unexpected page.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration</typeparam>
        /// <param name="logger">The logger instance.</param>
        /// <param name="dataPage">The data page as a byte array.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogIgnoredDataPage<TEnum>(this ILogger logger, byte[] dataPage, [CallerMemberName] string methodName = "") where TEnum : Enum
        {
            s_logIgnoredPage(logger, methodName, Enum.GetName(typeof(TEnum), dataPage[0]), BitConverter.ToString(dataPage));
        }

        [LoggerMessage(EventId = 1008, Level = LogLevel.Warning, Message = "{MethodName}: Ignoring page type {Page}, Data page = {DataPage}. The page was unexpected or not implemented.")]
        private static partial void s_logIgnoredPage(ILogger logger, string methodName, string? page, string dataPage);

        /// <summary>
        /// Logs a data page.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="level">Log level.</param>
        /// <param name="dataPage">The data page as a byte array.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogDataPage(this ILogger logger, LogLevel level, byte[] dataPage, [CallerMemberName] string methodName = "")
        {
            s_logDataPage(logger, level, methodName, BitConverter.ToString(dataPage));
        }

        [LoggerMessage(EventId = 1009, Message = "{MethodName}: Data page = {DataPage}")]
        private static partial void s_logDataPage(ILogger logger, LogLevel level, string methodName, string dataPage);

        /// <summary>
        /// Logs a data page.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration</typeparam>
        /// <param name="logger">The logger instance.</param>
        /// <param name="level">Log level.</param>
        /// <param name="value">Value of the enumeration.</param>
        /// <param name="dataPage">The data page as a byte array.</param>
        /// <param name="methodName">The caller member name.</param>
        public static void LogDataPage<TEnum>(this ILogger logger, LogLevel level, byte value, byte[] dataPage, [CallerMemberName] string methodName = "") where TEnum : Enum
        {
            s_logDataPage(logger, level, methodName, Enum.GetName(typeof(TEnum), value), BitConverter.ToString(dataPage));
        }

        [LoggerMessage(EventId = 1010, Message = "{MethodName}: Data page type {Page}, Data page = {DataPage}")]
        private static partial void s_logDataPage(ILogger logger, LogLevel level, string methodName, string? page, string dataPage);
    }
}
