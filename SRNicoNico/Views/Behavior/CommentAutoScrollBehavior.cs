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
using System.Windows.Input;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views.Behavior {
    public class CommentAutoScrollBehavior : Behavior<DataGrid> {

        public bool AutoScrollEnabled {
            get { return (bool)GetValue(AutoScrollEnabledProperty); }
            set { SetValue(AutoScrollEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoScrollEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollEnabledProperty =
            DependencyProperty.Register("AutoScrollEnabled", typeof(bool), typeof(CommentAutoScrollBehavior), new FrameworkPropertyMetadata(true));

        public int Vpos {
            get { return (int)GetValue(VposProperty); }
            set { SetValue(VposProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Vpos.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VposProperty =
            DependencyProperty.Register("Vpos", typeof(int), typeof(CommentAutoScrollBehavior), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(VposChanged)));

        private static void VposChanged(DependencyObject obj, DependencyPropertyChangedEventArgs p) {

#if DEBUG
            if((bool)(System.ComponentModel.DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)) {
                return;
            }
#endif
            var instance = obj as CommentAutoScrollBehavior;

            if (!(instance.AssociatedObject.ItemsSource is ObservableSynchronizedCollection<VideoCommentEntryViewModel> collection) ||
                !instance.AutoScrollEnabled || instance.AssociatedObject.IsMouseOver || collection.Count == 0) {

                return;
            }
            var col = collection.Where(e => e.Item.Vpos <= (int)(p.NewValue)).LastOrDefault();
            if (col == null) {

                return;
            }
            instance.AssociatedObject.ScrollIntoView(col);
        }
    }
}
