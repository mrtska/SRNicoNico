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

namespace SRNicoNico.Views.Behaviors {
    public class ListBoxAutoScrollBehavior : Behavior<ListBox> {

        protected override void OnAttached() {

            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching() {

            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private ScrollViewer ScrollViewer;

        private DispatcherCollection<ViewModels.AccessLogEntryViewModel> Collection;

        //ここでScrollViewerのインスタンスを取得する
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {

            if(VisualTreeHelper.GetChildrenCount(this.AssociatedObject) != 0) {

                var border = VisualTreeHelper.GetChild(this.AssociatedObject, 0) as Border;
                ScrollViewer = border.Child as ScrollViewer;

                if(AssociatedObject.ItemsSource is DispatcherCollection<ViewModels.AccessLogEntryViewModel>) {

                    Collection = (DispatcherCollection<ViewModels.AccessLogEntryViewModel>) AssociatedObject.ItemsSource;
                    Collection.CollectionChanged += Collection_CollectionChanged;
                }
            }
        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {

            if(ScrollViewer != null) {

                ScrollViewer.ScrollToEnd();
            }
        }
    }
}
