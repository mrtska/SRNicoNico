using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Livet;
using Microsoft.Xaml.Behaviors;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// リストボックスでインフィニットスクロールを実装するためにスクロールが一番下まで行った時にイベントを拾えるようにするBehavior
    /// </summary>
    public class ListBoxInfiniteScrollBehavior : Behavior<ListBox> {

        /// <summary>
        /// スクロールバーが一番下までスクロールされた時に呼び出したいメソッドの名前を設定する
        /// </summary>
        public string MethodName {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(nameof(MethodName), typeof(string), typeof(ListBoxInfiniteScrollBehavior), new PropertyMetadata(""));

        /// <summary>
        /// スクロールバーが一番下までスクロールされた時に呼び出したいメソッドがあるViewModelインスタンスを設定する
        /// </summary>
        public ViewModel Binding {
            get { return (ViewModel)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register(nameof(Binding), typeof(ViewModel), typeof(ListBoxInfiniteScrollBehavior), new PropertyMetadata(null));

        private ScrollViewer? ScrollViewer;

        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.Loaded += ListBox_Loaded;
        }

        /// <summary>
        /// 対象のListBoxのテンプレートがロードされた時に呼ばれる
        /// </summary>
        private void ListBox_Loaded(object _, RoutedEventArgs e) {

            var border = (Border)VisualTreeHelper.GetChild(AssociatedObject, 0);
            var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);

            ScrollViewer = scrollViewer;

            scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {

            // 画面に映っている範囲の高さ(ViewportHeight) + 縦スクロール位置(VerticalOffset) が
            // コンテンツ全体の高さ(ExtentHeight)だったら一番下までスクロールされたということ
            if (Math.Round(e.ViewportHeight + e.VerticalOffset, 0) == Math.Round(e.ExtentHeight, 0)) {

                var type = Binding.GetType();
                // 引数無しでメソッドを呼び出す
                var method = type.GetMethod(MethodName);
                method?.Invoke(Binding, null);
            }
        }

        protected override void OnDetaching() {

            // Scrollviewer?.ScrollChanged -= と書くとコンパイルエラーになる・・・
            if (ScrollViewer != null) {
                ScrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }

            AssociatedObject.Loaded -= ListBox_Loaded;

            base.OnDetaching();
        }
    }
}
