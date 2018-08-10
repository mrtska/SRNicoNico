using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SRNicoNico.Views.Behavior {
    public class ScrollViewerScrollingAmountBahavior : Behavior<ScrollViewer> {

        public int StepSize { get; set; }

        #region Attach & Detach
        protected override void OnAttached() {
            AssociatedObject.ScrollChanged += AssociatedObject_ScrollChanged;
            base.OnAttached();
        }

        protected override void OnDetaching() {
            AssociatedObject.ScrollChanged -= AssociatedObject_ScrollChanged;
            base.OnDetaching();
        }
        #endregion

        private void AssociatedObject_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            const double stepSize = 62;
            var scrollViewer = (ScrollViewer)sender;
            var steps = Math.Round(scrollViewer.VerticalOffset / stepSize, 0);
            var scrollPosition = steps * stepSize;
            if (scrollPosition >= scrollViewer.ScrollableHeight) {
                scrollViewer.ScrollToBottom();
                return;
            }
            scrollViewer.ScrollToVerticalOffset(scrollPosition);
        }
    }
}
