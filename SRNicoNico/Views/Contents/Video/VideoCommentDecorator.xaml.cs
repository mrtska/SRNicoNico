using SRNicoNico.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views {
    public partial class VideoCommentDecorator : UserControl {
        public VideoCommentDecorator() {
            InitializeComponent();
        }
        public void ApplyCommentColor(object sender, RoutedEventArgs e) {

            if (DataContext is VideoCommentPostViewModel vm) {
                var content = ((RadioButton)sender).Content as string;

                if (content.Contains(",")) {

                    content = content.Split(',')[0];
                }
                vm.Color = content;
            }
        }
    }
}
