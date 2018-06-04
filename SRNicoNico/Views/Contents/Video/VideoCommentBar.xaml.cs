using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace SRNicoNico.Views {
    public partial class VideoCommentBar : UserControl {
        public VideoCommentBar() {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        private static IntPtr GetHwnd(Popup popup) {
            var source = (HwndSource)PresentationSource.FromVisual(popup.Child);
            return source.Handle;
        }

        //popupとTextBoxにフォーカスを当てる
        public void FocusTextBox(Popup popup) {

            SetFocus(GetHwnd(popup));
            comment.Focus();
        }
    }
}
