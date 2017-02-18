using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(bool), typeof(string))]
    public class SecondsToMSSConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var time = System.Convert.ToInt32(value);

            int munites = time / 60;
            int seconds = time % 60;

            if(seconds < 10) {

                return munites + ":0" + seconds;
            }

            return munites + ":" + seconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }
    }
}
