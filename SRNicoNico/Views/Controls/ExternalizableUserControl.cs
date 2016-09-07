using System;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media;
using SRNicoNico.Views.Contents.Interface;

namespace SRNicoNico.Views.Controls {

    public class ExternalizableUserControl : UserControl {

        public ExternalizableUserControl() {

            DataContextChanged += ExternalizableUserControl_DataContextChanged;

        }

        private void ExternalizableUserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is IExternalizable) {

                var vm = (IExternalizable)DataContext;
                vm.View = this;
            }

        }
    }
}