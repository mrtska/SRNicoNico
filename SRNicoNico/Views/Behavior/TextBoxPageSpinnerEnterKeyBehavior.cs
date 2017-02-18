using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SRNicoNico.Views.Behavior {
    public class TextBoxPageSpinnerEnterKeyBehavior : Behavior<TextBox> {




        public ViewModel Binding {
            get { return (ViewModel)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Binding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(ViewModel), typeof(TextBoxPageSpinnerEnterKeyBehavior), new PropertyMetadata(null));



        public string MethodName {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register("MethodName", typeof(string), typeof(TextBoxPageSpinnerEnterKeyBehavior), new PropertyMetadata(""));


        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.KeyDown += TextBox_KeyDown;

        }

        protected override void OnDetaching() {
            base.OnDetaching();

            AssociatedObject.KeyDown -= TextBox_KeyDown;
        }

        //Enterキーで検索
        private void TextBox_KeyDown(object sender, KeyEventArgs e) {

            if(e.Key == Key.Enter) {

                InvokeMethod();
            }
        }

        private void InvokeMethod() {


            var type = Binding.GetType();

            var pass = type.GetProperty("CurrentPage");
            int outer;
            if(int.TryParse(AssociatedObject.Text, out outer)) {

                pass.SetValue(Binding, outer);
            } else {

                AssociatedObject.Text = "1";
                pass.SetValue(Binding, 1);
            }



            var method = type.GetMethod(MethodName);
            method.Invoke(Binding, null);
        }

    }
}
