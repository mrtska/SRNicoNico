using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// TextBoxに数字以外入力出来なくするBehavior
    /// </summary>
    public class TextBoxNumericOnlyBehavior : Behavior<TextBox> {

        protected override void OnAttached() {

            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;

        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e) {

            // 数字キー以外入力を拒否する
            if ((Key.D0 <= e.Key && e.Key <= Key.D9) || (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9) || (Key.Delete == e.Key) || (Key.Back == e.Key) || (Key.Tab == e.Key) || (Key.Enter == e.Key)) {

                e.Handled = false;
            } else {

                e.Handled = true;
            }
        }

        protected override void OnDetaching() {

            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;

        }
    }
}
