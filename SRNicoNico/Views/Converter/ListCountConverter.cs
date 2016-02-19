using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Livet;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views.Converter {

    //マイリストのリストからブロマガとかを弾いたCountに変換する
    public class ListCountConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var v = value as DispatcherCollection<MylistListEntryViewModel>;

            if(v == null) {

                return 0;
            }

            foreach(var entry in v) {

                if(entry.Entry.Type == 0) {

                    return v.Count;
                }
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
