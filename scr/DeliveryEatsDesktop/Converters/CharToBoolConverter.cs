﻿using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Converters
{
    internal class CharToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            char c = (char)value;
            if (c == '\0') return false;
            else return true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;
            if (isChecked)
                return '⋅';
            else return '\0';
        }
    }
}
