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

namespace SRNicoNico.Views.Behaviors {
    public class DataGridAutoScrollBehavior : Behavior<DataGrid> {

        protected override void OnAttached() {

            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching() {

            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private DispatcherCollection<NicoNicoCommentEntry> Collection;

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {

            if(AssociatedObject != null) {

                Collection = (DispatcherCollection<NicoNicoCommentEntry>) AssociatedObject.ItemsSource;
                Collection.CollectionChanged += Collection_CollectionChanged;
            }
        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {

            AssociatedObject.ScrollIntoView(Collection.Last());
        }
    }
}
