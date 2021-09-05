using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// コメントリストを自動スクロールするビヘイビア
    /// </summary>
    public class CommentAutoScrollBehavior : Behavior<ListBox> {

        /// <summary>
        /// オートスクロールの有効状態
        /// </summary>
        public bool AutoScrollEnabled {
            get => (bool)GetValue(AutoScrollEnabledProperty);
            set => SetValue(AutoScrollEnabledProperty, value);
        }
        public static readonly DependencyProperty AutoScrollEnabledProperty =
            DependencyProperty.Register(nameof(AutoScrollEnabled), typeof(bool), typeof(CommentAutoScrollBehavior), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 現在再生時間
        /// </summary>
        public double CurrentTime {
            get => (double)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(nameof(CurrentTime), typeof(double), typeof(CommentAutoScrollBehavior), new FrameworkPropertyMetadata(0D, (o, e) => {

                var behavior = (CommentAutoScrollBehavior)o;
                behavior.CurrentTimeChanged((double)e.NewValue);
            }));

        private void CurrentTimeChanged(double value) {

            // オートスクロールが無効、コメント欄にマウスカーソルが乗っている、コメントが0件の時は何もしない
            if (!AutoScrollEnabled || AssociatedObject.IsMouseOver || AssociatedObject.Items.IsEmpty) {
                return;
            }

            if (!(AssociatedObject.ItemsSource is IEnumerable<VideoCommentEntry> items)) {
                return;
            }

            var comment = items.LastOrDefault(w => w.Vpos <= (int)(value * 100));
            if (comment != null) {
                AssociatedObject.ScrollIntoView(comment);
            }
        }
    }
}
