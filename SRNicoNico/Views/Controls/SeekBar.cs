using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.Views.Controls {


	public class SeekBar : Control {


		public long VideoTime {
			get { return (long)GetValue(VideoTimeProperty); }
			set { SetValue(VideoTimeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for VideoTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty VideoTimeProperty =
			DependencyProperty.Register(nameof(VideoTime), typeof(long), typeof(SeekBar), new FrameworkPropertyMetadata(0L));


        public long CurrentTime {
			get { return (long)GetValue(CurrentTimeProperty); }
			set { SetValue(CurrentTimeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentTimeProperty =
			DependencyProperty.Register(nameof(CurrentTime), typeof(long), typeof(SeekBar), new FrameworkPropertyMetadata(0L, OnCurrentTimePropertyChanged));

        private static void OnCurrentTimePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            var control = source as SeekBar;
            long time = (long)e.NewValue;

            if(!control.IsDrag) {

                control.CurrentTimeWidth = (control.ActualWidth - 10) / control.VideoTime * control.CurrentTime;
                control.SeekCursor = new Thickness(control.CurrentTimeWidth, 0, 0, 0);
            }

        }


        public double CurrentTimeWidth {
			get { return (double)GetValue(CurrentTimeWidthProperty); }
			set { SetValue(CurrentTimeWidthProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CurrentTimeString.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentTimeWidthProperty =
			DependencyProperty.Register(nameof(CurrentTimeWidth), typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0.0));


		public double BufferedTime {
			get { return (double)GetValue(BufferedTimeProperty); }
			set { SetValue(BufferedTimeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BufferedTime.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BufferedTimeProperty =
			DependencyProperty.Register(nameof(BufferedTime), typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0D, OnBufferTimePropertyChanged));

        private static void OnBufferTimePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            var control = source as SeekBar;
            double time = (double)e.NewValue;

            control.BufferedTimeWidth = control.BufferedTime * control.ActualWidth;

        }


        public double BufferedTimeWidth {
			get { return (double)GetValue(BufferedTimeWidthProperty); }
			set { SetValue(BufferedTimeWidthProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BufferedTimeString.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BufferedTimeWidthProperty =
			DependencyProperty.Register(nameof(BufferedTimeWidth), typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0.0));



		public Thickness SeekCursor {
			get { return (Thickness)GetValue(SeekCursorProperty); }
			set { SetValue(SeekCursorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for SeekCursor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SeekCursorProperty =
			DependencyProperty.Register(nameof(SeekCursor), typeof(Thickness), typeof(SeekBar), new FrameworkPropertyMetadata(null));


        public string PopupText {
			get { return (string)GetValue(PopupTextProperty); }
			set { SetValue(PopupTextProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PopupText.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PopupTextProperty =
			DependencyProperty.Register(nameof(PopupText), typeof(string), typeof(SeekBar), new FrameworkPropertyMetadata(""));



		public bool IsPopupOpen {
			get { return (bool)GetValue(IsPopupOpenProperty); }
			set { SetValue(IsPopupOpenProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsPopupOpen.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsPopupOpenProperty =
			DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(SeekBar), new FrameworkPropertyMetadata(false));



		public bool IsPopupImageOpen {
			get { return (bool)GetValue(IsPopupImageOpenProperty); }
			set { SetValue(IsPopupImageOpenProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsPopupImageOpen.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsPopupImageOpenProperty =
			DependencyProperty.Register(nameof(IsPopupImageOpen), typeof(bool), typeof(SeekBar), new FrameworkPropertyMetadata(false));





		public Rect PopupRect {
			get { return (Rect)GetValue(PopupRectProperty); }
			set { SetValue(PopupRectProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PopupRect.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PopupRectProperty =
			DependencyProperty.Register(nameof(PopupRect), typeof(Rect), typeof(SeekBar), new FrameworkPropertyMetadata(null));



		public Rect PopupImageRect {
			get { return (Rect)GetValue(PopupImageRectProperty); }
			set { SetValue(PopupImageRectProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PopupImageRect.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PopupImageRectProperty =
			DependencyProperty.Register(nameof(PopupImageRect), typeof(Rect), typeof(SeekBar), new FrameworkPropertyMetadata(null));



		public BitmapSource PopupImage {
			get { return (BitmapSource)GetValue(PopupImageProperty); }
			set { SetValue(PopupImageProperty, value); }
		}

		// Using a DependencyProperty as the backing store for PopupImage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty PopupImageProperty =
			DependencyProperty.Register(nameof(PopupImage), typeof(BitmapSource), typeof(SeekBar), new FrameworkPropertyMetadata(null));



        public bool IsDrag {
            get { return (bool)GetValue(IsDragProperty); }
            set { SetValue(IsDragProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDrag.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDragProperty =
            DependencyProperty.Register("IsDrag", typeof(bool), typeof(SeekBar), new PropertyMetadata(false));



        static SeekBar() {

			DefaultStyleKeyProperty.OverrideMetadata(typeof(SeekBar), new FrameworkPropertyMetadata(typeof(SeekBar)));
		}

        public SeekBar() {

            MouseLeftButtonDown += SeekBar_MouseLeftButtonDown;
            MouseLeftButtonUp += SeekBar_MouseLeftButtonUp;
            MouseMove += SeekBar_MouseMove;
            SizeChanged += SeekBar_SizeChanged;
        }

        private void SeekBar_SizeChanged(object sender, SizeChangedEventArgs e) {

            CurrentTimeWidth = (ActualWidth - 10) / VideoTime * CurrentTime;
            SeekCursor = new Thickness(CurrentTimeWidth, 0, 0, 0);
            BufferedTimeWidth = BufferedTime * ActualWidth;

        }

        private void SeekBar_MouseMove(object sender, MouseEventArgs e) {

            if(IsDrag) {

                //マウスカーソルX座標
                double x = e.GetPosition(this).X;


                //シーク中の動画時間
                int ans = (int)(x / ActualWidth * VideoTime);
                if(ans < 0 || VideoTime < ans) {

                    return;
                }
                //シーク中の動画時間
                CurrentTimeWidth = (ActualWidth ) / VideoTime * ans;
                SeekCursor = new Thickness(x - 5, 0, 0, 0);

            }
        }

        private void SeekBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            IsDrag = false;
            UIElement el = sender as UIElement;
            el.ReleaseMouseCapture();
        }

        private void SeekBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

            IsDrag = true;
            UIElement el = sender as UIElement;
            el.CaptureMouse();

        }
	}
}
