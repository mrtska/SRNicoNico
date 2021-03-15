using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using SRNicoNico.Views.Converters;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// TextBlock用htmlレンダリング用の添付プロパティを提供する
    /// </summary>
    public static class TextBlockHtmlRenderingBehavior {

        public static string GetHtmlText(TextBlock wb) {
            return (string)wb.GetValue(HtmlTextProperty);
        }
        public static void SetHtmlText(TextBlock wb, string html) {
            wb.SetValue(HtmlTextProperty, html);
        }
        public static readonly DependencyProperty HtmlTextProperty =
            DependencyProperty.RegisterAttached("HtmlText", typeof(string), typeof(TextBlockHtmlRenderingBehavior), new FrameworkPropertyMetadata(string.Empty, OnHtmlTextChanged));


        private static void OnHtmlTextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e) {

            if (!(depObj is TextBlock txtBox)) {
                return;
            }
            if (!(e.NewValue is string)) {
                return;
            }
            var html = e.NewValue as string;
            string xaml;
            InlineCollection xamLines;

            try {

                xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, false);
                xamLines = ((Paragraph)((Section)System.Windows.Markup.XamlReader.Parse(xaml)).Blocks.FirstBlock).Inlines;

            } catch {
                return;
            }

            var newLines = new Inline[xamLines.Count];
            xamLines.CopyTo(newLines, 0);

            txtBox.Inlines.Clear();
            foreach (var l in newLines) {
                txtBox.Inlines.Add(l);
            }
        }
    }
}
