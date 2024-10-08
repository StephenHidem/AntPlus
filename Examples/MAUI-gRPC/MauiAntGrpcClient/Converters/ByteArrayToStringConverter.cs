﻿using System.Globalization;

namespace MauiAntGrpcClient.Converters
{
    public class ByteArrayToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is byte[] bytes)
            {
                return BitConverter.ToString(bytes);
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
