using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using SRNicoNico.Models.NicoNicoViewer;
namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(object), typeof(string))]
    public class ObjectToStringConverter : IValueConverter {




        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            return System.Convert.ToString(value) + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
