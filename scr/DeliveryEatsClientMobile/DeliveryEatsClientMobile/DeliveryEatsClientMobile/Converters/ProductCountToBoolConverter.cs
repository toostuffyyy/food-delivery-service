using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DeliveryEatsClientMobile.Converters;

public class ProductCountToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if ((int)value == 0 && parameter == "addButton")
            return true;
        if ((int)value > 0 && parameter == "editButton")
            return true;
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}