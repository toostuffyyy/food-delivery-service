using Avalonia.Data.Converters;
using desktop.Models;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Converters
{
    public class ProductStatisticToPieSeriesConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            IEnumerable<ProductStatistic> productStatistics = value as IEnumerable<ProductStatistic>;
            if(productStatistics == null)
                return new PieSeries<int>() { Values= new int[] {0} };
            return productStatistics.ToList().ConvertAll(p => new PieSeries<int>()
            {
                Values = new int[] {p.Count},
                Name = p.Product.Name
            });
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
