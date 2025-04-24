using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ProbSciANA.Interface
{
    public class FirstNonNullConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => values.FirstOrDefault(v => v != null);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}