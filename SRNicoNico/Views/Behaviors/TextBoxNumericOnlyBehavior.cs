using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

using SRNicoNico.Models.NicoNicoWrapper;
using System.Windows.Input;

namespace SRNicoNico.Views.Behaviors {
    public class TextBoxNumericOnlyBehavior : Behavior<TextBox> {


        

        protected override void OnAttached() {

            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;

        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e) {

            if((Key.D0 <= e.Key && e.Key <= Key.D9) || (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9) || (Key.Delete == e.Key) || (Key.Back == e.Key) || (Key.Tab == e.Key) || (Key.Enter == e.Key)) {

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
