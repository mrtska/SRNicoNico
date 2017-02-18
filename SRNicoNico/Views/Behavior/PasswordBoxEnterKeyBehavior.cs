﻿using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SRNicoNico.Views.Behavior {
    public class PasswordBoxEnterKeyBehavior : Behavior<PasswordBox> {


        public ViewModel Binding {
            get { return (ViewModel)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Binding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(ViewModel), typeof(PasswordBoxEnterKeyBehavior), new PropertyMetadata(null));



        public string MethodName {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register("MethodName", typeof(string), typeof(PasswordBoxEnterKeyBehavior), new PropertyMetadata(""));


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
            var method = type.GetMethod(MethodName);

            var pass = type.GetProperty("Password");
            pass.SetValue(Binding, AssociatedObject.Password);

            method.Invoke(Binding, null);
        }



    }
}
