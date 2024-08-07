using System;
using System.Globalization;
using System.Windows.Data;

namespace SRNicoNico.Views.Converters {
    /// <summary>
    /// 小数点型の数値を%表記に変換する
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToPacentageConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            return $"{Math.Floor(System.Convert.ToDouble(value) * 100)}%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
