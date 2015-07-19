using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;

namespace SRNicoNico.Views.Behaviors {

	public class ListBoxBehavior : Behavior<ListBox> {

		private ScrollViewer scrollViewer;


		protected override void OnAttached() {
			base.OnAttached();

			this.AssociatedObject.Loaded += AssociatedObject_Loaded;
			this.AssociatedObject.Unloaded += AssociatedObject_Unloaded;
		}

		

		//ここでScrollViewerのインスタンスを取得する
		private void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {


			if(VisualTreeHelper.GetChildrenCount(this.AssociatedObject) != 0) {


				var border = VisualTreeHelper.GetChild(this.AssociatedObject, 0) as Border;
				scrollViewer = border.Child as ScrollViewer;

				scrollViewer.ScrollChanged += scrollViewer_ScrollChanged;
			}
		}


		private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e) {


			scrollViewer.ScrollChanged -= scrollViewer_ScrollChanged;
		}

		void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {

				
			//一番下までスクロールしたら
			if(e.ExtentHeight == e.VerticalOffset + e.ViewportHeight) {

				App.ViewModelRoot.Search.SearchNext();
				this.scrollViewer.ScrollToVerticalOffset(e.ExtentHeight - e.ViewportHeight);

			}
		}




		protected override void OnDetaching() {
			base.OnDetaching();

			this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
			this.AssociatedObject.Unloaded -= AssociatedObject_Unloaded;


		}
	}
}
