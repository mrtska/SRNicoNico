using Livet;
using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Views.Contents.Video;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SRNicoNico.Views.Behaviors {


    public class VideoCommentAutoScrollBehavior : Behavior<DataGrid> {



        public bool AutoScrollEnabled {
            get { return (bool)GetValue(AutoScrollEnabledProperty); }
            set { SetValue(AutoScrollEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoScrollEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollEnabledProperty =
            DependencyProperty.Register("AutoScrollEnabled", typeof(bool), typeof(VideoCommentAutoScrollBehavior), new FrameworkPropertyMetadata(true));

        public string Vpos {
            get { return (string)GetValue(VposProperty); }
            set { SetValue(VposProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Vpos.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VposProperty =
            DependencyProperty.Register("Vpos", typeof(string), typeof(VideoCommentAutoScrollBehavior), new FrameworkPropertyMetadata("0", new PropertyChangedCallback(VposChanged)));

        protected override void OnAttached() {

            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching() {

            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }


        private DispatcherCollection<CommentEntryViewModel> Collection;


        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {

            if(AssociatedObject != null) {

                Collection = (DispatcherCollection<CommentEntryViewModel>)AssociatedObject.ItemsSource;
            }
        }


        private static void VposChanged(DependencyObject obj, DependencyPropertyChangedEventArgs p) {

            var instance = obj as VideoCommentAutoScrollBehavior;

            if(p.NewValue == null || !instance.AutoScrollEnabled || instance.Collection.Count == 0) {

                return;
            }
            instance.AssociatedObject.ScrollIntoView(instance.Collection.Where(e => int.Parse(e.Entry.Vpos) <= int.Parse((string)(p.NewValue))).Last());
        }
    }
}
