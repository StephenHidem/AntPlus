using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfUsbStickApp.Converters
{
    /// <summary>
    /// Converts between decimal degrees and semicircles.
    /// </summary>
    /// <remarks>
    /// Decimal degrees range from -90.0 to +90.0. West and south are negative, east and north are positive.
    /// </remarks>
    /// <seealso cref="IValueConverter" />
    internal class DegreesToSemicirclesConverter : IValueConverter
    {
        /// <summary>
        /// Converts semicircles to degrees. The expected value is a signed integer.
        /// </summary>
        /// <remarks>
        /// The conversion formula is degrees = 180 * semicircles / 2^31.
        /// </remarks>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// Decimal degrees as a double. If the method returns <see langword="null" />, the valid null value is used.
        /// </returns>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int val)
            {
                return 180.0 * val / Math.Pow(2, 31);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts degrees to semicircles.
        /// </summary>
        /// The conversion formula is semicircles = degrees * 2^31 / 180.
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// Semicircles as an integer. If the method returns <see langword="null" />, the valid null value is used.
        /// </returns>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string val)
            {
                return (int)(double.Parse(val) * Math.Pow(2, 31) / 180.0);
            }
            else
            {
                return null;
            }
        }
    }
}
