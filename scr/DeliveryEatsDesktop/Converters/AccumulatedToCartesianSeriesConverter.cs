using Avalonia.Data.Converters;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace desktop.Converters
{
    public class AccumulatedToCartesianSeriesConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            IEnumerable<float> productStatistics = value as IEnumerable<float>;
            if (productStatistics == null)
                return new LineSeries<float>() { Values = new float[] { 0 } };
            return new ISeries[]
            {
                new LineSeries<float>()
                {
                    Values = productStatistics,
                    Name = "" 
                }
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
