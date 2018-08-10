using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views.Controls {
    public class HtmlRenderer : Control {

        static HtmlRenderer() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HtmlRenderer), new FrameworkPropertyMetadata(typeof(HtmlRenderer)));
        }
        public string HtmlText {
            get { return (string)GetValue(HtmlTextProperty); }
            set { SetValue(HtmlTextProperty, value); }
        }

        public static readonly DependencyProperty HtmlTextProperty =
            DependencyProperty.Register("HtmlText", typeof(string), typeof(HtmlRenderer), new PropertyMetadata(""));

    }
}
