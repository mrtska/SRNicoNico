using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Views.Controls {


	public class SeekBar : Control {


		public long VideoTime {
			get { return (long)GetValue(VideoTimeProperty); }
			set { SetValue(VideoTimeProperty, value); }
		}


		// Using a DependencyProperty as the backing store for VideoTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty VideoTimeProperty =
			DependencyProperty.Register("VideoTime", typeof(long), typeof(SeekBar), new FrameworkPropertyMetadata(0L));



		

		public long CurrentTime {
			get { return (long)GetValue(CurrentTimeProperty); }
			set { SetValue(CurrentTimeProperty, value);	}
		}

		// Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentTimeProperty =
			DependencyProperty.Register("CurrentTime", typeof(long), typeof(SeekBar), new FrameworkPropertyMetadata(0L));



		public double CurrentTimeWidth {
			get { return (double)GetValue(CurrentTimeWidthProperty); }
			set { SetValue(CurrentTimeWidthProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CurrentTimeString.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentTimeWidthProperty =
			DependencyProperty.Register("CurrentTimeWidth", typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0.0));





		public long BufferedTime {
			get { return (long)GetValue(BufferedTimeProperty); }
			set { SetValue(BufferedTimeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BufferedTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BufferedTimeProperty =
			DependencyProperty.Register("BufferedTime", typeof(long), typeof(SeekBar), new FrameworkPropertyMetadata(0L));



		public double BufferedTimeWidth {
			get { return (double)GetValue(BufferedTimeWidthProperty); }
			set { SetValue(BufferedTimeWidthProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BufferedTimeString.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BufferedTimeWidthProperty =
			DependencyProperty.Register("BufferedTimeWidth", typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0.0));



		public Thickness SeekCursor {
			get { return (Thickness)GetValue(SeekCursorProperty); }
			set { SetValue(SeekCursorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SeekCursor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SeekCursorProperty =
			DependencyProperty.Register("SeekCursor", typeof(Thickness), typeof(SeekBar), new FrameworkPropertyMetadata(null));





        public string PopupText {
			get { return (string)GetValue(PopupTextProperty); }
			set { SetValue(PopupTextProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PopupText.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PopupTextProperty =
			DependencyProperty.Register("PopupText", typeof(string), typeof(SeekBar), new FrameworkPropertyMetadata(""));



		public bool IsPopupOpen {
			get { return (bool)GetValue(IsPopupOpenProperty); }
			set { SetValue(IsPopupOpenProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsPopupOpen.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsPopupOpenProperty =
			DependencyProperty.Register("IsPopupOpen", typeof(bool), typeof(SeekBar), new FrameworkPropertyMetadata(false));



		public bool IsPopupImageOpen {
			get { return (bool)GetValue(IsPopupImageOpenProperty); }
			set { SetValue(IsPopupImageOpenProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsPopupImageOpen.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsPopupImageOpenProperty =
			DependencyProperty.Register("IsPopupImageOpen", typeof(bool), typeof(SeekBar), new FrameworkPropertyMetadata(false));





		public Rect PopupRect {
			get { return (Rect)GetValue(PopupRectProperty); }
			set { SetValue(PopupRectProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PopupRect.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PopupRectProperty =
			DependencyProperty.Register("PopupRect", typeof(Rect), typeof(SeekBar), new FrameworkPropertyMetadata(null));



		public Rect PopupImageRect {
			get { return (Rect)GetValue(PopupImageRectProperty); }
			set { SetValue(PopupImageRectProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PopupImageRect.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PopupImageRectProperty =
			DependencyProperty.Register("PopupImageRect", typeof(Rect), typeof(SeekBar), new FrameworkPropertyMetadata(null));



		public BitmapSource PopupImage {
			get { return (BitmapSource)GetValue(PopupImageProperty); }
			set { SetValue(PopupImageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PopupImage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PopupImageProperty =
			DependencyProperty.Register("PopupImage", typeof(BitmapSource), typeof(SeekBar), new FrameworkPropertyMetadata(null));


		static SeekBar() {

			DefaultStyleKeyProperty.OverrideMetadata(typeof(SeekBar), new FrameworkPropertyMetadata(typeof(SeekBar)));
		}

        public SeekBar() {

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Fired;
            timer.Start();
        }

        private void Fired(object sender, EventArgs e) {

            CurrentTimeWidth = ActualWidth / VideoTime * CurrentTime;
            SeekCursor = new Thickness(CurrentTimeWidth, 0, 0, 0);
            BufferedTimeWidth = BufferedTime * ActualWidth;
        }




	}
}
