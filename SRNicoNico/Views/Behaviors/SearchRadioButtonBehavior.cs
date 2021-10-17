using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Views.Behaviors {
    /// <summary>
    /// ラジオボタンにチェックを入れるBehavior
    /// </summary>
    public class SearchRadioButtonBehavior : Behavior<RadioButton> {
        
        protected override void OnAttached() {

            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {

            if (AssociatedObject.DataContext is SearchGenreFacet facet) {
                // allだったらLoadedイベント時にチェックを入れる
                if (facet.Key == "all") {
                    AssociatedObject.IsChecked = true;
                }
            }
        }

        protected override void OnDetaching() {

            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }
    }
}
