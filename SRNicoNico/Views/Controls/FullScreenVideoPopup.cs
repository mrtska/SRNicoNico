using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace SRNicoNico.Views.Controls {
    /// <summary>
    /// フルスクリーン時のシークバーを良い感じにしてくれる専用コントロール
    /// </summary>
    public class FullScreenVideoPopup : Popup {

        /// <summary>
        /// ポップアップの位置
        /// </summary>
        public PopupPlacement PopupPlacement {
            get { return (PopupPlacement)GetValue(PopupPlacementProperty); }
            set { SetValue(PopupPlacementProperty, value); }
        }
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register(nameof(PopupPlacement), typeof(PopupPlacement), typeof(FullScreenVideoPopup), new FrameworkPropertyMetadata(PopupPlacement.Top));

        /// <summary>
        /// 常時オープンかどうか
        /// </summary>
        public bool AlwaysOpen {
            get { return (bool)GetValue(AlwaysOpenProperty); }
            set { SetValue(AlwaysOpenProperty, value); }
        }
        public static readonly DependencyProperty AlwaysOpenProperty =
            DependencyProperty.Register(nameof(AlwaysOpen), typeof(bool), typeof(FullScreenVideoPopup), new FrameworkPropertyMetadata(false));

        static FullScreenVideoPopup() {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(FullScreenVideoPopup), new FrameworkPropertyMetadata(typeof(FullScreenVideoPopup)));
        }

        private Timer? CloseTimer;
        private double CloseCount;
        private bool IsScrolled = false;

        public FullScreenVideoPopup() {

            StaysOpen = true;

            Loaded += (o, e) => {

                if (!AlwaysOpen) {
                    // 雑なタイマー実装だけど動くのでヨシ
                    CloseTimer = new Timer((_) => {
                        if (!IsScrolled && !IsMouseOver) {
                            CloseCount -= 100;
                            if (CloseCount <= 0) {
                                Dispatcher.Invoke(() => IsOpen = false);
                            }
                        }
                    }, null, 0, 100);
                }
                // ポップアップの親要素にいるScrollViewer要素を取得する
                var scrollViewer = (ScrollViewer)GetDependencyObjectFromVisualTree(this, typeof(ScrollViewer));
                // ListBoxなどのようなScrollViewerを持った要素内に設定された場合の動作
                scrollViewer.ScrollChanged += OnScrollChanged;

                // シークバーを一番下に持ってくる
                if (PopupPlacement == PopupPlacement.Bottom) {

                    VerticalOffset = scrollViewer.ActualHeight;
                } else if (PopupPlacement == PopupPlacement.Top) {

                    // シークバーが上の時は説明文が上に来るので動画がフルスクリーンになるように一番下にスクロールしておく
                    scrollViewer.ScrollToBottom();
                }
            };

            Unloaded += (o, e) => {

                var scrollViewer = (ScrollViewer)GetDependencyObjectFromVisualTree(this, typeof(ScrollViewer));
                scrollViewer.ScrollChanged -= OnScrollChanged;

                CloseTimer?.Dispose();
            };
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e) {

            if (PopupPlacement == PopupPlacement.Bottom) {

                // ポップアップの親要素にいるScrollViewer要素を取得する
                var scrollViewer = (ScrollViewer)GetDependencyObjectFromVisualTree(this, typeof(ScrollViewer));
                var child = (FrameworkElement)Child;
                VerticalOffset = scrollViewer.ActualHeight - child.ActualHeight;
                IsScrolled = e.VerticalOffset != 0;

            } else if (PopupPlacement == PopupPlacement.Top) {

                VerticalOffset = e.ExtentHeight - e.ViewportHeight;
                IsScrolled = Math.Round(e.ViewportHeight + e.VerticalOffset, 0) != Math.Round(e.ExtentHeight, 0);
            }

            // スクロールされたらポップアップの位置を更新する
            var offset = HorizontalOffset;
            HorizontalOffset = offset + 0.1;
            HorizontalOffset = offset;
        }

        private DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type) {
            var parent = startObject;
            while (parent != null) {
                if (type.IsInstanceOfType(parent)) {
                    break;
                } else {
                    parent = VisualTreeHelper.GetParent(parent);
                }
            }
            return parent!;
        }

        protected override void OnOpened(EventArgs e) {
            base.OnOpened(e);

            var hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;
            if (GetWindowRect(hwnd, out var rect)) {
                SetWindowPos(hwnd, -2, rect.Left, rect.Top, (int)Width, (int)Height, SWP_NOACTIVATE);
            }
            CloseCount = 3000;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        private const int HWND_TOPMOST = -1;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_NOSENDCHANGING = 0x0400;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, uint wFlags);
    }

    public enum PopupPlacement {
        /// <summary>
        /// 上に配置
        /// </summary>
        Top,
        /// <summary>
        /// 下に配置
        /// </summary>
        Bottom
    }
}
