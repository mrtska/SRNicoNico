using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SRNicoNico.Views.Converter {
    public class FontFamilyToNameConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

#if DEBUG
            if((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)) {
                return "Yu Gothic UI";
            }
#endif



            var v = value as FontFamily;
            var currentLang = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
            return v.FamilyNames.FirstOrDefault(o => o.Key == currentLang).Value ?? v.Source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
