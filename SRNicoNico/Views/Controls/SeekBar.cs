using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.ViewModels;

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
                if (bar.IsVisible) {
                    bar.CalcThumbPosition();
                    bar.RepeatCheck();
                }
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
            DependencyProperty.Register(nameof(PlayedRange), typeof(IEnumerable<TimeRange>), typeof(SeekBar), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// バッファ済みの時間幅のリスト
        /// </summary>
        public IEnumerable<TimeRange> BufferedRange {
            get { return (IEnumerable<TimeRange>)GetValue(BufferedRangeProperty); }
            set { SetValue(BufferedRangeProperty, value); }
        }
        public static readonly DependencyProperty BufferedRangeProperty =
            DependencyProperty.Register(nameof(BufferedRange), typeof(IEnumerable<TimeRange>), typeof(SeekBar), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 実際に動画をシークしたい時に呼ぶ
        /// </summary>
        public Action<double> SeekAction {
            get { return (Action<double>)GetValue(SeekActionProperty); }
            set { SetValue(SeekActionProperty, value); }
        }
        public static readonly DependencyProperty SeekActionProperty =
            DependencyProperty.Register(nameof(SeekAction), typeof(Action<double>), typeof(SeekBar), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ストーリーボードの画像のマップ
        /// </summary>
        public VideoStoryBoard StoryBoard {
            get { return (VideoStoryBoard)GetValue(StoryBoardProperty); }
            set { SetValue(StoryBoardProperty, value); }
        }
        public static readonly DependencyProperty StoryBoardProperty =
            DependencyProperty.Register(nameof(StoryBoard), typeof(VideoStoryBoard), typeof(SeekBar), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// リピート動作
        /// </summary>
        public RepeatBehavior RepeatBehavior {
            get { return (RepeatBehavior)GetValue(RepeatBehaviorProperty); }
            set { SetValue(RepeatBehaviorProperty, value); }
        }
        public static readonly DependencyProperty RepeatBehaviorProperty =
            DependencyProperty.Register(nameof(RepeatBehavior), typeof(RepeatBehavior), typeof(SeekBar), new FrameworkPropertyMetadata(RepeatBehavior.None, (obj, e) => {

                var bar = (SeekBar)obj;
                if (bar.IsVisible) {
                    if (bar.RepeatBehavior == RepeatBehavior.ABRepeat) {

                        bar.RepeatARect!.Visibility = Visibility.Visible;
                        bar.RepeatBRect!.Visibility = Visibility.Visible;
                    } else {

                        bar.RepeatARect!.Visibility = Visibility.Collapsed;
                        bar.RepeatBRect!.Visibility = Visibility.Collapsed;
                    }
                }
            }));

        /// <summary>
        /// ABリピートのA
        /// </summary>
        public double RepeatA {
            get { return (double)GetValue(RepeatAProperty); }
            set { SetValue(RepeatAProperty, value); }
        }
        public static readonly DependencyProperty RepeatAProperty =
            DependencyProperty.Register(nameof(RepeatA), typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(0D, (obj, e) => {

                var bar = (SeekBar)obj;
                if (bar.IsMeasureValid) {
                    bar.MoveRepeatA(bar.RepeatA);
                }
            }));

        /// <summary>
        /// ABリピートのB
        /// </summary>
        public double RepeatB {
            get { return (double)GetValue(RepeatBProperty); }
            set { SetValue(RepeatBProperty, value); }
        }
        public static readonly DependencyProperty RepeatBProperty =
            DependencyProperty.Register(nameof(RepeatB), typeof(double), typeof(SeekBar), new FrameworkPropertyMetadata(5D, (obj, e) => {

                var bar = (SeekBar)obj;
                if (bar.IsMeasureValid) {
                    bar.MoveRepeatB(bar.RepeatB);
                }
            }));

        private readonly Converters.DurationConverter DurationConverter = (Converters.DurationConverter)Application.Current.Resources["DurationConverter"];

        private double RequestSeekPosition = 0;
        private bool IsDragging = false;
        private Rectangle? Thumb;
        private Image? StoryBoardImage;
        private Popup? StoryBoardPopup;
        private TextBlock? PopupText;
        private Popup? ToolTipPopup;

        private Rectangle? RepeatARect;
        private Rectangle? RepeatBRect;
        private Popup? RepeatAPopup;
        private Popup? RepeatBPopup;
        private TextBlock? RepeatAPopupText;
        private TextBlock? RepeatBPopupText;

        public SeekBar() {

            Loaded += (o, e) => {

                MouseLeftButtonDown += SeekBar_MouseLeftButtonDown;
                MouseLeftButtonUp += SeekBar_MouseLeftButtonUp;
                MouseMove += SeekBar_MouseMove;
                MouseLeave += SeekBar_MouseLeave;
                SizeChanged += SeekBar_SizeChanged;
            };

            Unloaded += (o, e) => {

                MouseLeftButtonDown -= SeekBar_MouseLeftButtonDown;
                MouseLeftButtonUp -= SeekBar_MouseLeftButtonUp;
                MouseMove -= SeekBar_MouseMove;
                MouseLeave -= SeekBar_MouseLeave;
                SizeChanged -= SeekBar_SizeChanged;
            };
        }

        /// <summary>
        /// リピート動作を確認して必要ならシークする
        /// </summary>
        private void RepeatCheck() {

            var behavior = RepeatBehavior;

            if (behavior == RepeatBehavior.None) {
                return;
            }
            var currentTime = CurrentTime;

            if (behavior == RepeatBehavior.Repeat && currentTime == VideoDuration) {

                SeekAction?.Invoke(0);
                return;
            }
            // ABリピート時
            if (behavior == RepeatBehavior.ABRepeat && currentTime >= RepeatB) {
                // Aへシークする
                SeekAction?.Invoke(RepeatA);
            }
        }

        private void CalcThumbPosition() {

            // つまみ部分の横幅10論理ピクセル分を値の割合に応じて減らしつまみが動画の長さを超えないように調整する
            var target = IsDragging ? RequestSeekPosition : CurrentTime;
            var computedWidth = ActualWidth / (VideoDuration / target) - (10 * (target / VideoDuration));
            Canvas.SetLeft(Thumb, computedWidth);
        }

        private void SetSeekPosition(double amount) {

            if (amount < 0) {

                RequestSeekPosition = 0;
            } else if (VideoDuration < amount) {

                RequestSeekPosition = VideoDuration;
            } else {

                RequestSeekPosition = amount;
            }
        }

        private void MoveStoryBoard(double x) {

            // ストーリーボードを表示する
            if (StoryBoard != null) {

                var amount = x / ActualWidth * VideoDuration;
                if (amount > VideoDuration || amount < 0) {
                    return;
                }
                StoryBoardPopup!.PlacementRectangle = new Rect(x - StoryBoard.ThumbnailWidth / 2, 0, StoryBoard.ThumbnailWidth, StoryBoard.ThumbnailHeight);
                StoryBoardPopup.IsOpen = true;

                int iamount = (int)amount * 1000;
                int location = iamount - (iamount % (StoryBoard.Interval));

                if (StoryBoard.BitmapMap!.ContainsKey(location)) {

                    var hBitmap = StoryBoard.BitmapMap[location].GetHbitmap();
                    try {
                        StoryBoardImage!.Source = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    } catch {
                    } finally {
                        DeleteObject(hBitmap);
                    }
                }
            }
        }

        private void MoveABRepeatTooltip(bool showA, bool showB, double? apos = null, double? bpos = null) {

            if (apos == null) {
                apos = RepeatA;
            }
            if (bpos == null) {
                bpos = RepeatB;
            }

            if (RepeatBehavior == RepeatBehavior.ABRepeat) {

                if (showA) {

                    RepeatAPopupText!.Text = DurationConverter.Convert(apos, null!, null!, null!).ToString();
                    RepeatAPopup!.PlacementRectangle = new Rect(ActualWidth / (VideoDuration / apos.Value) - (10 * (apos.Value / VideoDuration)) - 10, 0, 20, 20);
                    RepeatAPopup.IsOpen = true;
                }

                if (showB) {

                    RepeatBPopupText!.Text = DurationConverter.Convert(bpos, null!, null!, null!).ToString();
                    RepeatBPopup!.PlacementRectangle = new Rect(ActualWidth / (VideoDuration / bpos.Value) - (10 * (bpos.Value / VideoDuration)) - 10, 0, 20, 20);
                    RepeatBPopup.IsOpen = true;
                }
            }
        }

        private void MoveRepeatA(double amount) {

            var computedWidth = ActualWidth / (VideoDuration / amount) - (10 * (amount / VideoDuration));
            Canvas.SetLeft(RepeatARect, computedWidth);
        }
        private void MoveRepeatB(double amount) {

            var computedWidth = ActualWidth / (VideoDuration / amount) - (10 * (amount / VideoDuration));
            Canvas.SetLeft(RepeatBRect, computedWidth);
        }

        private void SeekBar_SizeChanged(object sender, SizeChangedEventArgs e) {

            if (IsVisible) {
                CalcThumbPosition();
                MoveRepeatA(RepeatA);
                MoveRepeatB(RepeatB);
            }
        }

        private void SeekBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            IsDragging = true;
            CaptureMouse();
            Focus();
            e.Handled = true;
        }

        private void SeekBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            IsDragging = false;

            e.Handled = true;
            ReleaseMouseCapture();

            var x = e.GetPosition(this).X;

            var amount = x / ActualWidth * VideoDuration;
            SetSeekPosition(amount);
            SeekAction?.Invoke(RequestSeekPosition);
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private void SeekBar_MouseMove(object sender, MouseEventArgs e) {

            var x = e.GetPosition(this).X;
            var amount = x / ActualWidth * VideoDuration;

            // ポップアップが場外に行かないよう制御する
            if (amount > VideoDuration || amount < 0) {
                return;
            }

            // マウスオーバーしている部分の秒数を表示する
            ToolTipPopup!.PlacementRectangle = new Rect(x - 5, 0, 20, 20);
            ToolTipPopup.IsOpen = true;
            PopupText!.Text = DurationConverter.Convert(amount, null!, null!, null!).ToString();

            MoveABRepeatTooltip(true, true);

            MoveStoryBoard(x);

            if (!IsDragging) {
                return;
            }
            if (Thumb == null) {
                return;
            }

            SetSeekPosition(amount);
            SeekAction?.Invoke(RequestSeekPosition);
        }

        private void SeekBar_MouseLeave(object sender, MouseEventArgs e) {

            if (StoryBoard != null) {
                StoryBoardPopup!.IsOpen = false;
            }
            ToolTipPopup!.IsOpen = false;
            RepeatAPopup!.IsOpen = false;
            RepeatBPopup!.IsOpen = false;
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            // Generic.SeekBar.xamlに書いてあるコントロールを取得する
            Thumb = GetTemplateChild("Thumb_PART") as Rectangle;
            StoryBoardImage = GetTemplateChild("StoryBoard_PART") as Image;
            StoryBoardPopup = GetTemplateChild("StoryBoardPopup_PART") as Popup;
            PopupText = GetTemplateChild("PopupText_PART") as TextBlock;
            ToolTipPopup = GetTemplateChild("Popup_PART") as Popup;

            RepeatARect = GetTemplateChild("RepeatA_PART") as Rectangle;
            RepeatBRect = GetTemplateChild("RepeatB_PART") as Rectangle;

            RepeatAPopup = GetTemplateChild("RepeatAPopup_PART") as Popup;
            RepeatBPopup = GetTemplateChild("RepeatBPopup_PART") as Popup;
            RepeatAPopupText = GetTemplateChild("RepeatAPopupText_PART") as TextBlock;
            RepeatBPopupText = GetTemplateChild("RepeatBPopupText_PART") as TextBlock;


            if (Thumb == null || StoryBoardImage == null || StoryBoardPopup == null || PopupText == null || ToolTipPopup == null ||
                RepeatARect == null || RepeatBRect == null || RepeatAPopup == null || RepeatBPopup == null || RepeatAPopupText == null || RepeatBPopupText == null) {

                throw new InvalidOperationException("テンプレートが不正です");
            }

            RepeatARect.MouseLeftButtonDown += (o, e) => {

                var rect = (Rectangle)o;
                rect.CaptureMouse();
                e.Handled = true;
            };
            RepeatARect.MouseMove += (o, e) => {

                var rect = (Rectangle)o;
                e.Handled = true;

                ToolTipPopup.IsOpen = false;
                if (!rect.IsMouseCaptured) {

                    MoveABRepeatTooltip(true, true);
                    return;
                }

                var x = e.GetPosition(this).X;
                var amount = x / ActualWidth * VideoDuration;

                // リピートAはリピートB - 5秒より後にならないようにする
                if (amount < 0) {
                    amount = 0;
                } else if (amount > RepeatB - 5) {
                    amount = RepeatB - 5;
                }

                MoveABRepeatTooltip(true, true, amount);
                MoveStoryBoard(x);
                MoveRepeatA(amount);
            };
            RepeatARect.MouseLeftButtonUp += (o, e) => {

                var rect = (Rectangle)o;
                if (rect.IsMouseCaptured) {
                    rect.ReleaseMouseCapture();
                    e.Handled = true;
                }

                var x = e.GetPosition(this).X;
                var amount = x / ActualWidth * VideoDuration;
                // リピートAはリピートB - 5秒より後にならないようにする
                if (amount < 0) {
                    amount = 0;
                } else if (amount > RepeatB - 5) {
                    amount = RepeatB - 5;
                }

                RepeatA = amount;

                RepeatAPopup!.IsOpen = false;
                RepeatBPopup!.IsOpen = false;
            };

            RepeatBRect.MouseLeftButtonDown += (o, e) => {

                var rect = (Rectangle)o;
                rect.CaptureMouse();
                e.Handled = true;
            };
            RepeatBRect.MouseMove += (o, e) => {

                var rect = (Rectangle)o;
                e.Handled = true;

                ToolTipPopup.IsOpen = false;
                if (!rect.IsMouseCaptured) {

                    MoveABRepeatTooltip(true, true);
                    return;
                }

                var x = e.GetPosition(this).X;
                var amount = x / ActualWidth * VideoDuration;

                // リピートBはリピートA＋5秒より前にならないようにする
                if (amount <= RepeatA + 5) {
                    amount = RepeatA + 5;
                } else if (amount > VideoDuration) {
                    amount = VideoDuration;
                }

                MoveABRepeatTooltip(true, true, null, amount);
                MoveStoryBoard(x);
                MoveRepeatB(amount);
            };
            RepeatBRect.MouseLeftButtonUp += (o, e) => {

                var rect = (Rectangle)o;
                if (rect.IsMouseCaptured) {
                    rect.ReleaseMouseCapture();
                    e.Handled = true;

                    var x = e.GetPosition(this).X;
                    var amount = x / ActualWidth * VideoDuration;
                    // リピートBはリピートA＋5秒より前にならないようにする
                    if (amount <= RepeatA + 5) {
                        amount = RepeatA + 5;
                    } else if (amount > VideoDuration) {
                        amount = VideoDuration;
                    }

                    RepeatB = amount;

                    RepeatAPopup!.IsOpen = false;
                    RepeatBPopup!.IsOpen = false;
                }
            };
        }
    }

    /// <summary>
    /// シークバーのバッファ済み領域や再生済み領域を描画するコントロール
    /// </summary>
    public class SeekBarBackground : Control {
        static SeekBarBackground() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SeekBarBackground), new FrameworkPropertyMetadata(typeof(SeekBarBackground)));
        }

        /// <summary>
        /// 再生済みの時間幅のリスト
        /// </summary>
        public IEnumerable<TimeRange> Range {
            get { return (IEnumerable<TimeRange>)GetValue(PlayedProperty); }
            set { SetValue(PlayedProperty, value); }
        }
        public static readonly DependencyProperty PlayedProperty =
            DependencyProperty.Register(nameof(Range), typeof(IEnumerable<TimeRange>), typeof(SeekBarBackground), new FrameworkPropertyMetadata(null));

        public SeekBarBackground() {

            Loaded += (o, e) => {
                if (Range is INotifyCollectionChanged collection) {
                    collection.CollectionChanged += Collection_CollectionChanged;
                }
            };
            Unloaded += (o, e) => {
                if (Range is INotifyCollectionChanged collection) {
                    collection.CollectionChanged -= Collection_CollectionChanged;
                }
            };
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);

            if (Range == null) {
                return;
            }

            foreach (var range in Range) {

                var x = range.Start * ActualWidth;
                drawingContext.DrawRectangle(Background, null, new Rect(x, 0, range.End * ActualWidth - x, ActualHeight));
            }
        }
    }
    /// <summary>
    /// リピート動作
    /// </summary>
    public enum RepeatBehavior {
        /// <summary>
        /// リピートなし
        /// </summary>
        None,
        /// <summary>
        /// 通常リピート
        /// </summary>
        Repeat,
        /// <summary>
        /// ABリピート
        /// </summary>
        ABRepeat
    }
}
