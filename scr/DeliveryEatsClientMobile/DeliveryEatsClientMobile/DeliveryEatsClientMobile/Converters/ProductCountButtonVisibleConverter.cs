using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DeliveryEatsClientMobile.Converters;

public class ProductCountButtonVisibleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var count = (int)value;
        return count != null ? count > 0 : null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}