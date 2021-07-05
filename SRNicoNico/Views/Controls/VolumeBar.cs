using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace SRNicoNico.Views.Controls {
    /// <summary>
    /// ボリュームバーのコントロール
    /// </summary>
    public class VolumeBar : Control {
        static VolumeBar() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeBar), new FrameworkPropertyMetadata(typeof(VolumeBar)));
        }

        /// <summary>
        /// 実際の音量
        /// 0.0 から 1.0
        /// </summary>
        public float Volume {
            get { return (float)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register(nameof(Volume), typeof(float), typeof(VolumeBar), new FrameworkPropertyMetadata(0.0F, (obj, e) => {

                var bar = (VolumeBar)obj;
                // つまみ部分の横幅10論理ピクセル分を値の割合に応じて減らしつまみが100論理ピクセルを超えないように調整する
                var computedWidth = bar.Volume * 100 - 10 * bar.Volume;
                if (bar.Thumb != null) {
                    Canvas.SetLeft(bar.Thumb, computedWidth);
                }
                if (bar.ActiveRect != null) {
                    bar.ActiveRect.Width = computedWidth;
                }
                bar.SetVolumeIcon();
            }));

        /// <summary>
        /// ミュート状態かどうか
        /// </summary>
        public bool IsMute {
            get { return (bool)GetValue(IsMuteProperty); }
            set { SetValue(IsMuteProperty, value); }
        }
        public static readonly DependencyProperty IsMuteProperty =
            DependencyProperty.Register(nameof(IsMute), typeof(bool), typeof(VolumeBar), new FrameworkPropertyMetadata(false, (obj, e) => {

                var bar = (VolumeBar)obj;
                bar.SetVolumeWidth();
                bar.SetVolumeIcon();
            }));

        /// <summary>
        /// ミュートボタンのイベント
        /// </summary>
        public event RoutedEventHandler Click {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        public static RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
                                                    nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VolumeBar));

        private bool IsDragging = false;

        private Rectangle? Thumb;
        private Rectangle? ActiveRect;
        private RoundButton? MuteButton;

        public VolumeBar() {

            MouseLeftButtonDown += VolumeBar_MouseLeftButtonDown;
            MouseLeftButtonUp += VolumeBar_MouseLeftButtonUp;
            MouseMove += VolumeBar_MouseMove;
        }

        private void VolumeBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            IsDragging = true;
            ((UIElement)sender).CaptureMouse();
        }

        private void VolumeBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            IsDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
        }

        private void VolumeBar_MouseMove(object sender, MouseEventArgs e) {

            if (!IsDragging) {
                return;
            }
            if (Thumb == null) {
                return;
            }

            var x = e.GetPosition(this).X;

            var ammount = x / ActualWidth;

            if (ammount < 0) {

                Volume = 0;
            } else if (ammount > 1) {

                Volume = 1;
            } else {

                Volume = (float)ammount;
            }
        }

        /// <summary>
        /// ボリュームバーを描画する
        /// </summary>
        private void SetVolumeWidth() {

            // つまみ部分の横幅10論理ピクセル分を値の割合に応じて減らしつまみが100論理ピクセルを超えないように調整する
            var computedWidth = (Volume * 100) - (10 * Volume);
            if (Thumb != null) {
                Canvas.SetLeft(Thumb, computedWidth);
            }
            if (ActiveRect != null) {
                ActiveRect.Width = computedWidth;
            }
        }

        /// <summary>
        /// ミュートボタンのアイコンを設定する
        /// </summary>
        private void SetVolumeIcon() {

            if (MuteButton == null) {
                return;
            }

            if (IsMute) {
                MuteButton.Tag = "Mute";
                return;
            }

            var volume = Volume;
            if (volume == 0) {
                MuteButton.Tag = "0";
                return;
            }
            if (volume <= 0.33) {
                MuteButton.Tag = "1";
                return;
            }
            if (volume <= 0.66) {
                MuteButton.Tag = "2";
                return;
            }
            if (volume <= 1) {
                MuteButton.Tag = "3";
                return;
            }
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            // Generic.VolumeBar.xamlに書いてあるコントロールを取得する
            Thumb = GetTemplateChild("Thumb_PART") as Rectangle;
            ActiveRect = GetTemplateChild("ActiveRect_PART") as Rectangle;
            MuteButton = GetTemplateChild("Button_PART") as RoundButton;

            if (MuteButton != null) {
                MuteButton.Click += (o, e) => RaiseEvent(new RoutedEventArgs(ClickEvent));
            }

            // 初期値を設定する
            SetVolumeWidth();
            SetVolumeIcon();
        }
    }
}
