using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace SRNicoNico.Views.Controls {
    /// <summary>
    /// ポップアップのzIndexを良い感じにしてくれるコントロール
    /// </summary>
    public class ManagedPopup : Popup {

        public bool Topmost {
            get { return (bool)GetValue(TopmostProperty); }
            set { SetValue(TopmostProperty, value); }
        }

        public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(
            typeof(ManagedPopup), new FrameworkPropertyMetadata(false, OnTopmostChanged));

        private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            ((ManagedPopup)obj).UpdateWindow();
        }

        static ManagedPopup() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ManagedPopup), new FrameworkPropertyMetadata(typeof(ManagedPopup)));
        }

        private void OnFollowWindowChanged(object? sender, EventArgs e) {
            var offset = HorizontalOffset;
            // HorizontalOffsetなどのプロパティを一度変更しないと、ポップアップの位置が更新されないため、
            // 同一プロパティに2回値をセットしている。
            HorizontalOffset = offset + 1;
            HorizontalOffset = offset;
        }

        protected override void OnOpened(EventArgs e) {
            UpdateWindow();
            base.OnOpened(e);

            var window = Window.GetWindow(Child);
            window.LocationChanged += OnFollowWindowChanged;
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);

            var window = Window.GetWindow(Child);
            window.LocationChanged -= OnFollowWindowChanged;
        }

        private void UpdateWindow() {
            var hwnd = ((HwndSource)PresentationSource.FromVisual(Child)).Handle;

            if (GetWindowRect(hwnd, out var rect)) {
                SetWindowPos(hwnd, Topmost ? -1 : -2, rect.Left, rect.Top, (int)Width, (int)Height, SWP_NOACTIVATE);
            }
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
}
