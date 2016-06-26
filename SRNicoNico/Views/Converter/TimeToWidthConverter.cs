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
using SRNicoNico.Views.Controls;
using System.Windows;

namespace SRNicoNico.Views.Converter {

    [ValueConversion(typeof(long), typeof(int))]
    public class TimeToWidthConverter : DependencyObject, IValueConverter {

        public double ActualWidth {
            get { return (double)GetValue(ActualWidthProperty); }
            set { SetValue(ActualWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualWidthProperty =
            DependencyProperty.Register("ActualWidth", typeof(double), typeof(TimeToWidthConverter), new PropertyMetadata());




        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var v = (long) value;
            

            return 200;
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
