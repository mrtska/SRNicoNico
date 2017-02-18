using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

using Livet;
using System.Collections.Specialized;
using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace SRNicoNico.Views.Controls {
 
    public class SeekBar : Control {

        static SeekBar() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeekBar), new FrameworkPropertyMetadata(typeof(SeekBar)));
        }



        public bool IsPopupOpen {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPopupOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(SeekBar), new FrameworkPropertyMetadata(false));




        public int PopupTime {
            get { return (int)GetValue(PopupTimeProperty); }
            set { SetValue(PopupTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupTimeProperty =
            DependencyProperty.Register("PopupTime", typeof(int), typeof(SeekBar), new PropertyMetadata(0));





        public double CurrentTime {
            get { return (double)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(double), typeof(SeekBar), new PropertyMetadata(0.0D, (obj, e) => {


                var bar = obj as SeekBar;

                if(bar.Thumb == null) {

                    return;
                }

                if(bar.IsDragging) {


                    Canvas.SetLeft(bar.Thumb, bar.ActualWidth / (bar.VideoTime / bar.RequestSeekPos) - (10 * (bar.RequestSeekPos / bar.VideoTime)));
                } else {


                    Canvas.SetLeft(bar.Thumb, bar.ActualWidth / (bar.VideoTime / bar.CurrentTime) - (10 * (bar.CurrentTime / bar.VideoTime)));
                }
                
            }));



        public double VideoTime {
            get { return (double)GetValue(VideoTimeProperty); }
            set { SetValue(VideoTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VideoTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoTimeProperty =
            DependencyProperty.Register("VideoTime", typeof(double), typeof(SeekBar), new PropertyMetadata(0.0D));




        public Rect PopupTextRect {
            get { return (Rect)GetValue(PopupTextRectProperty); }
            set { SetValue(PopupTextRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupTextRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupTextRectProperty =
            DependencyProperty.Register("PopupTextRect", typeof(Rect), typeof(SeekBar), new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.NotDataBindable));




        public DispatcherCollection<TimeRange> PlayedRange {
            get { return (DispatcherCollection<TimeRange>)GetValue(PlayedRangeProperty); }
            set { SetValue(PlayedRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayedRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayedRangeProperty =
            DependencyProperty.Register("PlayedRange", typeof(DispatcherCollection<TimeRange>), typeof(SeekBar), new PropertyMetadata(null, (obj, e) => {

                var bar = (SeekBar)obj;

                var collection = bar.PlayedRange;
                collection.CollectionChanged += (o, ev) => {

                    bar.AdjustTimeRange(ev);
                };

            }));



        public DispatcherCollection<TimeRange> BufferedRange {
            get { return (DispatcherCollection<TimeRange>)GetValue(BufferedRangeProperty); }
            set { SetValue(BufferedRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BufferedRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BufferedRangeProperty =
            DependencyProperty.Register("BufferedRange", typeof(DispatcherCollection<TimeRange>), typeof(SeekBar), new PropertyMetadata(null, (obj, e) => {

                var bar = (SeekBar)obj;

                var collection = bar.BufferedRange;
                collection.CollectionChanged += (o, ev) => {

                    bar.AdjustTimeRange(ev);
                };
            }));


        private void AdjustTimeRange(NotifyCollectionChangedEventArgs e) {

            if(e.Action == NotifyCollectionChangedAction.Add) {

                foreach(var item in e.NewItems) {

                    if(item is TimeRange) {

                        var time = (TimeRange)item;

                        var temp = VideoTime / (time.EndTime - time.StartTime);

                        time.Start = ActualWidth / (VideoTime / time.StartTime);
                        time.Width = ActualWidth / temp;
                    }
                }

                Canvas.SetLeft(Thumb, ActualWidth / (VideoTime / CurrentTime) - (10 * (CurrentTime / VideoTime)));
            }
        }





        public bool IsStoryBoardOpen {
            get { return (bool)GetValue(IsStoryBoardOpenProperty); }
            set { SetValue(IsStoryBoardOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsStoryBoardOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStoryBoardOpenProperty =
            DependencyProperty.Register(nameof(IsStoryBoardOpen), typeof(bool), typeof(SeekBar), new FrameworkPropertyMetadata(false));


        public Rect StoryBoardBitmapRect {
            get { return (Rect)GetValue(StoryBoardBitmapRectProperty); }
            set { SetValue(StoryBoardBitmapRectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StoryBoardBitmapRect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StoryBoardBitmapRectProperty =
            DependencyProperty.Register("StoryBoardBitmapRect", typeof(Rect), typeof(SeekBar), new FrameworkPropertyMetadata(Rect.Empty));


        public NicoNicoStoryBoardData StoryBoardData {
            get { return (NicoNicoStoryBoardData)GetValue(StoryBoardDataProperty); }
            set { SetValue(StoryBoardDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StoryBoardList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StoryBoardDataProperty =
            DependencyProperty.Register("StoryBoardData", typeof(NicoNicoStoryBoardData), typeof(SeekBar), new PropertyMetadata(null));




        public BitmapSource StoryBoardImage {
            get { return (BitmapSource)GetValue(StoryBoardImageProperty); }
            set { SetValue(StoryBoardImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StoryBoardImage.  This enables animation, styling, binding, etc...
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

        private double RequestSeekPos;

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void SeekBar_PreviewMouseMove(object sender, MouseEventArgs e) {

            var x = e.GetPosition(this).X;
            var amount = (x / ActualWidth * VideoTime);
            var iamount = (int)amount;

            if(amount <= VideoTime && amount >= 0) {

                PopupTextRect = new Rect(x - 5, 0, 20, 20);
                PopupTime = (int)amount;

                if(StoryBoardData != null) {

                    if(StoryBoardData.BitmapCollection.ContainsKey(iamount - (iamount % StoryBoardData.Interval))) {

                        IsStoryBoardOpen = true;
                        StoryBoardBitmapRect = new Rect(x - StoryBoardData.Width / 2, 0, StoryBoardData.Width, StoryBoardData.Height);

                        var test = StoryBoardData.BitmapCollection[iamount - (iamount % StoryBoardData.Interval)];
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
