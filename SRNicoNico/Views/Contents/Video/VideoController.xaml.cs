using SRNicoNico.ViewModels;
using SRNicoNico.Views.Controls;
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

namespace SRNicoNico.Views {
    /// <summary>
    /// VideoController.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoController : UserControl {
        public VideoController() {
            InitializeComponent();
        }

        private void SeekBar_SeekRequested(object sender, SeekRequestedEventArgs e) {

            if(DataContext is VideoViewModel) {

                var vm = (VideoViewModel)DataContext;
                vm.Handler.Seek(e.Position);
            }

        }
    }
}
