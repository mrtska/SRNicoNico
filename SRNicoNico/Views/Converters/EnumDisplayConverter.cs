using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace SRNicoNico.Views.Converters {
    /// <summary>
    /// enumに付加されているDisplay属性の値にenumを変換する
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class EnumDisplayConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var field = value.GetType().GetField(value.ToString()!);
            var display = field?.GetCustomAttribute<DisplayAttribute>();

            return display is null ? value.ToString()! : display.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
