using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace DeliveryEatsClientMobile.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var IsChecked = (bool)value;
        return IsChecked ? new SolidColorBrush(Color.Parse("#e6e6e6")) : Brushes.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}