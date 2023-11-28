using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace SRNicoNico.Views.Converters {
    /// <summary>
    /// コメントのラベルをヒューマンリーダブルな形式に変換する
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class CommentLabelConverter : IValueConverter {

        private readonly static Dictionary<string, string> ConversionMap = new Dictionary<string, string>() {
            ["default"] = "通常コメント",
            ["main"] = "通常コメント",
            ["easy"] = "かんたんコメント",
            ["owner"] = "投稿者コメント",
            ["community"] = "チャンネルコメント"
        };
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            var str = System.Convert.ToString(value)!;
            
            if (ConversionMap.ContainsKey(str)) {

                return ConversionMap[str];
            }
            throw new InvalidOperationException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new InvalidOperationException();
        }
    }
}
