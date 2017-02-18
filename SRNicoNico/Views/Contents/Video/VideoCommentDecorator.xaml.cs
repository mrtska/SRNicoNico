using SRNicoNico.ViewModels;
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
    /// VideoCommentDecorator.xaml の相互作用ロジック
    /// </summary>
    public partial class VideoCommentDecorator : UserControl {
        public VideoCommentDecorator() {
            InitializeComponent();
        }

        public void ApplyCommentColor(object sender, RoutedEventArgs e) {

            if(DataContext is VideoPostCommentViewModel) {

                var vm = (VideoPostCommentViewModel)DataContext;

                var content = ((RadioButton)sender).Content as string;

                if(content.Contains(",")) {

                    content = content.Split(',')[0];
                }
                vm.Color = content;
            }
        }
    }
}
