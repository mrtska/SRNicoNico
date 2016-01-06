using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;

using Livet;

namespace SRNicoNico.Views.Behaviors {

	public class ListBoxInfiniteScrollBehavior : Behavior<ListBox> {

        public ViewModel ViewModel {
            get { return (ViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ViewModel), typeof(ListBoxInfiniteScrollBehavior), new PropertyMetadata(null));

        public string MethodName {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register("MethodName", typeof(string), typeof(ListBoxInfiniteScrollBehavior), new PropertyMetadata(""));

        private ScrollViewer ScrollViewer;

		protected override void OnAttached() {
			base.OnAttached();

			AssociatedObject.Loaded += AssociatedObject_Loaded;
			AssociatedObject.Unloaded += AssociatedObject_Unloaded;
		}

		//ここでScrollViewerのインスタンスを取得する
		private void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {

			if(VisualTreeHelper.GetChildrenCount(AssociatedObject) != 0) {

				var border = VisualTreeHelper.GetChild(AssociatedObject, 0) as Border;
				ScrollViewer = border.Child as ScrollViewer;

				ScrollViewer.ScrollChanged += scrollViewer_ScrollChanged;
			}
		}

		private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e) {

			ScrollViewer.ScrollChanged -= scrollViewer_ScrollChanged;
		}

        private double PrevExtentHeight;

		void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {

            
			//一番下までスクロールしたら
			if(e.ExtentHeight > 1 && PrevExtentHeight != e.ExtentHeight && e.ExtentHeight == (e.VerticalOffset + e.ViewportHeight) || (e.ExtentHeight < e.ViewportHeight && PrevExtentHeight != e.ExtentHeight)) {

                PrevExtentHeight = e.ExtentHeight;
#if DEBUG
                Console.WriteLine(e.ExtentHeight);
                Console.WriteLine(e.VerticalOffset);
#endif
                InvokeMethod();

                ScrollViewer.ScrollToVerticalOffset(e.ExtentHeight - e.ViewportHeight);
                
			}
		}

        private void InvokeMethod() {

            Type type = ViewModel.GetType();
            MethodInfo method = type.GetMethod(MethodName);
            method.Invoke(ViewModel, null);
        }

		protected override void OnDetaching() {
			base.OnDetaching();

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
		}
	}
}
