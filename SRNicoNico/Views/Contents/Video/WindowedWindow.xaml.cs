using MetroRadiance.Controls;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRNicoNico.Views.Contents.Video {
    /// <summary>
    /// FullScreenWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WindowedWindow : MetroWindow {
        public WindowedWindow() {
            InitializeComponent();
        }

        private void screen_KeyDown(object sender, KeyEventArgs e) {

            if(DataContext is VideoViewModel) {

                var vm = (VideoViewModel)DataContext;
                vm.KeyDown(e);
            }
        }
    }
}