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

namespace SRNicoNico.Views.Contents.Video {
	/// <summary>
	/// Video.xaml の相互作用ロジック
	/// </summary>
	public partial class Video : UserControl {
		public Video() {
			InitializeComponent();
            
		}

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {

            if(DataContext is VideoViewModel) {

                VideoViewModel vm = (VideoViewModel)DataContext;
                vm.Video = this;
            }
        }

        private void TextBlock_TriggerRequest(object sender, RoutedEventArgs e) {

            if(DataContext is VideoViewModel) {

                VideoViewModel vm = (VideoViewModel)DataContext;
                if(e.OriginalSource is Hyperlink) {

                    Hyperlink link = (Hyperlink) e.OriginalSource;

                    vm.OpenLink(link.NavigateUri);
                }
            }
        }
    }
}
