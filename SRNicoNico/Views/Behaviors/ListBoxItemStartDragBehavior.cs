using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;

using SRNicoNico.ViewModels;
using SRNicoNico.Views.Contents.Mylist;

namespace SRNicoNico.Views.Behaviors {
    public class ListBoxItemStartDragBehavior : Behavior<Control> {



        protected override void OnAttached() {
            base.OnAttached();

            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            
        }

        protected override void OnDetaching() {
            base.OnDetaching();

        }


        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e) {


            if(AssociatedObject is MylistResultEntry) {

                var obj = (MylistResultEntry)AssociatedObject;
                if(obj.DataContext is MylistListEntryViewModel) {

                    var vm = (MylistListEntryViewModel) obj.DataContext;
                    DragDrop.DoDragDrop(AssociatedObject, vm, DragDropEffects.All);

                }


            }


        }





    }
}
