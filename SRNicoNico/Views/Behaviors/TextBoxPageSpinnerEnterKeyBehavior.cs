using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Livet;
using Microsoft.Xaml.Behaviors;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// ページネーションUIのTextBox用のBehavior
    /// </summary>
    public class TextBoxPageSpinnerEnterKeyBehavior : Behavior<TextBox> {

        /// <summary>
        /// エンターキーを押した時に実行されるメソッドがあるインスタンス
        /// </summary>
        public PageSpinnerViewModel Binding {
            get { return (PageSpinnerViewModel)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register(nameof(Binding), typeof(PageSpinnerViewModel), typeof(TextBoxPageSpinnerEnterKeyBehavior), new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public string MethodName {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(nameof(MethodName), typeof(string), typeof(TextBoxPageSpinnerEnterKeyBehavior), new PropertyMetadata(""));


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

            if (e.Key == Key.Enter) {

                InvokeMethod();
            }
        }

        private void InvokeMethod() {


            var type = Binding.GetType();

            var page = type.GetProperty(nameof(PageSpinnerViewModel.CurrentPage)) ?? throw new InvalidOperationException("CurrentPageプロパティが取得出来ませんでした");
            if (int.TryParse(AssociatedObject.Text, out var outer)) {

                page.SetValue(Binding, outer);
            } else {

                AssociatedObject.Text = "1";
                page.SetValue(Binding, 1);
            }

            var method = type.GetMethod(MethodName) ?? throw new InvalidOperationException();
            method.Invoke(Binding, new object[] { (int)page.GetValue(Binding)! });
        }

    }
}
