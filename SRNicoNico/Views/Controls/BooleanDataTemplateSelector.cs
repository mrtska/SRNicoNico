using System;
using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views.Controls {
    public class BooleanDataTemplateSelector : DataTemplateSelector {

        public DataTemplate TrueTemplate { get; set; }
        public DataTemplate FalseTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {

            if(item == null) {

                return base.SelectTemplate(item, container);
            }

            if(item is bool value) {

                if(value) {

                    return TrueTemplate;
                } else {

                    return FalseTemplate;
                }
            }
            throw new InvalidOperationException("Boolean DataTemplete Selectorにbool以外の値が指定されました。");
        }
    }
}
