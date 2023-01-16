using System;
using System.Globalization;
using System.Windows.Data;

namespace AntPlusUsbClient.Converters
{
    internal class ByteArrayToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                byte[] bytes = (byte[])value;
                return BitConverter.ToString(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
