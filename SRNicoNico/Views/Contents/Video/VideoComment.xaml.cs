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

namespace SRNicoNico.Views.Contents.Video {
    /// <summary>
    /// VideoComment.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoComment : UserControl {
        public VideoComment() {
            InitializeComponent();
        }

        public void FocusComment() {

            comment.Focus();
        }
    }
}
