using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using SRNicoNico.Models.NicoNicoViewer;
namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(long), typeof(DateTime))]
    public class UnixTimeToDateTimeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            return UnixTime.FromUnixTime(System.Convert.ToInt64(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return 0L;
        }
    }
}
