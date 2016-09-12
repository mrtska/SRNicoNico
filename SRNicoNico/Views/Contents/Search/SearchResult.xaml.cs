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

using Livet;
using SRNicoNico.ViewModels;

namespace SRNicoNico.Views.Contents.Search {
	/// <summary>
	/// SearchResult.xaml の相互作用ロジック
	/// </summary>
	public partial class SearchResult : UserControl {

        

        public SearchResult() {
			InitializeComponent();
		}

        private void Button_Click(object sender, RoutedEventArgs e) {

            if(DataContext is SearchResultViewModel) {

                var vm = (SearchResultViewModel)DataContext;

                vm.CurrentPage--;
                vm.Owner.SearchPage(vm.CurrentPage);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {

            if(DataContext is SearchResultViewModel) {

                var vm = (SearchResultViewModel)DataContext;

                vm.CurrentPage++;
                vm.Owner.SearchPage(vm.CurrentPage);

            }
        }
    }
}
