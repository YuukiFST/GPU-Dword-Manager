using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace GPU_Dword_Manager_Avalonia.Models
{
    public class BoolToColorConverter : IValueConverter
    {
        public static readonly BoolToColorConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool exists && exists)
            {
                return Colors.LightGreen;
            }
            return Colors.Red;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

