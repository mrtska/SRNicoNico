using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SRNicoNico.Views.Controls {

    public class VolumeBar : Control {
        static VolumeBar() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VolumeBar), new FrameworkPropertyMetadata(typeof(VolumeBar)));
        }

        public double ThumbPos {
            get { return (double)GetValue(ThumbPosProperty); }
            set { SetValue(ThumbPosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThumbPos.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThumbPosProperty =
            DependencyProperty.Register("ThumbPos", typeof(double), typeof(VolumeBar), new PropertyMetadata(0.0D));

        public double Volume {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volume.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(VolumeBar), new FrameworkPropertyMetadata(50.0D, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (obj, e) => {

                var bar = obj as VolumeBar;

                bar.ThumbPos = bar.Volume - (10 * (bar.Volume / 100));

            }));

        public bool IsMute {
            get { return (bool)GetValue(IsMuteProperty); }
            set { SetValue(IsMuteProperty, value); }
        }

        public bool IsPopupOpen {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPopupOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register("IsPopupOpen", typeof(bool), typeof(VolumeBar), new PropertyMetadata(false));

        // Using a DependencyProperty as the backing store for IsMute.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMuteProperty =
            DependencyProperty.Register("IsMute", typeof(bool), typeof(VolumeBar), new PropertyMetadata(false));

        public VolumeBar() {

            PreviewMouseLeftButtonDown += VolumeBar_PreviewMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += VolumeBar_PreviewMouseLeftButtonUp;
            PreviewMouseMove += VolumeBar_PreviewMouseMove;

            MouseEnter += VolumeBar_MouseEnter;
            MouseLeave += VolumeBar_MouseLeave;
        }

        private void VolumeBar_MouseEnter(object sender, MouseEventArgs e) {

            IsPopupOpen = true;
        }

        private void VolumeBar_MouseLeave(object sender, MouseEventArgs e) {

            IsPopupOpen = false;
        }

        private Thumb Thumb;

        private bool IsDragging = false;

        private void VolumeBar_PreviewMouseMove(object sender, MouseEventArgs e) {

            if(IsDragging) {

                var x = e.GetPosition(this).X;

                var ammount = (x / ActualWidth * 100);

                if(ammount < 0) {

                    Volume = 0;
                } else if(ammount > 100) {

                    Volume = 100;
                } else {

                    Volume = ammount;
                }
            }
        }

        private void VolumeBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            IsDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
        }

        private void VolumeBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

            IsDragging = true;
            ((UIElement)sender).CaptureMouse();
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            Thumb = GetTemplateChild("Thumb_PART") as Thumb;
        }
    }
}
