using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SRNicoNico.Models.NicoNicoViewer {

    [ValueConversion(typeof(string), typeof(int))]
    public class VerifyEmptyText : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var obj = value as string;

            return obj.Trim().Length;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
