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

        protected override void OnAttached() {

            base.OnAttached();

            // ツリー配下のHyperlinkにイベントを設定する
            var style = new Style { TargetType = typeof(Hyperlink) };
            style.Setters.Add(new EventSetter(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(RequestNavigate)));
            style.Setters.Add(new EventSetter(FrameworkContentElement.LoadedEvent, new RoutedEventHandler(Loaded)));
            AssociatedObject.Resources.Add(typeof(Hyperlink), style);
        }

        private void RequestNavigate(object sender, RequestNavigateEventArgs e) {

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
