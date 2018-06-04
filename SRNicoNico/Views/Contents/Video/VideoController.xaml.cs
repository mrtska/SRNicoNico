using SRNicoNico.ViewModels;
using SRNicoNico.Views.Controls;
using System.Windows.Controls;

namespace SRNicoNico.Views {
    public partial class VideoController : UserControl {
        public VideoController() {
            InitializeComponent();
        }

        private void SeekBar_SeekRequested(object sender, SeekRequestedEventArgs e) {

            if (DataContext is VideoViewModel vm) {
                vm.Handler.Seek(e.Position);
            }
        }
    }
}
