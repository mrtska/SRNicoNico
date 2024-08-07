using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SRNicoNico.Views.Controls {
    /// <summary>
    /// UWPのToggleSwitchをWPFで使う
    /// </summary>
    public class ToggleSwitch : Control {

        static ToggleSwitch() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
        }

        /// <summary>
        /// スイッチがONかOFFかどうか
        /// </summary>
        public bool IsOn {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(ToggleSwitch), new FrameworkPropertyMetadata(false));

        private Grid? RootGrid;

        private void MouseLeftButtonDownHandler(object sender, MouseButtonEventArgs e) {

            // GridがクリックされたらIsOnを反転する
            IsOn ^= true;
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            if (RootGrid != null) {

                RootGrid.MouseLeftButtonDown -= MouseLeftButtonDownHandler;
            }

            RootGrid = (Grid) GetTemplateChild("PART_Grid");
            RootGrid.MouseLeftButtonDown += MouseLeftButtonDownHandler;
        }
    }
}
