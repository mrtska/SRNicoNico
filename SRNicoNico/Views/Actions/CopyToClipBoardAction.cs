using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace SRNicoNico.Views.Actions {
    /// <summary>
    /// 指定したテキストをクリップボードにコピーするトリガーアクション
    /// </summary>
    public class CopyToClipBoardAction : TriggerAction<DependencyObject> {
        /// <summary>
        /// コピーしたいテキスト
        /// </summary>
        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(CopyToClipBoardAction), new PropertyMetadata(""));

        protected override void Invoke(object parameter) {

            Clipboard.SetText(Text);
        }
    }
}
