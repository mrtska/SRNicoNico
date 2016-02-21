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
using MetroRadiance.UI.Controls;

namespace SRNicoNico.Views.Contents.PlayList {
    /// <summary>
    /// PlayList.xaml の相互作用ロジック
    /// </summary>
    public partial class PlayList : UserControl {
        public PlayList() {
            InitializeComponent();
        }

        private void TabView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            var listbox = sender as TabView;
            if(e.AddedItems.Count == 0) {

                return;
            }

            listbox.ScrollIntoView(e.AddedItems[0]);
        }
    }
}
