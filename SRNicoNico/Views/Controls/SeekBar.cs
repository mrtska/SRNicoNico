using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SRNicoNico.Views.Controls {
    /// <summary>
    /// シークバーのコントロール
    /// </summary>
    public class SeekBar : Control {
        static SeekBar() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeekBar), new FrameworkPropertyMetadata(typeof(SeekBar)));
        }

        /// <summary>
        /// 現在の再生時間
        /// </summary>
        public double CurrentTime {
            get { return (double)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(nameof(CurrentTime), typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0D, (o, e) => {

                var bar = (SeekBar)o;
                bar.CalcThumbPosition();
            }));

        /// <summary>
        /// 動画の長さ 秒以下も含む
        /// </summary>
        public double VideoDuration {
            get { return (double)GetValue(VideoDurationProperty); }
            set { SetValue(VideoDurationProperty, value); }
        }
        public static readonly DependencyProperty VideoDurationProperty =
            DependencyProperty.Register(nameof(VideoDuration), typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0D));

        /// <summary>
        /// 再生済みの時間幅のリスト
        /// </summary>
        public IEnumerable<TimeRange> PlayedRange {
            get { return (IEnumerable<TimeRange>)GetValue(PlayedRangeProperty); }
            set { SetValue(PlayedRangeProperty, value); }
        }
        public static readonly DependencyProperty PlayedRangeProperty =
            DependencyProperty.Register(nameof(PlayedRange), typeof(IEnumerable<TimeRange>), typeof(SeekBar), new FrameworkPropertyMetadata(null, (o, e) => {

                var bar = (SeekBar)o;
                if (bar.PlayedRange is INotifyCollectionChanged notify) {

                    notify.CollectionChanged += (_, e) => {
                        if (e.Action == NotifyCollectionChangedAction.Add) {
                            bar.CalcTimeRangeBounds(e.NewItems.Cast<TimeRange>());
                        }
                    };
                }
            }));

        /// <summary>
        /// バッファ済みの時間幅のリスト
        /// </summary>
        public IEnumerable<TimeRange> BufferedRange {
            get { return (IEnumerable<TimeRange>)GetValue(BufferedRangeProperty); }
            set { SetValue(BufferedRangeProperty, value); }
        }
        public static readonly DependencyProperty BufferedRangeProperty =
            DependencyProperty.Register(nameof(BufferedRange), typeof(IEnumerable<TimeRange>), typeof(SeekBar), new FrameworkPropertyMetadata(null, (o, e) => {

                var bar = (SeekBar)o;
                if (bar.BufferedRange is INotifyCollectionChanged notify) {

                    notify.CollectionChanged += (_, e) => {
                        if (e.Action == NotifyCollectionChangedAction.Add) {
                            bar.CalcTimeRangeBounds(e.NewItems.Cast<TimeRange>());
                        }
                    };
                }
            }));

        private bool IsDragging = false;
        private Rectangle? Thumb;

        public SeekBar() {

            MouseLeftButtonDown += SeekBar_MouseLeftButtonDown;
            MouseLeftButtonUp += SeekBar_MouseLeftButtonUp;
            MouseMove += SeekBar_MouseMove;

            SizeChanged += SeekBar_SizeChanged;
        }

        private void CalcThumbPosition() {

            // つまみ部分の横幅10論理ピクセル分を値の割合に応じて減らしつまみが動画の長さを超えないように調整する
            if (Thumb != null) {
                var computedWidth = ActualWidth / (VideoDuration / CurrentTime) - (10 * (CurrentTime / VideoDuration));
                Canvas.SetLeft(Thumb, computedWidth);
            }
        }

        private void CalcTimeRangeBounds(IEnumerable<TimeRange> range) {

            foreach (var played in range) {

                if (played == null) {
                    continue;
                }
                var computedLeft = ActualWidth / (VideoDuration / played.Start) - (10 * (played.Start / VideoDuration));
                var computedWidth = ActualWidth / (VideoDuration / played.End) - (5 * (played.End / VideoDuration));
                played.Left = computedLeft;
                played.Width = computedWidth - computedLeft;
            }
        }

        private void SeekBar_SizeChanged(object sender, SizeChangedEventArgs e) {

            CalcThumbPosition();
        }

        private void SeekBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            IsDragging = true;
            ((UIElement)sender).CaptureMouse();
        }

        private void SeekBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            IsDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
        }

        private void SeekBar_MouseMove(object sender, MouseEventArgs e) {

            if (!IsDragging) {
                return;
            }
            if (Thumb == null) {
                return;
            }

            var x = e.GetPosition(this).X;

            var ammount = x / ActualWidth;

            //if (ammount < 0) {

            //    Volume = 0;
            //} else if (ammount > 1) {

            //    Volume = 1;
            //} else {

            //    Volume = (float)ammount;
            //}
        }


        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            // Generic.SeekBar.xamlに書いてあるコントロールを取得する
            Thumb = GetTemplateChild("Thumb_PART") as Rectangle;
        }

    }
    /// <summary>
    /// 時間のレンジを管理する構造体
    /// </summary>
    public class TimeRange {

        public float Start { get; }
        public float End { get; }

        public double Left { get; set; }
        public double Width { get; set; }

        public TimeRange(float start, float end) {

            Start = start;
            End = end;
            Left = 0;
            Width = 0;
        }
    }
}
