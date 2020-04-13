using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Livet;
using Microsoft.Xaml.Behaviors;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// TextBoxのEnterキーをフックするBehavior
    /// </summary>
    public class TextBoxEnterKeyBehavior : Behavior<TextBox> {

        /// <summary>
        /// 呼び出したいメソッドに引数を付けたい場合、Trueを設定する
        /// </summary>
        public bool UseMethodParameter {
            get { return (bool)GetValue(UseMethodParameterProperty); }
            set { SetValue(UseMethodParameterProperty, value); }
        }
        public static readonly DependencyProperty UseMethodParameterProperty =
            DependencyProperty.Register(nameof(UseMethodParameter), typeof(bool), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(false));

        /// <summary>
        /// <see cref="UseProperty" /> がTrueの場合、Enterキーが押された時に更新したいプロパティを設定する Falseの場合は無視される
        /// </summary>
        public string PropertyName {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }
        public static readonly DependencyProperty FieldNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(""));

        /// <summary>
        /// Enterキーが押された時にプロパティを更新したい時にTrueにする
        /// Trueの場合は <see cref="PropertyName"/> に値を設定する
        /// </summary>
        public bool UseProperty {
            get { return (bool)GetValue(UseFieldNameProperty); }
            set { SetValue(UseFieldNameProperty, value); }
        }
        public static readonly DependencyProperty UseFieldNameProperty =
            DependencyProperty.Register(nameof(UseProperty), typeof(bool), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(false));

        /// <summary>
        /// Enterキーが押された時に呼び出したいメソッドがあるViewModelインスタンスを設定する
        /// </summary>
        public ViewModel Binding {
            get { return (ViewModel)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register(nameof(Binding), typeof(ViewModel), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Enterキーが押された時に呼び出したいメソッドの名前を設定する
        /// </summary>
        public string MethodName {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(nameof(MethodName), typeof(string), typeof(TextBoxEnterKeyBehavior), new PropertyMetadata(""));


        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.KeyDown += TextBox_KeyDown;
        }

        protected override void OnDetaching() {
            base.OnDetaching();

            AssociatedObject.KeyDown -= TextBox_KeyDown;
        }


        /// <summary>
        /// Enterキーで検索
        /// </summary>
        /// <param name="sender">TextBoxのインスタンス</param>
        /// <param name="e">キーイベント</param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e) {

            if (e.Key == Key.Enter) {

                InvokeMethod();
                e.Handled = true;
            }
        }

        /// <summary>
        /// リフレクションで指定されたメソッドを呼び出す
        /// </summary>
        private void InvokeMethod() {

            var type = Binding.GetType();

            // プロパティに書き込む
            if (UseProperty) {

                var pass = type.GetProperty(PropertyName);
                pass.SetValue(Binding, int.Parse(AssociatedObject.Text));
            }

            // メソッドの引数をつかう
            if (UseMethodParameter) {

                // 引数有りで実行する
                var method = type.GetMethod(MethodName, new[] { typeof(string) });
                method.Invoke(Binding, new[] { AssociatedObject.Text });
            } else {

                // 引数無しでメソッドを呼び出す
                var method = type.GetMethod(MethodName);
                method.Invoke(Binding, null);
            }
        }
    }
}
