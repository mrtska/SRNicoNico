using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SRNicoNico.Views.Controls {
    public class BindableTextBlock : TextBlock {
        public ObservableCollection<Inline> InlineList {
            get { return (ObservableCollection<Inline>)GetValue(InlineListProperty); }
            set { SetValue(InlineListProperty, value); }
        }

        public static readonly DependencyProperty InlineListProperty =
            DependencyProperty.Register("InlineList", typeof(ObservableCollection<Inline>), typeof(BindableTextBlock), new UIPropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            BindableTextBlock textBlock = sender as BindableTextBlock;
            ObservableCollection<Inline> list = e.NewValue as ObservableCollection<Inline>;
            list.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(textBlock.InlineCollectionChanged);
        }

        private void InlineCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
                int idx = e.NewItems.Count - 1;
                Inline inline = e.NewItems[idx] as Inline;
                this.Inlines.Add(inline);
            }
        }
    }
}
