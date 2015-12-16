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
using System.Reflection;

namespace SRNicoNico.Views.Contents.Video {
    /// <summary>
    /// VideoFlash.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoFlash : UserControl {




        public VideoFlash() {
            InitializeComponent();
        }




        private void browser_LoadCompleted(object sender, NavigationEventArgs e) {

#if DEBUG
            if((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)) {
                return;
            }
#endif
            if(DataContext is VideoViewModel) {

                VideoViewModel vm = (VideoViewModel)DataContext;
                //動画オープン
                vm.OpenVideo();
            }
            
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is VideoViewModel) {

                VideoViewModel vm = (VideoViewModel) DataContext;
                if(vm.VideoFlash == this) {

                    return;
                }
                //インスタンスを設定
                vm.WebBrowser = browser;
                vm.VideoFlash = this;
            }
        }

    }
}
