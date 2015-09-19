using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

using SRNicoNico.Views.Extentions;
using SRNicoNico.Views.Contents.Video;
using SRNicoNico.ViewModels;


namespace SRNicoNico.Views.Behaviors {


    public static class HtmlTextBoxProperties  {
        public static string GetHtmlText(TextBlock wb) {
            return wb.GetValue(HtmlTextProperty) as string;
        }
        public static void SetHtmlText(TextBlock wb, string html) {
            wb.SetValue(HtmlTextProperty, html);
        }
        public static readonly DependencyProperty HtmlTextProperty =
            DependencyProperty.RegisterAttached("HtmlText", typeof(string), typeof(HtmlTextBoxProperties), new UIPropertyMetadata("", OnHtmlTextChanged));

        public static readonly RoutedEvent TriggerRequestEvent = EventManager.RegisterRoutedEvent("TriggerRequest", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HtmlTextBoxProperties));

        public static void AddTriggerRequestHandler(DependencyObject d, RoutedEventHandler handler) {
            UIElement uie = d as UIElement;
            if(uie != null) {
                uie.AddHandler(TriggerRequestEvent, handler);
            }
        }
        public static void RemoveTriggerRequestHandler(DependencyObject d, RoutedEventHandler handler) {
            UIElement uie = d as UIElement;
            if(uie != null) {
                uie.RemoveHandler(TriggerRequestEvent, handler);
            }
        }

        private static void AddHandler(Hyperlink link) {

            //たまにnullの時がある
            if(link.NavigateUri != null) {

                if(link.NavigateUri.OriginalString.StartsWith("http://www.nicovideo.jp/watch/")) {

                    ToolTipService.SetShowDuration(link, 100000);

                    ToolTipService.SetHorizontalOffset(link, 150);
                    link.ToolTipOpening += Link_ToolTipOpening;
                    VideoToolTip tooltip = new VideoToolTip();
                    VideoDataViewModel vm = new VideoDataViewModel();
                    tooltip.DataContext = vm;


                    link.ToolTip = tooltip;

                } else {

                    link.ToolTip = link.NavigateUri.OriginalString;
                }
                link.RequestNavigate += ((sender, o) => {

                    link.RaiseEvent(new RoutedEventArgs(TriggerRequestEvent));
                });
            }
        }

        private static void Link_ToolTipOpening(object sender, ToolTipEventArgs e) {

            Hyperlink link = sender as Hyperlink;
            VideoToolTip tooltip = link.ToolTip as VideoToolTip;
            
            ((VideoDataViewModel)tooltip.DataContext).Intialize(link.NavigateUri.OriginalString.Substring(30));
        }

        private static void OnHtmlTextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e) {
            // Go ahead and return out if we set the property on something other than a textblock, or set a value that is not a string.
            var txtBox = depObj as TextBlock;
            if(txtBox == null)
                return;
            if(!(e.NewValue is string))
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

            //ハイパーリンクにイベントを設定する
            foreach(Inline inline in newLines) {

                //Spanの中のハイパーリンクにも設定する
                if(inline is Span) {

                    Span span = (Span) inline;
                    foreach(Inline spanInline in span.Inlines) {

                        if(spanInline is Hyperlink) {

                            Hyperlink spanLink = (Hyperlink)spanInline;

                            AddHandler(spanLink);
                        }
                    }
                }
                //イベント設定
                if(inline is Hyperlink) {

                    Hyperlink link = (Hyperlink) inline;
                    AddHandler(link);
                    
                }
            }

            txtBox.Inlines.Clear();
            foreach(var l in newLines) {
                txtBox.Inlines.Add(l);
            }
        }
    }
}
