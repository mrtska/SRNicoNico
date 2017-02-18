using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRNicoNico.Views {
    /// <summary>
    /// VideoCommentBar.xaml の相互作用ロジック
    /// </summary>
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
