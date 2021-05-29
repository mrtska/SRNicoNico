using System;
using System.Globalization;
using System.Windows.Data;

namespace SRNicoNico.Views.Converters {
    /// <summary>
    /// センチ秒を分と時間に分けて:で区切った文字に変換する
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class VposDurationConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var seconds = System.Convert.ToInt32(value) / 100;
            int minutes = seconds / 60;
            int second = seconds % 60;

            return $"{minutes}:{second:00}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
