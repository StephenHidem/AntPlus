using Microsoft.Extensions.Logging;
using System;

namespace SmallEarthTech.AntPlus.Extensions.Logging
{
    /// <summary>
    /// Provides extension methods for logging.
    /// </summary>
    public static partial class LoggerExtensions
    {
        private static Action<ILogger, string, Exception?>? s_unknownDataPage;

        /// <summary>
        /// Logs a warning message indicating an unknown data page.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="dataPage">The data page as a byte array.</param>
        /// <param name="eventId">The event ID for the log message.</param>
        public static void UnknownDataPage(this ILogger logger, byte[] dataPage, int eventId = 3000)
        {
            s_unknownDataPage = LoggerMessage.Define<string>(LogLevel.Warning, new(eventId, nameof(UnknownDataPage)), "Unknown data page. Page = {Page}");
            s_unknownDataPage(logger, BitConverter.ToString(dataPage), default);
        }
    }
}
