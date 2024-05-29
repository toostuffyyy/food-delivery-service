using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DeliveryEatsClientMobile.Converters;

public class ProductEnabledConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values[0] is int count && values[1] is int avarible)
            return count < avarible;
        return false;
    }
}