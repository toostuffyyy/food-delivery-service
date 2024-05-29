using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DeliveryEatsClientMobile.Converters;

public class ExpirationDateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var days = (int)value;
        return days switch
        {
            <= 0 => "0 дн.",
            < 30 => $"{days:0} дн.",
            < 365 => $"{days / 30:0} мес.",
            _ => $"{days / 365:0} год."
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}