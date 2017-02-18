using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using SRNicoNico.Models.NicoNicoViewer;
namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(double), typeof(double))]
    public class SliderValueRoundingConverter : IValueConverter {




        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var round = System.Convert.ToDouble(parameter);

            var dou = System.Convert.ToDouble(value);

            if(dou % round > 0.175) {

                dou += dou % round;
            } else {

                dou -= dou % round;
            }

            return dou;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return System.Convert.ToDouble(value);
        }
    }
}
