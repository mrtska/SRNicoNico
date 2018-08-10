using System.Windows.Controls;
using System.Windows.Input;

namespace SRNicoNico.Views.Controls {
    public class DisabledScrollViewerFlowDocumentScrollViewer : FlowDocumentScrollViewer {

        protected override void OnMouseWheel(MouseWheelEventArgs e) {

            // FlowDocumentScrollViewerのスクロール処理を無効
            // FlowDocumentScrollViewerのScrollViewerは、スクロール処理がこのイベントを受け取った後に
            // _ContentHost.LineDown()/LineUp()しか呼んでいなく、スクロール量を全く考慮しないので使いづらい
        }
    }
}
