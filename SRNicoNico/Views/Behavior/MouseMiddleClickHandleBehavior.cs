using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SRNicoNico.Views.Behavior {
    public class MouseMiddleClickHandleBehavior : Behavior<Grid> {


        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.MouseDown += Mouse_ButtonDown;

        }

        protected override void OnDetaching() {
            base.OnDetaching();

            AssociatedObject.MouseDown -= Mouse_ButtonDown;
        }


        private void Mouse_ButtonDown(object sender, MouseEventArgs e) {

            if(e.MiddleButton == MouseButtonState.Pressed && AssociatedObject.DataContext is VideoViewModel vm) {

                vm.Close();
            }
        }
    }
}
