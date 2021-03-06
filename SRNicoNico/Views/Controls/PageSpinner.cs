using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views.Controls {
    /// <summary>
    /// ページネーション用のUIを提供するカスタムコントロール
    /// </summary>
    public class PageSpinner : ContentControl {
        
        static PageSpinner() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PageSpinner), new FrameworkPropertyMetadata(typeof(PageSpinner)));
        }
    }
}
