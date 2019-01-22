using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(int), typeof(DateTime))]
    public class VposToDateTimeConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            return NicoNicoUtil.ConvertTime(System.Convert.ToInt32(value) / 100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return 0L;
        }
    }
}
