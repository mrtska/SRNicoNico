using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(double), typeof(double))]
    public class InvertDoubleConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var doubleValue = (double)value;
            return -(doubleValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var doubleValue = (double)value;
            return doubleValue;
        }
    }
}
