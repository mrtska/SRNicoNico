using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SRNicoNico.Views.Controls {
    public class StateRoundButton : Button {
        static StateRoundButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StateRoundButton), new FrameworkPropertyMetadata(typeof(StateRoundButton)));
        }

        public bool State {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(StateRoundButton), new FrameworkPropertyMetadata(false));

        public double EllipseDiameter {
            get { return (double)GetValue(EllipseDiameterProperty); }
            set { SetValue(EllipseDiameterProperty, value); }
        }
        public static readonly DependencyProperty EllipseDiameterProperty =
            DependencyProperty.Register("EllipseDiameter", typeof(double), typeof(StateRoundButton), new PropertyMetadata(25.0D));

        public double EllipseStrokeThickness {
            get { return (double)GetValue(EllipseStrokeThicknessProperty); }
            set { SetValue(EllipseStrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty EllipseStrokeThicknessProperty =
            DependencyProperty.Register("EllipseStrokeThickness", typeof(double), typeof(StateRoundButton), new PropertyMetadata(1.0D));

        public Geometry IconData {
            get { return (Geometry)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }
        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register("IconData", typeof(Geometry), typeof(StateRoundButton));

        public Geometry IconData2 {
            get { return (Geometry)GetValue(IconData2Property); }
            set { SetValue(IconData2Property, value); }
        }
        public static readonly DependencyProperty IconData2Property =
            DependencyProperty.Register("IconData2", typeof(Geometry), typeof(StateRoundButton));

        public double IconHeight {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(StateRoundButton), new PropertyMetadata(12.0D));

        public double IconWidth {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(StateRoundButton), new PropertyMetadata(12.0D));
    }
}
