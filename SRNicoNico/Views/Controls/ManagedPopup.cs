using System;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media;

namespace SRNicoNico.Views.Controls {

    //WPFのポップアップをほかのウィンドウには重ならないようにするすごいやつ 感謝 (http://sourcechord.hatenablog.com/entry/2014/10/25/205036) thanks (https://chriscavanagh.wordpress.com/2008/08/13/non-topmost-wpf-popup/) 
    public class ManagedPopup : Popup {

        public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(
        typeof(ManagedPopup), new FrameworkPropertyMetadata(false, OnTopmostChanged));

        static ManagedPopup() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ManagedPopup), new FrameworkPropertyMetadata(typeof(ManagedPopup)));

            // PopupのIsOpenプロパティ更新のイベントハンドラを設定する。
            ManagedPopup.IsOpenProperty.OverrideMetadata(typeof(ManagedPopup), new FrameworkPropertyMetadata(IsOpenChanged));
        }

        private static void IsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var ctrl = d as ManagedPopup;
            if(ctrl == null)
                return;

            var target = ctrl.PlacementTarget;
            if(target == null)
                return;

            var win = Window.GetWindow(target);
            // ポップアップの親要素にいるScrollViewer要素があれば取得する。
            var scrollViewer = ctrl.GetDependencyObjectFromVisualTree(ctrl, typeof(ScrollViewer)) as ScrollViewer;

            // 更新前のIsOpenプロパティがtrueだったので、
            // 登録済みのイベントハンドラを解除する。
            if(e.OldValue != null && (bool)e.OldValue == true) {
                if(win != null) {
                    // ウィンドウの移動/リサイズ時の処理を設定
                    win.LocationChanged -= ctrl.OnFollowWindowChanged;
                    win.SizeChanged -= ctrl.OnFollowWindowChanged;
                }

                if(scrollViewer != null) {
                    // ListBoxなどのようなScrollViewerを持った要素内に設定された場合の動作
                    scrollViewer.ScrollChanged -= ctrl.OnScrollChanged;
                }
            }

            // IsOpenプロパティをtrueに変更したので、
            // 各種イベントハンドラを登録する。
            if(e.NewValue != null && (bool)e.NewValue == true) {
                if(win != null) {
                    // ウィンドウの移動/リサイズ時の処理を設定
                    win.LocationChanged += ctrl.OnFollowWindowChanged;
                    win.SizeChanged += ctrl.OnFollowWindowChanged;
                }

                if(scrollViewer != null) {
                    // ListBoxなどのようなScrollViewerを持った要素内に設定された場合の動作
                    scrollViewer.ScrollChanged += ctrl.OnScrollChanged;
                }
            }
        }

        private void OnFollowWindowChanged(object sender, EventArgs e) {
            var offset = this.HorizontalOffset;
            // HorizontalOffsetなどのプロパティを一度変更しないと、ポップアップの位置が更新されないため、
            // 同一プロパティに2回値をセットしている。
            this.HorizontalOffset = offset + 1;
            this.HorizontalOffset = offset;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e) {
            this.IsOpen = false;
        }

        private DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type) {
            var parent = startObject;
            while(parent != null) {
                if(type.IsInstanceOfType(parent))
                    break;
                else
                    parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }

        public bool Topmost {
            get { return (bool)GetValue(TopmostProperty); }
            set { SetValue(TopmostProperty, value); }
        }

        private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            (obj as ManagedPopup).UpdateWindow();
        }

        protected override void OnOpened(EventArgs e) {
            UpdateWindow();
        }

        private void UpdateWindow() {
            var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
            RECT rect;

            if(GetWindowRect(hwnd, out rect)) {
                SetWindowPos(hwnd, Topmost ? -1 : -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
            }
        }

        #region P/Invoke imports & definitions

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion
    }
}