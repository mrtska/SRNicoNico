using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views.Controls {
    /// <summary>
    /// htmlをWPF上でレンダリングするためのカスタムコントロール
    /// </summary>
    public class HtmlRenderer : Control {
        
        static HtmlRenderer() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HtmlRenderer), new FrameworkPropertyMetadata(typeof(HtmlRenderer)));
        }

        /// <summary>
        /// 表示したいHTMLテキスト
        /// </summary>
        public string HtmlText {
            get { return (string)GetValue(HtmlTextProperty); }
            set { SetValue(HtmlTextProperty, value); }
        }

        public static readonly DependencyProperty HtmlTextProperty =
            DependencyProperty.Register(nameof(HtmlText), typeof(string), typeof(HtmlRenderer), new PropertyMetadata(string.Empty));
    }
}
