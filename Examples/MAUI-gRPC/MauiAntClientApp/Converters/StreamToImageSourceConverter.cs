using SmallEarthTech.AntPlus;
using System.Globalization;

namespace MauiAntClientApp.Converters
{
    public class StreamToImageSourceConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is AntDevice device ? ImageSource.FromStream(() => device.DeviceImageStream) : (object?)null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
