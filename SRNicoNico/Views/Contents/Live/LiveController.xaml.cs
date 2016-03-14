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

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);



		private void Seek_MouseMove(object sender, MouseEventArgs e) {

            if(!(DataContext is VideoViewModel)) {

                return;
            }
            var vm = (VideoViewModel) DataContext;

			//マウスカーソルX座標
			double x = e.GetPosition(this).X;

			
			//シーク中の動画時間
			int ans = (int) (x / ActualWidth * Seek.VideoTime);
            if(ans < 0 || Seek.VideoTime < ans) {

                return;
            }

            Seek.PopupText = NicoNicoUtil.ConvertTime(ans);
            Seek.PopupRect = new Rect(x - 5, 0, 20, 20);

		}

		private void Seek_MouseLeave(object sender, MouseEventArgs e) {

			Seek.IsPopupOpen = false;
		}

		private void Seek_MouseEnter(object sender, MouseEventArgs e) {

            if(!(DataContext is LiveWatchViewModel)) {

                return;
            }

			Seek.IsPopupOpen = true;
		}

        private void Seek_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            if(!(DataContext is LiveWatchViewModel)) {

                return;
            }
            var vm = (LiveWatchViewModel)DataContext;

            double x = e.GetPosition(this).X;
            int ans = (int)(x / ActualWidth * Seek.VideoTime);
            
            if(ans < 0) {

                ans = 0;
            } else if(ans > Seek.VideoTime) {

                ans = (int) Seek.VideoTime;
            }

            vm.Handler.Seek(ans);
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
