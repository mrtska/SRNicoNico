using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using SRNicoNico.Models.NicoNicoViewer;
namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(int), typeof(DateTime))]
    public class VposToDateTimeConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            return DateTime.Today.Add(TimeSpan.FromMilliseconds(System.Convert.ToInt32(value) * 10));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return 0L;
        }
    }
}
