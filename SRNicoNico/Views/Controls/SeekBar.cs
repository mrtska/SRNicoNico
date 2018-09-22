using Livet;
using SRNicoNico.Models.NicoNicoWrapper;
using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace SRNicoNico.Views.Controls {

    public class SeekBar : Control {

        static SeekBar() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeekBar), new FrameworkPropertyMetadata(typeof(SeekBar)));
        }

        public bool IsPopupOpen {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(SeekBar), new FrameworkPropertyMetadata(false));

        public int PopupTime {
            get { return (int)GetValue(PopupTimeProperty); }
            set { SetValue(PopupTimeProperty, value); }
        }

        public static readonly DependencyProperty PopupTimeProperty =
            DependencyProperty.Register("PopupTime", typeof(int), typeof(SeekBar), new PropertyMetadata(0));

        public double CurrentTime {
            get { return (double)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(double), typeof(SeekBar), new PropertyMetadata(0.0D, (obj, e) => {

                var bar = obj as SeekBar;
                if (bar.Thumb == null) {

                    return;
                }

                if (bar.IsDragging) {

                    Canvas.SetLeft(bar.Thumb, bar.ActualWidth / (bar.VideoTime / bar.RequestSeekPos) - (10 * (bar.RequestSeekPos / bar.VideoTime)));
                } else {

                    Canvas.SetLeft(bar.Thumb, bar.ActualWidth / (bar.VideoTime / bar.CurrentTime) - (10 * (bar.CurrentTime / bar.VideoTime)));
                }
            }));

        public double VideoTime {
            get { return (double)GetValue(VideoTimeProperty); }
            set { SetValue(VideoTimeProperty, value); }
        }

        public static readonly DependencyProperty VideoTimeProperty =
            DependencyProperty.Register("VideoTime", typeof(double), typeof(SeekBar), new PropertyMetadata(0.0D));

        public Rect PopupTextRect {
            get { return (Rect)GetValue(PopupTextRectProperty); }
            set { SetValue(PopupTextRectProperty, value); }
        }

        public static readonly DependencyProperty PopupTextRectProperty =
            DependencyProperty.Register("PopupTextRect", typeof(Rect), typeof(SeekBar), new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.NotDataBindable));

        public ObservableSynchronizedCollection<TimeRange> PlayedRange {
            get { return (ObservableSynchronizedCollection<TimeRange>)GetValue(PlayedRangeProperty); }
            set { SetValue(PlayedRangeProperty, value); }
        }

        public static readonly DependencyProperty PlayedRangeProperty =
            DependencyProperty.Register("PlayedRange", typeof(ObservableSynchronizedCollection<TimeRange>), typeof(SeekBar), new PropertyMetadata(null));

        public ObservableSynchronizedCollection<TimeRange> BufferedRange {
            get { return (ObservableSynchronizedCollection<TimeRange>)GetValue(BufferedRangeProperty); }
            set { SetValue(BufferedRangeProperty, value); }
        }

        public static readonly DependencyProperty BufferedRangeProperty =
            DependencyProperty.Register("BufferedRange", typeof(ObservableSynchronizedCollection<TimeRange>), typeof(SeekBar), new PropertyMetadata(null));


        public bool IsStoryBoardOpen {
            get { return (bool)GetValue(IsStoryBoardOpenProperty); }
            set { SetValue(IsStoryBoardOpenProperty, value); }
        }

        public static readonly DependencyProperty IsStoryBoardOpenProperty =
            DependencyProperty.Register(nameof(IsStoryBoardOpen), typeof(bool), typeof(SeekBar), new FrameworkPropertyMetadata(false));


        public Rect StoryBoardBitmapRect {
            get { return (Rect)GetValue(StoryBoardBitmapRectProperty); }
            set { SetValue(StoryBoardBitmapRectProperty, value); }
        }

        public static readonly DependencyProperty StoryBoardBitmapRectProperty =
            DependencyProperty.Register("StoryBoardBitmapRect", typeof(Rect), typeof(SeekBar), new FrameworkPropertyMetadata(Rect.Empty));


        public NicoNicoStoryBoard StoryBoard {
            get { return (NicoNicoStoryBoard)GetValue(StoryBoardDataProperty); }
            set { SetValue(StoryBoardDataProperty, value); }
        }

        public static readonly DependencyProperty StoryBoardDataProperty =
            DependencyProperty.Register("StoryBoard", typeof(NicoNicoStoryBoard), typeof(SeekBar), new PropertyMetadata(null));

        public BitmapSource StoryBoardImage {
            get { return (BitmapSource)GetValue(StoryBoardImageProperty); }
            set { SetValue(StoryBoardImageProperty, value); }
        }

        public static readonly DependencyProperty StoryBoardImageProperty =
            DependencyProperty.Register("StoryBoardImage", typeof(BitmapSource), typeof(SeekBar), new PropertyMetadata(null));

        public event SeekRequestedHandler SeekRequested;

        public SeekBar() {

            PreviewMouseLeftButtonDown += SeekBar_PreviewMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += SeekBar_PreviewMouseLeftButtonUp;
            PreviewMouseMove += SeekBar_PreviewMouseMove;

            MouseEnter += SeekBar_MouseEnter;
            MouseLeave += SeekBar_MouseLeave;
        }

        private void SeekBar_MouseEnter(object sender, MouseEventArgs e) {

            IsPopupOpen = true;
        }

        private void SeekBar_MouseLeave(object sender, MouseEventArgs e) {

            IsPopupOpen = false;
            IsStoryBoardOpen = false;
        }

        private Thumb Thumb;

        private bool IsDragging = false;

        public double RequestSeekPos {
            get { return (double)GetValue(RequestSeekPosProperty); }
            set { SetValue(RequestSeekPosProperty, value); }
        }
        public static readonly DependencyProperty RequestSeekPosProperty =
            DependencyProperty.Register("RequestSeekPos", typeof(double), typeof(SeekBar), new PropertyMetadata(0.0D));

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void SeekBar_PreviewMouseMove(object sender, MouseEventArgs e) {

            var x = e.GetPosition(this).X;
            var amount = (x / ActualWidth * VideoTime);
            var iamount = (int)amount;

            if(amount <= VideoTime && amount >= 0) {

                PopupTextRect = new Rect(x - 5, 0, 20, 20);
                PopupTime = (int)amount;

                if(StoryBoard != null) {

                    if(StoryBoard.Bitmap.ContainsKey(iamount - (iamount % StoryBoard.Interval))) {

                        IsStoryBoardOpen = true;
                        StoryBoardBitmapRect = new Rect(x - StoryBoard.Width / 2, 0, StoryBoard.Width, StoryBoard.Height);

                        var test = StoryBoard.Bitmap[iamount - (iamount % StoryBoard.Interval)];
                        var hBitMap = test.GetHbitmap();
                        try {

                            StoryBoardImage = Imaging.CreateBitmapSourceFromHBitmap(hBitMap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        } finally {

                            DeleteObject(hBitMap);
                        }
                    }
                }
            }
            if(IsDragging) {

                if(amount < 0) {

                    RequestSeekPos = 0;
                } else if(VideoTime < amount) {

                    RequestSeekPos = VideoTime;
                } else {

                    RequestSeekPos = amount;
                }
            } else {

            }
        }

        private void SeekBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            IsDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
            SeekRequested(this, new SeekRequestedEventArgs(RequestSeekPos));
        }

        private void SeekBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

            IsDragging = true;
            ((UIElement)sender).CaptureMouse();
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            Thumb = GetTemplateChild("Thumb_PART") as Thumb;
        }
    }
}
