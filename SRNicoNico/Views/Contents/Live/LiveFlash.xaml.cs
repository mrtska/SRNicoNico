using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.IO;
using SRNicoNico.ViewModels;
using System.Windows.Interop;
using AxShockwaveFlashObjects;
using Flash.External;

namespace SRNicoNico.Views.Contents.Live {
    /// <summary>
    /// VideoFlash.xaml の相互作用ロジック
    /// </summary>
    public partial class LiveFlash : UserControl {
        public LiveFlash() {
            InitializeComponent();
        }
        
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is LiveViewModel) {

                var vm = (LiveViewModel) DataContext;
                if(vm.LiveFlash == this && vm.FullScreenLiveFlash == this) {

                    return;
                }

                //インスタンスを設定
                vm.Handler = new LiveFlashHandler(vm, flash);
            }
        }
    }
}
