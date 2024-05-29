using System;
using System.Globalization;
using Avalonia.Data.Converters;
using desktop.Views;

namespace desktop.Converters;

public class ImageNameToFullNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string nameImage = value as string;
        if (nameImage != null)
            return SplashWindow.RestApiURL + "/Image/" + nameImage.Replace("/", "%2F");
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}