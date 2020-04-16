using System.Diagnostics;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Xaml.Behaviors;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// 初期URL以外に遷移しようとした時にブラウザでリンクを開くようにするBehavior
    /// </summary>
    public class WebViewNavigationBehavior : Behavior<Microsoft.Toolkit.Wpf.UI.Controls.WebView> {
        protected override void OnAttached() {

            AssociatedObject.NavigationStarting += AssociatedObject_NavigationStarting;
        }
        protected override void OnDetaching() {

            AssociatedObject.NavigationStarting += AssociatedObject_NavigationStarting;
        }

        private void AssociatedObject_NavigationStarting(object? sender, WebViewControlNavigationStartingEventArgs e) {

            if (e.Uri == AssociatedObject.Source) {

                return;
            }
            Process.Start(new ProcessStartInfo(e.Uri.OriginalString) { UseShellExecute = true });
            e.Cancel = true;
        }
    }
}
