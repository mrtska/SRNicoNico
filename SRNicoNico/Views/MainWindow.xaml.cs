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
using MetroRadiance.UI.Controls;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views {
    /* 
	 * ViewModelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedWeakEventListenerや
     * CollectionChangedWeakEventListenerを使うと便利です。独自イベントの場合はLivetWeakEventListenerが使用できます。
     * クローズ時などに、LivetCompositeDisposableに格納した各種イベントリスナをDisposeする事でイベントハンドラの開放が容易に行えます。
     *
     * WeakEventListenerなので明示的に開放せずともメモリリークは起こしませんが、できる限り明示的に開放するようにしましょう。
     */

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow {
        public MainWindow() {
            InitializeComponent();
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e) {

            if (DataContext is MainWindowViewModel) {

                var vm = (MainWindowViewModel)DataContext;
                vm.KeyDown(e);
            }
        }

        private void MetroWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e) {

            if (DataContext is MainWindowViewModel) {
                var vm = (MainWindowViewModel)DataContext;
                vm.MouseDown(e);
            }
        }

        private void MetroWindow_PreviewKeyUp(object sender, KeyEventArgs e) {

            var vm = (MainWindowViewModel)DataContext;
            if (DataContext is MainWindowViewModel) {

                vm.KeyUp(e);
            }
        }
    }
}
