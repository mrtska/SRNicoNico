using System;
using System.Globalization;
using System.Windows.Data;

namespace SRNicoNico.Views.Converters {
    /// <summary>
    /// bool値を反転するコンバータ
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }
    }
}
