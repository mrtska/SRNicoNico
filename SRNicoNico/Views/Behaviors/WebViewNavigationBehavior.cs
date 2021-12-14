using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Xaml.Behaviors;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// 初期URL以外に遷移しようとした時にブラウザでリンクを開くようにするBehavior
    /// </summary>
    public class WebViewNavigationBehavior : Behavior<WebView2> {
        protected async override void OnAttached() {

            await AssociatedObject.EnsureCoreWebView2Async();
            AssociatedObject.CoreWebView2.NavigationStarting += AssociatedObject_NavigationStarting;
            AssociatedObject.CoreWebView2.ProcessFailed += CoreWebView2_ProcessFailed;
        }


        protected override void OnDetaching() {

            AssociatedObject.CoreWebView2.NavigationStarting -= AssociatedObject_NavigationStarting;
            AssociatedObject.CoreWebView2.ProcessFailed -= CoreWebView2_ProcessFailed;
        }

        private void CoreWebView2_ProcessFailed(object? sender, CoreWebView2ProcessFailedEventArgs e) {

            // クラッシュしないようにイベントを登録だけしておく
        }


        private void AssociatedObject_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e) {

            if (e.Uri == AssociatedObject.Source.OriginalString) {

                return;
            }
            Process.Start(new ProcessStartInfo(e.Uri) { UseShellExecute = true });
            e.Cancel = true;
        }
    }
}
