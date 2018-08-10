using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using SRNicoNico.Views.Converter;

namespace SRNicoNico.Views.Behavior {

    public static class ParagraphHtmlRenderingBehavior {

        public static string GetRawHtmlText(Paragraph wb) {
            return wb.GetValue(RawHtmlTextProperty) as string;
        }
        public static void SetRawHtmlText(Paragraph wb, string html) {
            wb.SetValue(RawHtmlTextProperty, html);
        }
        public static readonly DependencyProperty RawHtmlTextProperty =
            DependencyProperty.RegisterAttached("RawHtmlText", typeof(string), typeof(ParagraphHtmlRenderingBehavior), new UIPropertyMetadata("", OnHtmlTextChanged));


        private static void OnHtmlTextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e) {
            // Go ahead and return out if we set the property on something other than a textblock, or set a value that is not a string.
            if (!(depObj is Paragraph txtBox))
                return;
            if (!(e.NewValue is string))
                return;
            var html = e.NewValue as string;
            string xaml;
            InlineCollection xamLines;

            try {

                xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, false);
                xamLines = ((Paragraph)((Section)System.Windows.Markup.XamlReader.Parse(xaml)).Blocks.FirstBlock).Inlines;

            } catch {
                // There was a problem parsing the html, return out. 
                return;
            }
            // Create a copy of the Inlines and add them to the TextBlock.
            Inline[] newLines = new Inline[xamLines.Count];
            xamLines.CopyTo(newLines, 0);

            txtBox.Inlines.Clear();
            foreach(var l in newLines) {
                txtBox.Inlines.Add(l);
            }
        }
    }
}
