using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SRNicoNico.Views.Controls {
    /// <summary>
    /// 丸いボタンのUIコントロール
    /// </summary>
    public class RoundButton : Button {

        static RoundButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundButton), new FrameworkPropertyMetadata(typeof(RoundButton)));
        }

        /// <summary>
        /// 円の大きさ
        /// </summary>
        public double EllipseDiameter {
            get { return (double)GetValue(EllipseDiameterProperty); }
            set { SetValue(EllipseDiameterProperty, value); }
        }
        public static readonly DependencyProperty EllipseDiameterProperty =
            DependencyProperty.Register(nameof(EllipseDiameter), typeof(double), typeof(RoundButton), new PropertyMetadata(25.0D));

        /// <summary>
        /// 円の線の太さ
        /// </summary>
        public double EllipseStrokeThickness {
            get { return (double)GetValue(EllipseStrokeThicknessProperty); }
            set { SetValue(EllipseStrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty EllipseStrokeThicknessProperty =
            DependencyProperty.Register(nameof(EllipseStrokeThickness), typeof(double), typeof(RoundButton), new PropertyMetadata(1.0D));

        /// <summary>
        /// 円の中央に描画されるアイコン
        /// </summary>
        public Geometry IconData {
            get { return (Geometry)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }
        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(RoundButton));

        /// <summary>
        /// アイコンの高さ
        /// </summary>
        public double IconHeight {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register(nameof(IconHeight), typeof(double), typeof(RoundButton), new PropertyMetadata(12.0D));

        /// <summary>
        /// アイコンの横幅
        /// </summary>
        public double IconWidth {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register(nameof(IconWidth), typeof(double), typeof(RoundButton), new PropertyMetadata(12.0D));
    }
}
