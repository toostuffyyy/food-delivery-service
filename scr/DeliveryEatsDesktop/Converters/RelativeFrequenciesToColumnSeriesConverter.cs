using Avalonia.Data.Converters;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Converters
{
    public class RelativeFrequenciesToColumnSeriesConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            IEnumerable<(string, float)> productStatistics = value as IEnumerable<(string, float)>;
            if(productStatistics == null)
                return new ColumnSeries<float>() { Values = new float[] {0} };
            return productStatistics.ToList().ConvertAll(p => new ColumnSeries<float>()
            {
                Values = new float[] {p.Item2},
                Name = p.Item1
            });
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
