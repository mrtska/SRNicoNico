using System.Diagnostics;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Toolkit.Wpf.UI.Controls;
using Microsoft.Xaml.Behaviors;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// 初期URL以外に遷移しようとした時にブラウザでリンクを開くようにするBehavior
    /// </summary>
    public class WebViewNavigationBehavior : Behavior<Microsoft.Toolkit.Wpf.UI.Controls.WebView> {

        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.NavigationStarting += AssociatedObject_NavigationStarting;
        }

        protected override void OnDetaching() {
            base.OnDetaching();

            AssociatedObject.NavigationStarting += AssociatedObject_NavigationStarting;
        }

        private void AssociatedObject_NavigationStarting(object sender, WebViewControlNavigationStartingEventArgs e) {

            if (e.Uri == AssociatedObject.Source) {

                return;
            }
            Process.Start(new ProcessStartInfo("cmd", $"/c start {e.Uri.OriginalString.Replace("&", "^&")}") { CreateNoWindow = true });
            e.Cancel = true;
        }
    }
}
