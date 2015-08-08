using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Drawing;

namespace SRNicoNico.Views.Contents.Video {
    /// <summary>
    /// VideoController.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoController : UserControl {
        public VideoController() {
            InitializeComponent();
        }

		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		private void Seek_MouseDown(object sender, MouseButtonEventArgs e) {

			Console.WriteLine("MouseDown:" + ActualWidth / e.GetPosition(this).X);
			
		}

		private void Seek_MouseMove(object sender, MouseEventArgs e) {

			//マウスカーソルX座標
			double x = e.GetPosition(this).X;

			
			//シーク中の動画時間
			int ans = (int) (x / ActualWidth * Seek.VideoTime);

			Seek.PopupText = ans.ToString();
			Seek.PopupRect = new Rect(x - 5, 0, 20, 20);


			Seek.PopupImageRect = new Rect(x + 25, -90, 20, 20);


			Bitmap test = App.ViewModelRoot.CurrentVideo.VideoData.StoryBoardData.BitmapCollection[ans & 0x7FFFFFFE];
			IntPtr hBitMap = test.GetHbitmap();
			try {

				Seek.PopupImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitMap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			} finally {

				DeleteObject(hBitMap);
			}

		}

		private void Seek_MouseLeave(object sender, MouseEventArgs e) {

			Seek.IsPopupOpen = false;
		}

		private void Seek_MouseEnter(object sender, MouseEventArgs e) {

			Seek.IsPopupOpen = true;
		}
	}
}
