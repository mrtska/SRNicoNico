using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using System.Drawing;

using SRNicoNico.Models.NicoNicoWrapper;



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



		private void Seek_MouseMove(object sender, MouseEventArgs e) {

			//マウスカーソルX座標
			double x = e.GetPosition(this).X;

			
			//シーク中の動画時間
			int ans = (int) (x / ActualWidth * Seek.VideoTime);
			ans = Math.Abs(ans);

			Seek.PopupText = NicoNicoUtil.GetTimeFromLong(ans);
			Seek.PopupRect = new Rect(x - 5, 0, 20, 20);


			if(Seek.IsPopupImageOpen) {

				NicoNicoStoryBoardData Story = App.ViewModelRoot.CurrentVideo.VideoData.StoryBoardData;

				Seek.PopupImageRect = new Rect(x - Story.Width / 2, -10, Story.Width, Story.Height);

				Bitmap test = Story.BitmapCollection[ans - ans % Story.Interval];
				IntPtr hBitMap = test.GetHbitmap();
				try {

					Seek.PopupImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitMap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				} finally {

					DeleteObject(hBitMap);
				}
			}
			

		}

		private void Seek_MouseLeave(object sender, MouseEventArgs e) {



			Seek.IsPopupImageOpen = false;
			Seek.IsPopupOpen = false;
		}

		private void Seek_MouseEnter(object sender, MouseEventArgs e) {


			if(App.ViewModelRoot.CurrentVideo.VideoData.StoryBoardData != null) {

				Seek.IsPopupImageOpen = true;
			}

			Seek.IsPopupOpen = true;
		}

		private void Seek_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {


			//マウスカーソルX座標
			double x = e.GetPosition(this).X;

			App.ViewModelRoot.CurrentVideo.Player.Position = (float)(x / ActualWidth);
		}
	}
}
