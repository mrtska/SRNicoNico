using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using Microsoft.Xaml.Behaviors;
using SRNicoNico.Models;
using Unity;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// TextBlockのハイパーリンクを動作するようにするBehavior
    /// </summary>
    public class TextBlockHyperlinkBehavior : Behavior<FrameworkElement> {

        /// <summary>
        /// ブラウザで開くことを強制するかどうか
        /// </summary>
        public bool ForceOpenWithBrowser {
            get { return (bool)GetValue(ForceOpenWithBrowserProperty); }
            set { SetValue(ForceOpenWithBrowserProperty, value); }
        }
        public static readonly DependencyProperty ForceOpenWithBrowserProperty =
            DependencyProperty.Register("ForceOpenWithBrowser", typeof(bool), typeof(TextBlockHyperlinkBehavior), new PropertyMetadata(false));



        protected override void OnAttached() {

            base.OnAttached();

            // ツリー配下のHyperlinkにイベントを設定する
            var style = new Style { TargetType = typeof(Hyperlink) };
            style.Setters.Add(new EventSetter(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(RequestNavigate)));
            style.Setters.Add(new EventSetter(FrameworkContentElement.LoadedEvent, new RoutedEventHandler(Loaded)));
            AssociatedObject.Resources.Add(typeof(Hyperlink), style);
        }

        private void RequestNavigate(object sender, RequestNavigateEventArgs e) {

            if (ForceOpenWithBrowser) {
                // ダウンロードリンクをデフォルトブラウザで開く
                Process.Start(new ProcessStartInfo {
                    UseShellExecute = true,
                    FileName = e.Uri.OriginalString
                });
                return;
            }

            var nnv = App.UnityContainer!.Resolve<INicoNicoViewer>();
            // URLを最適なUIで開く
            nnv.OpenUrl(e.Uri.OriginalString);
        }

        private void Loaded(object sender, RoutedEventArgs e) {

            var hyperlink = (Hyperlink)sender;
            hyperlink.ToolTip = hyperlink.NavigateUri.OriginalString;
        }
    }
}
