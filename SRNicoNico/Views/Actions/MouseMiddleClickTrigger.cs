using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace SRNicoNico.Views.Actions {
    /// <summary>
    /// マウスの中クリックが押された時にアクションを発火するトリガー
    /// </summary>
    public class MouseMiddleClickTrigger : TriggerBase<UIElement> {

        protected override void OnAttached() {

            AssociatedObject.MouseUp += AssociatedObject_MouseUp;
        }

        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e) {

            if (e.ChangedButton == MouseButton.Middle && e.MiddleButton == MouseButtonState.Released) {

                InvokeActions(e);
            }
        }

        protected override void OnDetaching() {
            AssociatedObject.MouseUp -= AssociatedObject_MouseUp;
        }
    }
}
