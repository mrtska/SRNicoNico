using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SRNicoNico.Views.Controls {
    public class RoundButton : Button {
        static RoundButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundButton), new FrameworkPropertyMetadata(typeof(RoundButton)));
        }

        public double EllipseDiameter {
            get { return (double)GetValue(EllipseDiameterProperty); }
            set { SetValue(EllipseDiameterProperty, value); }
        }
        public static readonly DependencyProperty EllipseDiameterProperty =
            DependencyProperty.Register("EllipseDiameter", typeof(double), typeof(RoundButton), new PropertyMetadata(25.0D));

        public double EllipseStrokeThickness {
            get { return (double)GetValue(EllipseStrokeThicknessProperty); }
            set { SetValue(EllipseStrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty EllipseStrokeThicknessProperty =
            DependencyProperty.Register("EllipseStrokeThickness", typeof(double), typeof(RoundButton), new PropertyMetadata(1.0D));

        public Geometry IconData {
            get { return (Geometry)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }
        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register("IconData", typeof(Geometry), typeof(RoundButton));

        public double IconHeight {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(RoundButton), new PropertyMetadata(12.0D));

        public double IconWidth {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(RoundButton), new PropertyMetadata(12.0D));
    }
}
