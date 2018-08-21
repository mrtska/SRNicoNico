using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SRNicoNico.Views.Controls {
    public class DisabledScrollViewerFlowDocumentScrollViewer : FlowDocumentScrollViewer {

        protected override void OnMouseWheel(MouseWheelEventArgs e) {

            var host = GetType().GetProperty("ScrollViewer", BindingFlags.NonPublic | BindingFlags.Instance);

            var scroll = host.GetValue(this) as ScrollViewer;
            var info = scroll.GetType().GetProperty("ScrollInfo", BindingFlags.NonPublic | BindingFlags.Instance);

            var scrollInfo = info.GetValue(scroll) as IScrollInfo;

            if (e.Delta < 0) {

                scrollInfo.MouseWheelDown();
            } else {

                scrollInfo.MouseWheelUp();
            }
            // FlowDocumentScrollViewerのスクロール処理を無効
            // FlowDocumentScrollViewerのScrollViewerは、スクロール処理がこのイベントを受け取った後に
            // _ContentHost.LineDown()/LineUp()しか呼んでいなく、スクロール量を全く考慮しないので使いづらい
            // リフレクションでScrollInfoを取得してマウスホイール用のスクロールメソッドを呼び出す
        }
    }
}
