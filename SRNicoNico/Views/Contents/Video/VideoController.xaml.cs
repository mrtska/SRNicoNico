using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using System.Drawing;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;

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

            if(!(DataContext is VideoViewModel)) {

                return;
            }
            VideoViewModel vm = (VideoViewModel) DataContext;

			//マウスカーソルX座標
			double x = e.GetPosition(this).X;

			
			//シーク中の動画時間
			int ans = (int) (x / ActualWidth * Seek.VideoTime);
            if(ans < 0 || Seek.VideoTime < ans) {

                return;
            }

            Seek.PopupText = NicoNicoUtil.ConvertTime(ans);
            Seek.PopupRect = new Rect(x - 5, 0, 20, 20);

            if(Seek.IsPopupImageOpen) {

				NicoNicoStoryBoardData Story = vm.VideoData.StoryBoardData;


                if(Story.BitmapCollection.ContainsKey(ans - ans % Story.Interval)) {


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
		}

		private void Seek_MouseLeave(object sender, MouseEventArgs e) {

            Seek.IsPopupImageOpen = false;
			Seek.IsPopupOpen = false;
		}

		private void Seek_MouseEnter(object sender, MouseEventArgs e) {

            if(!(DataContext is VideoViewModel)) {

                return;
            }
            VideoViewModel vm = (VideoViewModel)DataContext;
            

            if(vm.VideoData.StoryBoardData != null) {

				Seek.IsPopupImageOpen = true;
			}

			Seek.IsPopupOpen = true;
		}

        private void Seek_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            if(!(DataContext is VideoViewModel)) {

                return;
            }
            VideoViewModel vm = (VideoViewModel)DataContext;

            double x = e.GetPosition(this).X;
            int ans = (int)(x / ActualWidth * Seek.VideoTime);
            
            if(ans < 0) {

                ans = 0;
            } else if(ans > Seek.VideoTime) {

                ans = (int) Seek.VideoTime;
            }

            vm.Seek(ans);
        }

    }
}
