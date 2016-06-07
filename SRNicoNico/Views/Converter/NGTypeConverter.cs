using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using SRNicoNico.Models.NicoNicoWrapper;
using System.ComponentModel;

namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(NGType), typeof(int))]
    public class NGTypeConverter : IValueConverter {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var v = (NGType) value;
            switch(v) {
                case NGType.Word:
                    return 0;
                case NGType.WordContains:
                    return 1;
                case NGType.UserId:
                    return 2;
                case NGType.RegEx:
                    return 3;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

            var v = (int)value;
            switch(v) {
                case 0:
                    return NGType.Word;
                case 1:
                    return NGType.WordContains;
                case 2:
                    return NGType.UserId;
                case 3:
                    return NGType.RegEx;
            }
            return NGType.Word;
        }
    }
}
