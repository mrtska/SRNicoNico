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
    public class TextBoxEnterKeyBehavior : Behavior<TextBox> {




        public Type TargetType {
            get { return (Type)GetValue(TargetTypeProperty); }
            set { SetValue(TargetTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetTypeProperty =
            DependencyProperty.Register("TargetType", typeof(Type), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(typeof(string)));



        public bool UseMethodParameter {
            get { return (bool)GetValue(UseMethodParameterProperty); }
            set { SetValue(UseMethodParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseMethodParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseMethodParameterProperty =
            DependencyProperty.Register("UseMethodParameter", typeof(bool), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(false));



        public string FieldName {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FieldNameProperty =
            DependencyProperty.Register("FieldName", typeof(string), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(""));



        public bool UseFieldName {
            get { return (bool)GetValue(UseFieldNameProperty); }
            set { SetValue(UseFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseFieldNameProperty =
            DependencyProperty.Register("UseFieldName", typeof(bool), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(false));






        public ViewModel Binding {
            get { return (ViewModel)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Binding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(ViewModel), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(null));



        public string MethodName {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register("MethodName", typeof(string), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(""));


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

            MethodInfo method;

            //フィールド(というかプロパティ)に書き込む
            if(UseFieldName) {

                var pass = type.GetProperty(FieldName);
                pass.SetValue(Binding, int.Parse(AssociatedObject.Text));
            }

            //メソッドの引数をつかう
            if(UseMethodParameter) {

                method = type.GetMethod(MethodName, new[] { TargetType });
                method.Invoke(Binding, new[] { AssociatedObject.Text });
                return;
            }

            method = type.GetMethod(MethodName);
            method.Invoke(Binding, null);
        }

    }
}
