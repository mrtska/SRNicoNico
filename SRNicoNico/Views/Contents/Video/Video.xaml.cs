using SRNicoNico.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SRNicoNico.Views {
    public partial class Video : UserControl {
        public Video() {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {

            if (DataContext is VideoViewModel vm) {
                if (vm.ApiData == null) {

                    vm.Initialize();
                }
            }
        }
    }
}
