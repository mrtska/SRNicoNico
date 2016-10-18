using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(int), typeof(string))]
    public class TimeStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            return NicoNicoUtil.ConvertTime(System.Convert.ToInt32(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {

            return NicoNicoUtil.ConvertTime((string)value);
        }
    }
}
