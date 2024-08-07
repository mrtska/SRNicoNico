using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// ウィンドウのインプット系のイベントをフックしてViewModelに転送する
    /// </summary>
    public class WindowInputEventBehavior : Behavior<Window> {

        protected override void OnAttached() {

            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
            AssociatedObject.PreviewKeyUp += AssociatedObject_PreviewKeyUp;
            AssociatedObject.PreviewMouseDown += AssociatedObject_PreviewMouseDown;
        }

        protected override void OnDetaching() {

            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
            AssociatedObject.PreviewKeyUp -= AssociatedObject_PreviewKeyUp;
            AssociatedObject.PreviewMouseDown -= AssociatedObject_PreviewMouseDown;
        }
        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e) {

            if (AssociatedObject.DataContext is MainWindowViewModel vm) {

                vm.KeyDown(e);
            }
        }

        private void AssociatedObject_PreviewKeyUp(object sender, KeyEventArgs e) {

            if (AssociatedObject.DataContext is MainWindowViewModel vm) {

                vm.KeyUp(e);
            }
        }

        private void AssociatedObject_PreviewMouseDown(object sender, MouseButtonEventArgs e) {

            if (AssociatedObject.DataContext is MainWindowViewModel vm) {

                vm.MouseDown(e);
            }
        }
    }
}
