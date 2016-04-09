using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using System.Drawing;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace SRNicoNico.Views.Contents.Live {
	/// <summary>
	/// LiveController.xaml の相互作用ロジック
	/// </summary>
	public partial class LiveController : UserControl {
        public LiveController() {
            InitializeComponent();
        }


        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        IntPtr GetHwnd(Popup popup) {
            var source = (HwndSource)PresentationSource.FromVisual(popup.Child);
            return source.Handle;
        }

        private void popup_Opened(object sender, EventArgs e) {

            //PopupにTextBoxを配置するとIMEがおかしくなるので
            var handle = GetHwnd(popup);
            SetFocus(handle);
        }

        public void FocusComment() {

            //TextBoxにフォーカスを移す
            comment.comment.Focus();
        }
    }
}
