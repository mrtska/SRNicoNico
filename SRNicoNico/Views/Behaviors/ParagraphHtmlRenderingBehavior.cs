using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using SRNicoNico.Views.Converters;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// ParagraphにHTMLをロードする添付プロパティを提供する
    /// </summary>
    public static class ParagraphHtmlRenderingBehavior {

        public static string GetRawHtmlText(Paragraph wb) {
            return (string)wb.GetValue(RawHtmlTextProperty);
        }
        public static void SetRawHtmlText(Paragraph wb, string html) {
            wb.SetValue(RawHtmlTextProperty, html);
        }
        public static readonly DependencyProperty RawHtmlTextProperty =
            DependencyProperty.RegisterAttached("RawHtmlText", typeof(string), typeof(ParagraphHtmlRenderingBehavior), new FrameworkPropertyMetadata("", OnHtmlTextChanged));


        private static void OnHtmlTextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e) {

            if (!(depObj is Paragraph txtBox)) {
                return;
            }
            if (!(e.NewValue is string html)) {
                return;
            }

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
